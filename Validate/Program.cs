using System;
using System.Diagnostics;
using PowerArgs;

namespace Validate
{
    /// <summary>
    /// Usage
    ///
    ///   Validate.exe
    /// 
    ///     -xml        Full path to xml file to validate.
    ///                 If -schema is given, that schema file is used. Otherwise an inline (schemaLocation) is searched.
    ///                 If -schematron is given, that xslt file is used to do schematron validation.
    ///
    ///     -schema     Full path to .xsd file to validate.
    ///                 If used in combination with -xml supplied schema file will be used for schema valiation of supplied xml file.
    ///
    ///     -schematron Full path to .xslt file to validate.
    ///                 If used in combination with -xml supplied schematron file will be used for schematron valiation of supplied xml file.
    ///
    /// Examples:
    ///     Validate.exe -xml C:/Path/to/file.xml [-schema C:/Path/to/schema.xsd] [-schematron C:/Path/to/schematron.xslt]
    ///     Validate.exe -schema C:/Path/to/schema.xsd
    ///     Validate.exe -schematron C:/Path/to/schematron.xslt
    /// </summary>
    static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine();
            ColorConsole.Write("*******************************  ", ConsoleColor.DarkCyan);
            ColorConsole.Write("VALIDATE", ConsoleColor.Cyan);
            ColorConsole.WriteLine("  *******************************", ConsoleColor.DarkCyan);
            Console.WriteLine();

            Args.InvokeAction<ValidationProgram>(args);

            Console.WriteLine();
            ColorConsole.WriteLine("**************************************************************************", ConsoleColor.DarkCyan);

            if (Debugger.IsAttached)
                Console.ReadLine();
        }
    }
}
