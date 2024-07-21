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
        public static string path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
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
            CreateConfigFile();

            Console.WriteLine("Fetching program Database");
            string jsc = "";
            Task.Run(async () =>
            {
                jsc = await FetchDatabase.FetchDB();
            }).Wait();
            database = JsonConvert.DeserializeObject<Database>(jsc);

            
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
                    sw.WriteLine("CONFIG FILE V1.2");
                    sw.WriteLine("Add the url or local file path with \"\" at the start and end of the string of the database on the line 4, the default one is https://pastebin.com/raw/XkgNYRTL");
                    sw.WriteLine("Starting on line 5 add one program ID per line without separators or spaces");
                    sw.WriteLine("https://pastebin.com/raw/XkgNYRTL");
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