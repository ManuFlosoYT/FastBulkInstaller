using System;
using System.Diagnostics;
using System.Security.Principal;

namespace FastBulkInstaller
{
    public class UI
    {
        public static void MainMenu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄▄ \r\n█       █  ▄    █   █\r\n█    ▄▄▄█ █▄█   █   █\r\n█   █▄▄▄█       █   █\r\n█    ▄▄▄█  ▄   ██   █\r\n█   █   █ █▄█   █   █\r\n█▄▄▄█   █▄▄▄▄▄▄▄█▄▄▄█\r\n");
            Console.ForegroundColor = ConsoleColor.White;


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("1. List the available apps");
            if (FBI.isAdmin)
            {
                Console.WriteLine("2. Install apps");
            }
            else
            {
                Console.WriteLine("2. Install apps (Administrator permissions requiered)");
            } 
            Console.WriteLine("3. Proyect GitHub");
            Console.WriteLine("4. Exit");
            Console.ForegroundColor = ConsoleColor.White;


            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                if (choice < 1 || choice > 4)
                {
                    MainMenu();
                }
            }
            else
            {
                MainMenu();
            }


            switch (choice)
            {
                case 1:
                    ProgramList.List();
                    break;
                case 2:
                    if (FBI.isAdmin)
                    {
                        Installer.ReadFile();
                    }
                    else
                    {
                        FBI.AskAdmin();
                    }
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
        }
    }
}
