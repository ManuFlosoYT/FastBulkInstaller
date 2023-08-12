using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FastBulkInstaller
{
    public class Installer
    {
        public static void ReadFile()
        {
            IEnumerable<string> lines = File.ReadLines(FBI.path);

            List<string> list = new List<string>(lines);

            list.RemoveAt(0);

            Console.ForegroundColor = ConsoleColor.White;

            foreach (AppInfo appInfo in FBI.database.DB)
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

            if (!FBI.isSilent)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Do you want to wipe config.yaml? (y/n) (invalid inputs will be considered as 'n'): ");
                char response = Console.ReadKey().KeyChar;
                if (response == 'y' || response == 'Y')
                {
                    File.Delete(FBI.path);
                    FBI.CreateConfigFile();
                }
                Console.WriteLine();
                Console.WriteLine("Instalation finished, press any key to return . . .");
                Console.ReadKey();
                UI.MainMenu();
            }  
        }

        public static async Task Download(string url, string fileName)
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

        public static void PowerShell(string powerShellCommand)
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
        
        public static void DeleteFiles(string path)
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
    }
}
