using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteChecker
{
    internal static class UserInterface
    {
        internal static void PrintCommandList()
        {
            PrintString("Введите команду из следующего списка: ", true);
            PrintString("- Добавить :для добавления сайта в список", true);
            PrintString("- Удалить :для удаления сайта из списка", true);
            PrintString("- Сохранить :для сохранения внесенных изменений", true);
            PrintString("- Сброс :для сброса изменений (отката к последнему сохранению)", true);
            PrintString("- Печать :для вывода списка сайтов", true);
            PrintString("- Выключение :для завершения работы с программой", true);
            PrintString("- Справка :для повторного вывода списка команд", true);
        }

        internal static void PrintString(string text, bool withWrap)
        {
            string outputText = text + (withWrap ? "\n" : "");
            Console.Write(outputText);
        }

        internal static void PrintSites(List<Site> sites)
        {
            PrintString("Список сайтов: ", true);
            foreach (Site site in sites)
            {
                string output = String.Format("- {0}", site.Address);
                PrintString(output, true);
            }
        }

        internal static void ReadCommand(ref List<Site> sites, ref bool endWork)
        {
            string command = Console.ReadLine();
            switch(command.ToLower())
            {
                case "добавить":
                    SiteListChanger.AddSite(ref sites);
                    break;
                case "удалить":
                    SiteListChanger.RemoveSite(ref sites);
                    break;
                case "сохранить":
                    SiteListChanger.CommitChanges(sites);
                    break;
                case "сброс":
                    sites = SiteListChanger.LoadSites();
                    break;
                case "печать":
                    PrintSites(sites);
                    break;
                case "выключение":
                    endWork = true;
                    break;
                case "справка":
                    PrintCommandList();
                    break;
                default:
                    PrintString("Неизвестная команда. Проверьте правильность ввода или введите \"Справка\" для просмотра списка команд", true);
                    break;
            }
        }

        
}
}
