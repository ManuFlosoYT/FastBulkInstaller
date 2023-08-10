using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FastBulkInstaller
{
    internal class FBI
    {
        public static void Main()
        {
            string fileName = "config.yaml";
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (!File.Exists(path))
            {
                CreateFile(path);
            }

            Console.WriteLine("Fetching program Database from the web");
            string jsc = "";
            Task.Run(async () =>
            {
                jsc = await FetchDatabase();
            }).Wait();

            Database database = JsonConvert.DeserializeObject<Database>(jsc);

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄▄ \r\n█       █  ▄    █   █\r\n█    ▄▄▄█ █▄█   █   █\r\n█   █▄▄▄█       █   █\r\n█    ▄▄▄█  ▄   ██   █\r\n█   █   █ █▄█   █   █\r\n█▄▄▄█   █▄▄▄▄▄▄▄█▄▄▄█\r\n");
            Console.ForegroundColor = ConsoleColor.White;


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
                    ListaProgramas(database);
                    break;
                case 2:
                    ReadFile(path, database);
                    break;
                case 3:
                    string url = "https://github.com/ManuFlosoYT/FastBulkInstaller";
                    Process.Start(url);
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to exit . . .");
            Console.ReadKey();
        }

        static async Task<string> FetchDatabase()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://pastebin.com/raw/XkgNYRTL");

                    if (response.IsSuccessStatusCode)
                    {
                        string rawText = await response.Content.ReadAsStringAsync();
                        return rawText;  
                    }
                    else
                    {
                        Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                return null;
            }
        }

        static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. List the available apps");
            Console.WriteLine("2. Install apps");
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

        static void ReadFile(string path, Database database)
        {
            IEnumerable<string> lines = File.ReadLines(path);

            List<string> list = new List<string>(lines);

            list.RemoveAt(0);

            Console.ForegroundColor = ConsoleColor.White;

            foreach (AppInfo appInfo in database.DB)
            {
                if (list.Contains(appInfo.Codename))
                {
                    if (Directory.Exists(appInfo.InstallRoute))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{appInfo.Name} is already installed! Skipping instalation");
                        continue;
                    }
                    Download(appInfo.Url, appInfo.FileName).Wait();
                    if (appInfo.InstalableSilently)
                    {
                        PowerShell(appInfo.InstallCommand);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{appInfo.Name} installed succesfully!");
                        DeleteFiles(Directory.GetCurrentDirectory());
                    }
                }
            }
        }

        static void DeleteFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string excludedFileName = "FastBulkInstaller.exe";
                string[] exeFiles = Directory.GetFiles(path, "*.exe");

                foreach (string exeFile in exeFiles)
                {
                    if (!Path.GetFileName(exeFile).Equals(excludedFileName))
                    {
                        try
                        {
                            if (File.Exists(exeFile))
                            {
                                File.Delete(exeFile);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Deleted file: {exeFile}");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"File does not exist: {exeFile}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error deleting file {exeFile}: {ex.Message}");
                        }
                    }
                }
            }
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
            process.WaitForExit();
        }

        static void ListaProgramas(Database database)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Codename");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Program Name");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Available Version");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Instalable Silently");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();

            Console.WriteLine();
            foreach (AppInfo appInfo in database.DB)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(appInfo.Codename);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" | ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(appInfo.Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" | ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(appInfo.Version);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" | ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(appInfo.InstalableSilently);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to return . . .");
            Console.ReadKey();
            Main();
        }

        static async Task Download(string url, string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Started downloading {fileName} to {filePath}");

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
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"File downloaded and saved to: {filePath}");
                                }
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Failed to download the file. Status code: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }

    public class AppInfo
    {
        public string Codename { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
        public string Hash { get; set; }
        public bool InstalableSilently { get; set; }
        public string InstallCommand { get; set; }
        public string InstallRoute { get; set; }
        public string FileName { get; set; }
    }

    public class Database
    {
        public List<AppInfo> DB { get; set; }
    }
}