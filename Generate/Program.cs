using System;
using System.Diagnostics;
using PowerArgs;

namespace Generate
{
    /// <summary>
    /// Usage
    ///
    ///   merge           Normalize (merge/concatenate) .xsd (schema) file that includes other .xsd files into one single file.
    ///     -schema       Full path to .xsd file (mandatory)
    ///     -out          Full path to output .xsd file (optional)
    /// 
    ///   schema         Create schema (xsd) from a xml-file.
    ///     -in           Full path to .xml file (mandatory)
    ///     -out          Full path to output .xsd file (optional)
    ///
    ///   styled         Create styled output from a .xml file using a stylesheet (xslt)
    ///     -in           Full path to .xml file (mandatory)
    ///     -using        Full path to .xslt file (stylesheet) (mandatory)
    ///     -out          Full path to output file (optional)
    ///
    ///   schematron     Create schematron file (xslt) from schematron rules. Rules can be read in a .sch (schematron) file, or inlined insied a .xsd (xml schema) file.
    ///     -in           Full path to file containing schematron rules. Can be .sch-file or .xsd with inline rules.
    ///     -out          Full path to output file (optional)
    ///     -v|-version   Xslt version to use. 1 or 2. Default is 2.
    ///
    ///
    /// Examples:
    ///     Generate.exe merge -in C:/Path/to/file.xsd [-out C:/Path/to/output.xsd]
    ///     Generate.exe -schema -in C:/Path/to/file.xml [-out C:/Path/to/output.xyz]
    ///     Generate.exe -styled -in C:/Path/to/input.xml -using C:/Path/to/stylesheet.xslt [-out C:/Path/to/file.html]
    ///     Generate.exe -schematron -in C:/Path/to/input.sch [-out C:/Path/to/file.xslt] [-v 2]
    ///     Generate.exe -schematron -in C:/Path/to/input.xsd [-out C:/Path/to/file.xslt] [-v 1]
    ///
    /// </summary>
    static class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeAction<GenerationProgram>(args);
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Klar");
                Console.ReadLine();
            }
        }
    }
}
