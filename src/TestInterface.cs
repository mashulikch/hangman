namespace Hangman;
using System;

/// <summary>
/// Демо/тестовый режим: сравнивает два слова посимвольно, используется для автотестов
/// </summary>
public class TestInterface
{
    /// <summary>
    /// Принимает загаданное слово и попытку пользователя, возвращает результат проверки по правилам задачи
    /// </summary>
    public static void Run(string word, string guess)
    {
        if (word.Length != guess.Length)
        {
            Console.WriteLine("Ошибка: длины слов не совпадают");
            return;
        }

        var engine = new HangmanEngine(word);
        for (int i = 0; i < guess.Length; i++)
        {
            engine.Guess(guess[i]);
        }

        var state = engine.GetCurrentState();
        var result = (state == word) ? "POS" : "NEG";
        Console.WriteLine($"{state};{result}");
    }
}