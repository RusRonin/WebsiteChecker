using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public class UserInterface : ICommandUserInterface
    {
        public UserInterface()
        { }

        public void PrintCommandList()
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

        public void PrintString(string text, bool withWrap)
        {
            string outputText = text + (withWrap ? "\n" : "");
            Console.Write(outputText);
        }

        public void PrintSites(ISiteList sites)
        {
            PrintString("Список сайтов: ", true);
            foreach (Site site in sites)
            {
                string output = String.Format("- {0}", site.Address);
                PrintString(output, true);
            }
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void ReadCommand(ISiteList sites, ref bool endWork)
        {
            Site site;
            string command = ReadLine();
            switch (command.ToLower())
            {
                case "добавить":
                    site = GetNewSiteFromUser();
                    if (site != null)
                    {
                        try
                        {
                            sites.AddSite(site);
                        }
                        catch (SiteCheckingFrequencyException e)
                        {
                            PrintString($"Ошибка добавления сайта: {e.Message}", true);
                        }
                        catch (Exception e)
                        {
                            PrintString($"Ошибка добавления сайта: {e.Message}", true);
                        }
                    }
                    break;
                case "удалить":
                    site = GetExcistingSiteFromUser();
                    sites.RemoveSite(site);
                    break;
                case "сохранить":
                    try
                    {
                        sites.CommitChanges();
                    }
                    catch (DatabaseFileNotFoundException e)
                    {
                        PrintString($"Ошибка сохранения данных: {e.Message}", true);
                    }
                    catch (DatabaseException e)
                    {
                        PrintString($"Ошибка сохранения данных: {e.Message}", true);
                    }
                    catch (DatabaseNullArgumentException e)
                    {
                        PrintString($"Ошибка сохранения данных: {e.Message}", true);
                    }
                    catch (Exception e)
                    {
                        PrintString($"Ошибка сохранения данных: {e.Message}", true);
                    }
                    break;
                case "сброс":
                    try
                    {
                        sites.ReloadSites();
                    }
                    catch (DatabaseFileNotFoundException e)
                    {
                        PrintString($"Ошибка загрузки данных: {e.Message}", true);
                    }
                    catch (DatabaseException e)
                    {
                        PrintString($"Ошибка загрузки данных: {e.Message}", true);
                    }
                    catch (DatabaseNullArgumentException e)
                    {
                        PrintString($"Ошибка сохранения данных: {e.Message}", true);
                    }
                    catch (Exception e)
                    {
                        PrintString($"Ошибка загрузки данных: {e.Message}", true);
                    }
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

        private Site GetNewSiteFromUser()
        {
            PrintString("Введите адрес сайта: ", false);
            string address = ReadLine();
            PrintString("Введите частоту проверки сайта в милллисекундах (от 1 до 60000)", false);
            string stringFrequency = ReadLine();
            uint frequency;
            if (UInt32.TryParse(stringFrequency, out frequency))
            {
                Site site = new Site(address, frequency);
                return site;
            }
            else
            {
                PrintString("Некорректное значение частоты проверки", true);
                return null;
            }
        }

        private Site GetExcistingSiteFromUser()
        {
            PrintString("Введите адрес сайта: ", false);
            string address = ReadLine();
            //так как сравнение сайтов происходит только по адресу, указывается любая допустимая частота проверки
            Site site = new Site(address, 1);
            return site;
        }
    }
}