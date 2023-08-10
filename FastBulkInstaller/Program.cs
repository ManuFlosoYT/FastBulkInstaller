using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastBulkInstaller
{
    internal class FBI
    {
        static void Main()
        {
            string fileName = "config.yaml";
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            Dictionary<string, Tuple<string, string, string, string, bool, string>> programdictionary = new Dictionary<string, Tuple<string, string, string, string, bool, string>>();
            ProgramDiccionary(programdictionary);
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄▄ \r\n█       █  ▄    █   █\r\n█    ▄▄▄█ █▄█   █   █\r\n█   █▄▄▄█       █   █\r\n█    ▄▄▄█  ▄   ██   █\r\n█   █   █ █▄█   █   █\r\n█▄▄▄█   █▄▄▄▄▄▄▄█▄▄▄█\r\n");
            Console.ForegroundColor = ConsoleColor.White;

            if (!File.Exists(path))
            {
                CreateFile(path);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            DisplayMenu();
            Console.ForegroundColor = ConsoleColor.White;

            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                if (choice < 1 || choice > 4)
                {
                    Main();
                }
            }
            else
            {
                Main();
            }


            switch (choice)
            {
                case 1:
                    ListaProgramas(programdictionary);
                    break;
                case 2:
                    ReadFile(path, programdictionary);
                    break;
                case 3:
                    //string url = "https://www.example.com";
                    //Process.Start(url);
                    
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to exit . . .");
            Console.ReadKey();
        }

        static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. List the available apps");
            Console.WriteLine("2. Install apps (WIP, just downloads the installers for now)");
            Console.WriteLine("3. Proyect GitHub");
            Console.WriteLine("4. Exit");
        }

        static void CreateFile(string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("#Add one program ID per line without separators or spaces, DO NOT EDIT THIS FIRST LINE as this line is ignored in code");
            }
        }

        static void ReadFile(string path, Dictionary<string, Tuple<string, string, string, string, bool, string>> programDictionary)
        {
            IEnumerable<string> lines = File.ReadLines(path);

            List<string> list = new List<string>(lines);

            list.RemoveAt(0);

            Console.ForegroundColor = ConsoleColor.White;

            //Searches for an item from config.yaml on the dictionary
            foreach (var kvp in programDictionary)
            {
                if (list.Contains(kvp.Key))
                {
                    Task.Run(async () => await Download(kvp.Value.Item2)).Wait();
                    
                    if (kvp.Value.Item5)
                    {
                        PowerShell(kvp.Value.Item6);
                    }
                }
            }
            Wait();
        }

        static void CMD(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C " + command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();

            StreamReader streamReader = process.StandardOutput;

            StreamReader errorReader = process.StandardError;
            string errors = errorReader.ReadToEnd();
            Console.WriteLine(errors);

            string output = streamReader.ReadToEnd();
            Console.WriteLine(output);

            streamReader.Close();
            errorReader.Close();
            process.WaitForExit();
            process.Close();

        }

        static void PowerShell(string powerShellCommand)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -Command \"{powerShellCommand}\""
            };

            Process process = new Process { StartInfo = psi };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine("Output:");
            Console.WriteLine(output);

            Console.WriteLine("Error:");
            Console.WriteLine(error);
        }

        static void Wait()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to continue . . .");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }

        static void ListaProgramas(Dictionary<string, Tuple<string, string, string, string, bool, string>> programDictionary)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Codename | Program Name | Available Version | Instalable Silently");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            foreach (var kvp in programDictionary)
            {
                Console.WriteLine($"{kvp.Key} | {kvp.Value.Item1} | {kvp.Value.Item3} | { kvp.Value.Item5}");
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to return . . .");
            Console.ReadKey();
            Main();
        }

        static void ProgramDiccionary(Dictionary<string, Tuple<string, string, string, string, bool, string>> programDictionary)
        {
            // CODENAME,
            // URL , VERSION , HASH , INSTALABLE SILENTLY, INSTALL
            programDictionary.Add("np++", new Tuple<string, string, string, string, bool, string>
            (
                "Notepad ++",
                "https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v8.5.5/npp.8.5.5.Installer.x64.exe",
                "8.5.5",
                "260ba9ebf2932419604d723f7381d13fa1fa83745b58a93ce4d460cde15022bd",
                true,
                "start-process -FilePath npp.8.5.5.Installer.x64.exe -ArgumentList '/S' -Verb runas -Wait"
            ));

            programDictionary.Add("vlc", new Tuple<string, string, string, string, bool, string>
            (
                "VLC media player",
                "https://mirrors.netix.net/vlc/vlc/3.0.18/win64/vlc-3.0.18-win64.exe",
                "3.0.18",
                "ba575f153d357eaf3fdbf446b9b93a12ced87c35887cdd83ad4281733eb86602",
                true,
                "start-process -FilePath vlc-3.0.18-win64.exe -ArgumentList '/L=1033 /S /NCRC' -Verb runas -Wait"
            ));
        }

        static async Task Download(string url)
        {
            string fileName = Path.GetFileName(url);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                            {
                                using (FileStream fileStream = File.Create(filePath))
                                {
                                    await contentStream.CopyToAsync(fileStream);
                                    Console.WriteLine($"File downloaded and saved to: {filePath}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to download the file. Status code: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}