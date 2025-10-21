using System;
using System.IO;
using NUnit.Framework;
using Hangman;


// Tесты для HangmanEngine
[TestFixture]
public class HangmanEngineTests
{
    [Test]
    public void CorrectlyDisplaysGuessedLetters()
    {
        var engine = new HangmanEngine("дом");
        engine.Guess('д');
        engine.Guess('м');
        Assert.That(engine.GetCurrentState(), Is.EqualTo("д*м"));
    }

    [Test]
    public void IsCaseInsensitive()
    {
        var engine = new HangmanEngine("ВИСЕЛИЦА");
        engine.Guess('в'); // нижний регистр к верхнему слову
        engine.Guess('И'); // верхний регистр к нижнему внутри движка
        Assert.That(engine.GetCurrentState(), Does.StartWith("ви"));
        Assert.That(engine.Errors, Is.EqualTo(0), "верные буквы не должны увеличивать ошибки");
    }

    [Test]
    public void FailsOnTooManyErrors()
    {
        var engine = new HangmanEngine("кот", 2);
        engine.Guess('ы');
        engine.Guess('у');
        Assert.IsTrue(engine is { GameOver: true, IsLost: true });
        Assert.IsFalse(engine.IsWon);
    }

    [Test]
    public void Constructor_ValidWord_InitializesCorrectly()
    {
        var engine = new HangmanEngine("тест", 6);
        Assert.That(engine.GetCurrentState(), Is.EqualTo("****"));
        Assert.That(engine.Errors, Is.EqualTo(0));
        Assert.IsFalse(engine.GameOver);
    }

    [Test]
    public void Guess_IncorrectLetter_ReturnsFalse()
    {
        var engine = new HangmanEngine("кот");
        Assert.IsFalse(engine.Guess('м'));
        Assert.That(engine.Errors, Is.EqualTo(1));
    }

    [TestCase("а")]
    [TestCase("абв")]
    [TestCase("привет")]
    public void Constructor_DifferentWordLengths_WorksCorrectly(string word)
    {
        var engine = new HangmanEngine(word);
        var expectedStars = new string('*', word.Length);
        Assert.That(engine.GetCurrentState(), Is.EqualTo(expectedStars));
    }

    [Test]
    public void State_Changes_OnHit_And_NotOnRepeat()
    {
        var engine = new HangmanEngine("пес", 6);
        Assert.IsTrue(engine.Guess('п'));
        Assert.That(engine.GetCurrentState(), Is.EqualTo("п**"));

        // Повтор той же буквы не меняет состояние и не добавляет ошибку
        Assert.IsFalse(engine.Guess('П'));
        Assert.That(engine.GetCurrentState(), Is.EqualTo("п**"));
        Assert.That(engine.Errors, Is.EqualTo(0));
    }
}
;

// Тесты для Dictionary
[TestFixture]
public class DictionaryTests
{
    [Test]
    public void GetRandomWord_ValidParams_ReturnsWordFromCategoryAndDifficulty_AndHintMatches()
    {
        var rand = new Random(42);
        var category = "Животные";
        var difficulty = "Лёгкий";

        var (word, hint) = Dictionary.GetRandomWord(rand, category, difficulty);

        var expectedPool = Dictionary.WordsByCategoryDifficulty[category][difficulty];
        Assert.That(expectedPool, Has.Member(word));

        if (Dictionary.Hints.TryGetValue(word, out var expectedHint))
        {
            Assert.That(hint, Is.EqualTo(expectedHint));
        }
        else
        {
            Assert.IsNull(hint);
        }
    }

    [Test]
    public void Words_AllCategories_ContainAllDifficulties()
    {
        var expectedDifficulties = new[] { "Лёгкий", "Средний", "Сложный" };
        foreach (var category in Dictionary.WordsByCategoryDifficulty.Keys)
        {
            foreach (var difficulty in expectedDifficulties)
            {
                Assert.IsTrue(
                    Dictionary.WordsByCategoryDifficulty[category].ContainsKey(difficulty),
                    $"Category {category} missing difficulty {difficulty}"
                );
            }
        }
    }
}

// Интеграционные тесты интерактивного режима
[TestFixture]
public class InteractiveInterfaceTests
{
    [Test]
    public void LongInput_RePrompts_WithoutChangingState_AndEventuallyLoses()
    {
        // Последовательность ввода:
        // 1) Категория "1" -> "Животные"
        // 2) Сложность "1" -> "Лёгкий" => maxErrors = 7
        // 3) "аб" -> опечатка (строка > 1 символов) => повторный ввод
        // 4) 7 букв, которых нет ни в "кот","пес","лев": щ й э ю я б г
        var input = string.Join(Environment.NewLine, new[]
        {
            "1",
            "1",
            "аб",
            "щ","й","э","ю","я","б","г"
        }) + Environment.NewLine;

        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            using var sr = new StringReader(input);
            using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            InteractiveInterface.Run();

            var output = sw.ToString();
            // Ищем строки "Попыток осталось: X" — первые две после старта должны быть одинаковыми (из-за опечатки)
            var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var attempts = lines.Where(l => l.StartsWith("Попыток осталось:")).ToList();
            Assert.That(attempts.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(attempts[0], Is.EqualTo("Попыток осталось: 7"));
            Assert.That(attempts[1], Is.EqualTo("Попыток осталось: 7"));

            Assert.That(output, Does.Contain("Введите одну русскую букву!"));
            Assert.That(output, Does.Contain("Вы проиграли! Слово было:"));
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}

// Интеграционные тесты (движок)
[TestFixture]
public class EngineIntegrationTests
{
    [Test]
    public void FullGame_WinScenario()
    {
        var engine = new HangmanEngine("кот", 6);
        engine.Guess('к');
        Assert.IsFalse(engine.GameOver);
        engine.Guess('о');
        Assert.IsFalse(engine.GameOver);
        engine.Guess('т');
        Assert.IsTrue(engine.IsWon);
        Assert.IsTrue(engine.GameOver);
    }

    [Test]
    public void FullGame_LoseScenario()
    {
        var engine = new HangmanEngine("кот", 3);
        engine.Guess('м');
        engine.Guess('н');
        engine.Guess('п');
        Assert.IsTrue(engine.IsLost);
        Assert.IsTrue(engine.GameOver);
    }
}