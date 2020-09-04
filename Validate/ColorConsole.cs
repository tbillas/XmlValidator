using System;

namespace Validate
{
    public static class ColorConsole
    {
        private static ConsoleColor SetColor(ConsoleColor color)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            return current;
        }

        public static void Write(string format, ConsoleColor color)
        {
            var previous = SetColor(color);
            Console.Write(format);
            SetColor(previous);
        }
        public static void WriteLine(string format, ConsoleColor color)
        {
            var current = SetColor(color);
            Console.WriteLine(format);
            SetColor(current);
        }


        public static void Ok()
        {
            Ok("Ok");
        }
        public static void Ok(string message)
        {
            WriteLine(message, ConsoleColor.DarkGreen);
        }

        public static void Warning(string message)
        {
            WriteLine(message, ConsoleColor.DarkYellow);
        }

        public static void Error(string message)
        {
            WriteLine(message, ConsoleColor.DarkRed);
        }
    }
}