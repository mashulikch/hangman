namespace Hangman;
using System;

/// <summary>
/// Отвечает за визуализацию виселицы в консоли
/// </summary>
public  static class HangmanAscii
{
    private static readonly string[] Stages =
    [
        // 0 ошибок
        "     \n" +
        "     \n" +
        "     \n" +
        "     \n" +
        "     \n" +
        "=====",

        // 1 ошибка — стойка
        "     |\n" +
        "     |\n" +
        "     |\n" +
        "     |\n" +
        "     |\n" +
        "=====",

        // 2 ошибки — верх перекладины
        " +---+\n" +
        "     |\n" +
        "     |\n" +
        "     |\n" +
        "     |\n" +
        "=====",

        // 3 ошибки — веревка
        " +---+\n" +
        " |   |\n" +
        "     |\n" +
        "     |\n" +
        "     |\n" +
        "=====",

        // 4 ошибки — голова
        " +---+\n" +
        " |   |\n" +
        " O   |\n" +
        "     |\n" +
        "     |\n" +
        "=====",

        // 5 ошибок — туловище
        " +---+\n" +
        " |   |\n" +
        " O   |\n" +
        " |   |\n" +
        "     |\n" +
        "=====",

        // 6 ошибок — руки
        " +---+\n" +
        " |   |\n" +
        " O   |\n" +
        "/|\\  |\n" +
        "     |\n" +
        "=====",

        // 7 ошибок — ноги (полный человечек)
        " +---+\n" +
        " |   |\n" +
        " O   |\n" +
        "/|\\  |\n" +
        "/ \\  |\n" +
        "====="
    ];

    /// <summary>
    /// Печатает виселицу по количеству ошибок (обратная совместимость)
    /// Считаем, что «полная» виселица соответствует 7 ошибкам
    /// </summary>

    public static void Print(int errors)
    {
        Print(errors, Stages.Length - 1);
    }
    
    /// <summary>
    /// Печатает виселицу, масштабируя стадию относительно лимита ошибок
    /// При errors == maxErrors всегда покажем последний кадр
    /// </summary>
    public static void Print(int errors, int maxErrors)
    {
        // Защита от некорректных значений
        if (errors < 0)
        {
            errors = 0;
        }
        if (maxErrors < 1)
        {
            maxErrors = 1;
        }
        if (errors > maxErrors)
        {
            errors = maxErrors;
        }

        // Индекс кадра: равномерное распределение по всем стадиям
        // integer-math без Round, чтобы избежать неожиданностей
        int last = Stages.Length - 1;
        int idx = errors * last / maxErrors;
        
        if (idx < 0)
        {
            idx = 0;
        }

        if (idx > last)
        {
            idx = last;
        }

        Console.WriteLine(Stages[idx]);
    }
}