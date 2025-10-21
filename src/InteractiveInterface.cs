namespace Hangman;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Класс реализует интерактивный (пользовательский) режим игры "Виселица"
/// </summary>
public abstract class InteractiveInterface
{
    /// <summary>
    /// Запуск основного игрового процесса в интерактивном режиме
    /// </summary>
    public static void Run()
    {
        var rand = new Random();
        var allCategories = new List<string>(Dictionary.WordsByCategoryDifficulty.Keys);
        var allDifficulties = new List<string> { "Лёгкий", "Средний", "Сложный" };

        string chosenCategory;
        string chosenDifficulty;

        // Выбор категории
        Console.WriteLine("Выберите категорию, написав число (или Enter для случайной):");
        for (int i = 0; i < allCategories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {allCategories[i]}");
        }

        var categoryInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(categoryInput))
        {
            chosenCategory = Dictionary.GetRandomCategory(rand);
            Console.WriteLine($"Случайно выбрана категория: {chosenCategory}");
        }
        else if (int.TryParse(categoryInput, out int catNum) && catNum >= 1 && catNum <= allCategories.Count)
        {
            chosenCategory = allCategories[catNum - 1];
        }
        else
        {
            chosenCategory = Dictionary.GetRandomCategory(rand);
            Console.WriteLine($"Некорректно, случайно выбрана категория: {chosenCategory}");
        }

        // Выбор сложности
        Console.WriteLine("Выберите сложность, написав число (или Enter для случайной):");
        for (int i = 0; i < allDifficulties.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {allDifficulties[i]}");
        }

        var difficultyInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(difficultyInput))
        {
            chosenDifficulty = allDifficulties[rand.Next(allDifficulties.Count)];
            Console.WriteLine($"Случайно выбрана сложность: {chosenDifficulty}");
        }
        else if (int.TryParse(difficultyInput, out int diffNum) && diffNum >= 1 && diffNum <= allDifficulties.Count)
        {
            chosenDifficulty = allDifficulties[diffNum - 1];
        }
        else
        {
            chosenDifficulty = allDifficulties[rand.Next(allDifficulties.Count)];
            Console.WriteLine($"Некорректно, случайно выбрана сложность: {chosenDifficulty}");
        }

        var maxErrors = chosenDifficulty switch
        {
            "Лёгкий" => 7,
            "Средний" => 6,
            "Сложный" => 5,
            _ => 6
        };

        var (word, hint) = Dictionary.GetRandomWord(rand, chosenCategory, chosenDifficulty);
        var engine = new HangmanEngine(word, maxErrors);
        
        Console.WriteLine($"Категория: {chosenCategory}, сложность: {chosenDifficulty}.");
        Console.WriteLine($"У вас максимум {maxErrors} ошибок.");
        int threshold = (maxErrors + 1) / 2;
        // Основной игровой цикл
        while (!engine.GameOver)
        {
            // Отрисовка состояния, обработка ошибок, показ подсказки и т.д
            HangmanAscii.Print(engine.Errors, maxErrors);
            Console.WriteLine(engine.GetCurrentState());
            Console.WriteLine($"Попыток осталось: {maxErrors - engine.Errors}");
            
            if (maxErrors - engine.Errors <= threshold)
            {
                Console.WriteLine($"Подсказка: {hint}");
            }
            
            Console.Write("Введите букву: ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input) || input.Length != 1 || !input.All(IsCyrillic))
            {
                Console.WriteLine("Введите одну русскую букву!");
                continue;
            }
            char ch = char.ToLower(input[0]);

            if (engine.HasTried(ch))
            {
                Console.WriteLine("Эта буква уже использована");
                continue;
            }

            engine.Guess(ch);
        }
        
        // Итоговое сообщение пользователю — победа или поражение
        if (engine.IsWon)
        {
            Console.WriteLine($"Поздравляем! Вы отгадали: {word}");
        }
        else
        {
            HangmanAscii.Print(engine.Errors, maxErrors);
            Console.WriteLine($"Вы проиграли! Слово было: {word}");
        }
    }

    private static bool IsCyrillic(char c)
    {
        return (c >= '\u0400' && c <= '\u04FF') || // Основная кириллица
               (c >= '\u0500' && c <= '\u052F');   // Дополнительная кириллица
    }
}