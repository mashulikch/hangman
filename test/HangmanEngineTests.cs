namespace DefaultNamespace;
using NUnit.Framework;

//Tесты для HangmanEngine
[TestFixture]
public class HangmanEngineTests
{
    [Test]
    public void CorrectlyDisplaysGuessedLetters()
    {
        var engine = new HangmanEngine("дом");
        engine.Guess('д');
        engine.Guess('м');
        Assert.AreEqual("д*м", engine.GetCurrentState());
    }

    [Test]
    public void IsCaseInsensitive()
    {
        var engine = new HangmanEngine("ВИСЕЛИЦА");
        engine.Guess('в');
        engine.Guess('и');
        Assert.IsTrue(engine.GetCurrentState().StartsWith("ви"));
    }

    [Test]
    public void FailsOnTooManyErrors()
    {
        var engine = new HangmanEngine("кот", 2);
        engine.Guess('ы');
        engine.Guess('у');
        Assert.IsTrue(engine.GameOver && engine.IsLost);
    }
    
    [Test]
    public void Constructor_ValidWord_InitializesCorrectly()
    {
        var engine = new HangmanEngine("тест", 6);
        Assert.AreEqual("****", engine.GetCurrentState());
        Assert.AreEqual(0, engine.Errors);
        Assert.IsFalse(engine.GameOver);
    }
    
    [Test]
    public void Guess_IncorrectLetter_ReturnsFalse()
    {
        var engine = new HangmanEngine("кот");
        Assert.IsFalse(engine.Guess('м'));
        Assert.AreEqual(1, engine.Errors);
    }
    
    [TestCase("а")]
    [TestCase("абв")]
    [TestCase("привет")]
    public void Constructor_DifferentWordLengths_WorksCorrectly(string word)
    {
        var engine = new HangmanEngine(word);
        var expectedStars = new string('*', word.Length);
        Assert.AreEqual(expectedStars, engine.GetCurrentState());
    }
}

//Тесты для Dictionary
[TestFixture]
public class DictionaryTests
{
    [Test]
    public void GetRandomWord_ValidParams_ReturnsWordFromCategory()
    {
        var rand = new Random(42); // Фиксированное зерно для предсказуемости
        var (word, _) = Dictionary.GetRandomWord(rand, "Животные", "Лёгкий");
        
        var expectedWords = new[] { "кот", "пес" };
        Assert.Contains(word, expectedWords);
    }

    [Test]
    public void GetCategoryAndDifficulty_ValidParams_ReturnsCorrectFormat()
    {
        var result = Dictionary.GetCategoryAndDifficulty("Животные", "Лёгкий");
        Assert.AreEqual("Категория: Животные, сложность: Лёгкий.", result);
    }

    [Test]
    public void GetWordHint_ExistingWord_ReturnsHint()
    {
        var result = Dictionary.GetWordHint("кот");
        Assert.AreEqual("Подсказка: Домашнее животное", result);
    }

    [Test]
    public void GetWordHint_NonExistingWord_ReturnsEmpty()
    {
        var result = Dictionary.GetWordHint("несуществующееслово");
        Assert.AreEqual("", result);
    }

    [Test]
    public void Words_AllCategories_ContainAllDifficulties()
    {
        var expectedDifficulties = new[] { "Лёгкий", "Средний", "Сложный" };
        
        foreach (var category in Dictionary.Words.Keys)
        {
            foreach (var difficulty in expectedDifficulties)
            {
                Assert.IsTrue(Dictionary.Words[category].ContainsKey(difficulty),
                    $"Category {category} missing difficulty {difficulty}");
            }
        }
    }
}

//Тесты для неинтерактивного режима
[TestFixture]
public class TestInterfaceTests
{
    private StringWriter output;
    private TextWriter originalOutput;

    [SetUp]
    public void Setup()
    {
        originalOutput = Console.Out;
        output = new StringWriter();
        Console.SetOut(output);
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetOut(originalOutput);
    }

    [TestCase("кот", "кот", "кот;POS")]
    [TestCase("кот", "мот", "*от;NEG")]
    [TestCase("волокно", "барахло", "******о;NEG")]
    [TestCase("окно", "окно", "окно;POS")]
    public void Run_ValidInputs_ProducesCorrectOutput(string word, string guess, string expected)
    {
        TestInterface.Run(word, guess);
        var result = output.ToString().Trim();
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void Run_DifferentLengths_ShowsError()
    {
        TestInterface.Run("кот", "котик");
        var result = output.ToString();
        Assert.Contains("Ошибка: длины слов не совпадают", result);
    }

    [Test]
    public void Run_EmptyStrings_HandledCorrectly()
    {
        TestInterface.Run("", "");
        var result = output.ToString().Trim();
        Assert.AreEqual(";POS", result);
    }
}
//Тесты валидации ввода
[TestFixture]
public class InputValidationTests
{
    [TestCase('а', true)]
    [TestCase('Я', true)]
    [TestCase('ё', true)]
    [TestCase('Ё', true)]
    [TestCase('1', false)]
    [TestCase('!', false)]
    [TestCase(' ', false)]
    [TestCase('a', false)]
    [TestCase("R", false)]
        
    public void IsCyrillic_VariousCharacters_ReturnsExpectedResult(char c, bool expected)
    {
        var result = InputValidator.IsCyrillic(c);
        Assert.AreEqual(expected, result);
    }
}
//Интеграционные тесты
[TestFixture]
public class IntegrationTests
{
    [Test]
    public void FullGame_WinScenario_CompletesSuccessfully()
    {
        var engine = new HangmanEngine("кот", 6);
        
        // Симуляция полной игры
        engine.Guess('к');
        Assert.IsFalse(engine.GameOver);
        
        engine.Guess('о');
        Assert.IsFalse(engine.GameOver);
        
        engine.Guess('т');
        Assert.IsTrue(engine.IsWon);
        Assert.IsTrue(engine.GameOver);
    }

    [Test]
    public void FullGame_LoseScenario_CompletesSuccessfully()
    {
        var engine = new HangmanEngine("кот", 3);
        
        engine.Guess('м');
        engine.Guess('н');
        engine.Guess('п');
        
        Assert.IsTrue(engine.IsLost);
        Assert.IsTrue(engine.GameOver);
    }
}
