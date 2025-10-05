namespace Hangman;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Класс-словaрь для игры. Содержит основной набор слов, распределённых по категориям и уровням сложности,
/// а также подсказки к словам и методы их получения.
/// </summary>
public static class Dictionary
{
    /// <summary>
    /// Основной словарь: категория → уровень сложности → список слов.
    /// </summary>
    public static Dictionary<string, Dictionary<string, List<string>>> WordsByCategoryDifficulty = new()
    {
        { "Животные", new() { { "Лёгкий", new List<string> { "кот", "пес", "лев" } }, { "Средний", new List<string> { "жираф", "енот", "лиса" } }, { "Сложный", new List<string> { "хамелеон", "кенгуру", "крокодил" } } } },
        { "Города", new() { { "Лёгкий", new List<string> { "минск", "казань", "омск" } }, { "Средний", new List<string> { "калуга", "тверь", "томск" } }, { "Сложный", new List<string> { "владивосток", "екатеринбург", "иннополис" } } } },
        { "Предметы", new() { { "Лёгкий", new List<string> { "стол", "лампа", "стул" } }, { "Средний", new List<string> { "кресло", "комод", "кровать" } }, { "Сложный", new List<string> { "микроскоп", "термометр", "спидометр" } } } }
    };

    /// <summary>
    /// Подсказки для слов. Ключ — слово, значение — текст подсказки.
    /// </summary>
    public static readonly Dictionary<string, string> Hints = new()
    {
        { "кот", "Домашнее животное" },
        { "пес", "Друг человека" },
        { "лев", "Король саваны" },
        { "енот", "Любит полоскать пищу" },
        { "лиса", "Умеет хихикать" },
        { "жираф", "Очень длинная шея" },
        { "хамелеон", "Меняет цвет" },
        { "кенгуру", "Прыгает по Австралии" },
        { "крокодил", "Гена" },
        { "минск", "Столица Беларуси" },
        { "казань", "Родина КАМАЗа" },
        { "омск", "Есть песня у Смешариков" },
        { "калуга", "Космический город" },
        { "тверь", "вкусные пряники" },
        { "томск", "Сибирские Афины" },
        { "екатеринбург", "город Бесов" },
        { "владивосток", "Порт у Японского моря" },
        { "иннополис", "IT-город в Татарстане" },
        { "стол", "Поставишь обед" },
        { "лампа", "Источает свет" },
        { "стул", "можно сидеть" },
        { "кресло", "Мягкая сидушка" },
        { "комод", "Где лежат вещи" },
        { "кровать", "обычно на этом спят" },
        { "термометр", "Измеряет температуру" },
        { "микроскоп", "Увидеть незримое" },
        { "спидометр", "измеряют скорость" },
    };

    /// <summary>
    /// Возвращает случайную категорию из доступных.
    /// </summary>
    public static string GetRandomCategory(Random rand)
    {
        var keys = WordsByCategoryDifficulty.Keys.ToList();
        return keys[rand.Next(keys.Count)];
    }

    /// <summary>
    /// Возвращает случайное слово из указанной категории и уровня сложности.
    /// </summary>
    public static (string word, string? hint) GetRandomWord(Random rand, string category, string difficulty)
    {
        var words = WordsByCategoryDifficulty[category][difficulty];
        var word = words[rand.Next(words.Count)];
        Hints.TryGetValue(word, out var hint);
        return (word, hint);
    }
}