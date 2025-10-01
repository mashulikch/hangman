using System;
using Hangman;
using System.Text;

/// <summary>
/// Точка входа в игру
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        // Установка правильной кодировки для корректной работы с русскими символами в консоли
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Выбор режима запуска: интерактивный или неинтерактивный
        if (args.Length == 2)
        {
            TestInterface.Run(args[0], args[1]);
        }
        else
        {
            InteractiveInterface.Run();
        }
    }
}