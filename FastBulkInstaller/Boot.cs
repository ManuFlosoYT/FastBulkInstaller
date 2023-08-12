using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Security.Principal;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FastBulkInstaller
{
    public class FBI
    {
        public static string path = Path.Combine(Directory.GetCurrentDirectory(), "config.yaml");
        public static bool isSilent = false;
        public static Database database;
        public static bool isAdmin = false;

        public static void Main(string[] args)
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (args.Length != 0)
            {
                if (args[0] == "/S" || args[0] == "/s")
                {
                    if (isAdmin)
                    {
                        isSilent = true;
                    }
                    else
                    {
                        Console.WriteLine("Administrator permissions requiered!");
                        Console.WriteLine("Press any key to exit . . .");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                }
            }  

            Console.WriteLine("Fetching program Database from the web");
            string jsc = "";
            Task.Run(async () =>
            {
                jsc = await FetchDatabase.FetchDB();
            }).Wait();
            database = JsonConvert.DeserializeObject<Database>(jsc);

            CreateConfigFile();

            if (!isSilent)
            {
                UI.MainMenu();
            }
            else
            {
                Installer.ReadFile();
            }
        }

        public static void AskAdmin()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "FastBulkInstaller.exe",
                Verb = "runas"
            };

            try
            {
                Process.Start(psi);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static void CreateConfigFile()
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("#Add one program ID per line without separators or spaces, DO NOT EDIT THIS FIRST LINE as this line is ignored in code");
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