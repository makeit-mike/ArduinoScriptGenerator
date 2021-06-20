using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Management;
using System.Management.Automation;
using System.Timers;

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

                    Console.WriteLine("\r\nDo you want to continue? (Y/n)");
                    shouldContinue = Console.ReadLine() != "n";
                }
            }
            catch (Exception ex) { ErrorCatch(ex); }
        }
        private static void BuildScript()
        {
            try
            {
                string filePath = GetFilePath();
                int indexOfHexFile = filePath.LastIndexOf('\\'); //"C:\someFolders\unimap.hex" where \ before unimap is the last index before the hex file.
                
                //var pathParts = new[]     //Use if you want to split up the location of the HEX and the actual HEX file.
                //{
                //    filePath.Substring(0, indexOfHexFile),
                //    filePath.Substring(indexOfHexFile)
                //};

                string comPort = DetermineCOMPort();

                Console.WriteLine("\r\nCommand-Line Script Generated.\r\nCopy and paste into a terminal of your choice.\r\nRun the AVRDude script with Bootloader mode ON.\r\n");

                //Console.WriteLine($"cd {pathParts[0]}"); //Looks like: "C:\someFolders"
                //Console.WriteLine($"avrdude -patmega32u4 -cavr109 -b57600 -Uflash:w:{pathParts[1]} -P{comPort}"); //Looks like: "avrdude -patmega32u4 -cavr109 -b57600 -Uflash:w:unimap.hex -PCOM17" 

                string powerShellScript = $"avrdude -patmega32u4 -cavr109 -b57600 -Uflash:w:\"{filePath}\":a -P{comPort}";
                Console.WriteLine(powerShellScript);

                Console.WriteLine("Would you like to flash file now? (Y/n)");
                bool shouldAttemptFlash = Console.ReadLine() == "Y";
                if (shouldAttemptFlash)
                {
                    RunPowershellScript(powerShellScript);
                }
            }
            catch (Exception ex) { ErrorCatch(ex); }
        }

        private static string DetermineCOMPort()
        {
            Console.WriteLine("\r\nWould you like to search for an Arduino COM Port? (Y/n)");
            string comPort = "";

            if (Console.ReadLine() == "Y")
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                Console.WriteLine("Activate Arduino Bootloader. . .");
                int numSecondsToSearchForComPort = 5;
                while (comPort.Length == 0 && stopWatch.ElapsedMilliseconds < numSecondsToSearchForComPort * 1000)
                {
                    comPort = AutodetectArduinoPort();
                }
                stopWatch.Stop();
            }
            if (comPort.Length == 0)
            {
                comPort = "COM";
                Console.WriteLine("\r\nCOM Port not found. You will need to manually set your port number. Can be found in the Device Manager or in the Arduino IDE.\r\n");
            }

            return comPort;
        }
        private static void RunPowershellScript(string powerShellScript)
        {
            try
            {
                using (PowerShell powerShell = PowerShell.Create())
                {
                    // Source functions.
                    powerShell.AddScript(powerShellScript);

                    // invoke execution on the pipeline (collecting output)
                    Console.WriteLine("Invoking script in PowerShell");
                    Collection<PSObject> PSOutput = powerShell.Invoke();

                    // loop through each output object item
                    foreach (PSObject outputItem in PSOutput)
                        if (outputItem != null) // if null object was dumped to the pipeline during the script then a null object may be present here
                            Console.WriteLine($"$ [{outputItem}]");

                    if (powerShell.Streams.Error.Count > 0) // check the other output streams (for example, the error stream)
                        foreach (var err in powerShell.Streams.Error)
                            Console.WriteLine($"$ [{err.Exception.Message}]");
                }
            }
            catch (Exception ex) { ErrorCatch(ex); }
        }

        private static void ErrorCatch(Exception ex)
        {
            Console.WriteLine(ex.Message);

            Console.WriteLine($"\r\nWould you like to see verbose error details? (Y/n)");
            if (Console.ReadLine() == "Y")
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Paste path to HEX file.");

            var filePath = Console.ReadLine();
            Console.WriteLine($"\r\nIs this path correct (Y/n): {filePath}");

            bool isCorrectPath = Console.ReadLine() == "Y";

            if (isCorrectPath && filePath.Length > 0)
            {
                return filePath.Trim().Replace("\"", "");
            }
            else
            {
                Console.WriteLine();
                filePath = GetFilePath();
                return filePath;
            }
        }

        private static string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        Console.WriteLine($"\r\nArduino COM Port Found at {deviceId}!\r\n");
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e) { }
            return "";
        }
    }
}

class USBDeviceInfo
{
    public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
    {
        this.DeviceID = deviceID;
        this.PnpDeviceID = pnpDeviceID;
        this.Description = description;
    }
    public string DeviceID { get; private set; }
    public string PnpDeviceID { get; private set; }
    public string Description { get; private set; }
}
