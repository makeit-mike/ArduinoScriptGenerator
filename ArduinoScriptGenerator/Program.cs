using System;

namespace ArduinoScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Welcome to the AVRDUDE script generator.\r\nMy only experience with Arduinos is with the Leonardo flashing Keyboard firmware using a USB Host Shield. Some tinkering may be needed.");

                bool shouldContinue = true;
                while (shouldContinue)
                {
                    BuildScript();

                    Console.WriteLine("Do you want to continue? (Y/n)");
                    shouldContinue = Console.ReadLine() != "n";
                }
            }
            catch (Exception ex) {ErrorCatch(ex);}
        }
        private static void BuildScript()
        {
            try
            {
                string filePath = GetFilePath();
                int indexOfHexFile = filePath.LastIndexOf('\\'); //"C:\someFolders\unimap.hex" where \ before unimap is the last index before the hex file.
                var pathParts = new[]
                {
                    filePath.Substring(0, indexOfHexFile),
                    filePath.Substring(indexOfHexFile)
                };

                //Looks like: "C:\someFolders"
                Console.WriteLine($"cd {pathParts[0]}");
                //Looks like: "avrdude -patmega32u4 -cavr109 -b57600 -Uflash:w:unimap.hex -PCOM17" //Will need to manually set your port number
                Console.WriteLine($"avrdude -patmega32u4 -cavr109 -b57600 -Uflash:w:{pathParts[1]} -PCOM17");

            }
            catch (Exception ex) { ErrorCatch(ex); }
        }
        private static void ErrorCatch(Exception ex)
        {
            Console.WriteLine(ex.Message);

            Console.WriteLine($"Would you like to see verbose error details? (Y/n)");
            if (Console.ReadLine() == "Y")
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Paste path to HEX file.");

            var filePath = Console.ReadLine();
            Console.WriteLine($"Is this path correct (Y/n): {filePath}");

            bool isCorrectPath = Console.ReadLine() == "Y";

            if (isCorrectPath && filePath.Length > 0)
            {
                return filePath.Trim().Replace("\"","");
            }
            else
            {
                Console.WriteLine();
                filePath = GetFilePath();
                return filePath;
            }
        }
    }
}
