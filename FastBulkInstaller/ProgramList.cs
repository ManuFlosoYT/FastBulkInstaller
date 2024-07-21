using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FastBulkInstaller
{
    public class ProgramList
    {
        public static void List()
        {
            IEnumerable<string> lines = File.ReadLines(FBI.path);
            List<string> list = new List<string>(lines);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Database: {list[3].Replace("\"", "")}");
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
            foreach (AppInfo appInfo in FBI.database.DB)
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
            UI.MainMenu();
        }
    }
}
