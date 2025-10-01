using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Логика игровой сессии "Виселица": хранит состояние игры, обрабатывает ввод игрока
/// </summary>
public class HangmanEngine
{
    private string secretWord; //Загаданное слово (в нижнем регистре)
    private HashSet<char> guessedLetters = new(); // Множество введённых (отгаданных и неотгаданных) букв
    private int maxErrors; //Максимально допустимое число ошибок
    public int Errors { get; private set; } = 0; //Количество уже допущенных ошибок
    public bool GameOver => IsWon || IsLost; //Закончена ли игра (победа или поражение)
    public bool IsWon => secretWord.All(c => guessedLetters.Contains(c)); //Победа (все буквы открыты)
    public bool IsLost => Errors >= maxErrors; //Поражение (достигнут лимит ошибочных попыток)

    /// <summary>
    /// Конструктор: принимает слово и лимит ошибок
    /// </summary>
    public HangmanEngine(string word, int maxErrors = 6)
    {
        secretWord = word.ToLower();
        this.maxErrors = maxErrors;
    }

    /// <summary>
    /// Обрабатывает ввод буквы, возвращает True если буква есть в слове
    /// </summary>
    public bool Guess(char letter)
    {
        letter = Char.ToLower(letter);
        if (secretWord.Contains(letter))
        {
            guessedLetters.Add(letter);
            return true;
        }
        guessedLetters.Add(letter);
        Errors++;
        return false;
    }

    /// <summary>
    /// Формирует текущее состояние слова (открытые буквы/звёздочки).
    /// </summary>
    public string GetCurrentState()
    {
        return new string(secretWord.Select(c => guessedLetters.Contains(c) ? c : '*').ToArray());
    }
}