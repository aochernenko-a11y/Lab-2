using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab2
{
    internal static class Program
    {
        private static readonly List<Product> Items = new List<Product>();

        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                ShowMenu();
                Console.Write("Ваш вибір: ");
                string choice = (Console.ReadLine() ?? "").Trim();

                if (choice == "0") break;

                switch (choice)
                {
                    case "1": AddItem(); break;
                    case "2": ListAll(); break;
                    case "3": FindItem(); break;
                    case "4": DemoBehavior(); break;
                    case "5": DeleteItem(); break;
                    default:
                        Warn("Невірний пункт меню.");
                        Pause();
                        break;
                }
            }
        }

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Практична робота (Lab-2): Інкапсуляція, властивості ===\n");
            Console.WriteLine("1 — Додати об'єкт");
            Console.WriteLine("2 — Переглянути всі об'єкти");
            Console.WriteLine("3 — Знайти об'єкт");
            Console.WriteLine("4 — Продемонструвати поведінку");
            Console.WriteLine("5 — Видалити об'єкт");
            Console.WriteLine("0 — Вийти з програми\n");
        }

        // ------- 1. Додати -------
        private static void AddItem()
        {
            var p = new Product();
            Console.WriteLine("Створення товару (всі значення через властивості):");

            try
            {
                p.Name = ReadString("Назва (2..60): ", 2, 60);
                p.Sku = ReadString("SKU (ABC-1234): ", 3, 20);
                p.Category = ReadOptional("Категорія (необов'язково): ", "Загальна");
                p.MinAge = ReadInt("Мін. вік (0..21): ", 0, 21);
                p.Price = ReadDecimal("Ціна (0..1 000 000), ₴: ", 0m, 1000000m);
                p.Quantity = ReadInt("Кількість (0..100000): ", 0, 100000);
                Items.Add(p);
                Ok("Додано: " + p.ShortInfo());
            }
            catch (Exception ex)
            {
                Error("Помилка встановлення властивості: " + ex.Message);
            }
            Pause();
        }

        // ------- 2. Переглянути всі -------
        private static void ListAll()
        {
            if (Items.Count == 0) { Warn("Список порожній."); Pause(); return; }
            Console.WriteLine("Збережені об'єкти:");
            for (int i = 0; i < Items.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + Items[i].ShortInfo() + " | Редагувалось: " + Items[i].TimesEdited + " раз(и)");
            }
            Pause();
        }

        // ------- 3. Знайти -------
        private static void FindItem()
        {
            if (Items.Count == 0) { Warn("Список порожній."); Pause(); return; }
            Console.Write("Введіть частину назви або SKU: ");
            string q = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            int found = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                Product p = Items[i];
                string a = p.Name.ToLowerInvariant();
                string b = p.Sku.ToLowerInvariant();
                if (a.IndexOf(q, StringComparison.Ordinal) >= 0 || b.IndexOf(q, StringComparison.Ordinal) >= 0)
                {
                    Console.WriteLine((i + 1) + ". " + p.ShortInfo());
                    found++;
                }
            }
            if (found == 0) Warn("Нічого не знайдено.");
            Pause();
        }

        // ------- 4. Продемонструвати поведінку -------
        private static void DemoBehavior()
        {
            if (Items.Count == 0) { Warn("Список порожній."); Pause(); return; }
            ListAllNoPause();

            int idx = ReadInt("Оберіть № для дії (1.." + Items.Count + "): ", 1, Items.Count) - 1;
            Product p = Items[idx];

            Console.WriteLine("\nДії:");
            Console.WriteLine("1 — Надати знижку %");
            Console.WriteLine("2 — Дозакупити (збільшити кількість)");
            Console.WriteLine("3 — Змінити ціну");
            Console.Write("Вибір: ");
            string act = (Console.ReadLine() ?? "").Trim();

            try
            {
                if (act == "1")
                {
                    int percent = ReadInt("Відсоток знижки (0..90): ", 0, 90);
                    p.ApplyDiscountPercent(percent);
                }
                else if (act == "2")
                {
                    int add = ReadInt("Скільки додати (>0): ", 1, 100000);
                    p.Restock(add);
                }
                else if (act == "3")
                {
                    decimal np = ReadDecimal("Нова ціна (0..1 000 000): ", 0m, 1000000m);
                    p.Price = np;
                }
                else
                {
                    Warn("Невідома дія.");
                }

                Ok("Поточний стан: " + p.ShortInfo());
            }
            catch (Exception ex)
            {
                Error("Помилка: " + ex.Message);
            }
            Pause();
        }

        // ------- 5. Видалити -------
        private static void DeleteItem()
        {
            if (Items.Count == 0) { Warn("Список порожній."); Pause(); return; }
            ListAllNoPause();
            int idx = ReadInt("Вкажіть № для видалення (1.." + Items.Count + "): ", 1, Items.Count) - 1;
            string name = Items[idx].Name;
            Items.RemoveAt(idx);
            Ok("Видалено: " + name);
            Pause();
        }

        // ------- допоміжні введення (усі значення через властивості у викликах set) -------
        private static string ReadString(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "");
                s = s.Trim();
                if (s.Length >= min && s.Length <= max) return s;
                Error("Довжина має бути " + min + ".." + max + ".");
            }
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string raw = (Console.ReadLine() ?? "").Trim();
                int v;
                if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
                {
                    Error("Очікується ціле число.");
                    continue;
                }
                if (v < min || v > max)
                {
                    Error("Допустимо " + min + ".." + max + ".");
                    continue;
                }
                return v;
            }
        }

        private static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(prompt);
                string raw = (Console.ReadLine() ?? "").Trim();
                decimal v;
                if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
                {
                    Error("Очікується число.");
                    continue;
                }
                if (v < min || v > max)
                {
                    Error("Допустимо " + min + ".." + max + ".");
                    continue;
                }
                return v;
            }
        }

        private static string ReadOptional(string prompt, string fallback)
        {
            Console.Write(prompt);
            string s = (Console.ReadLine() ?? "");
            s = s.Trim();
            return s.Length == 0 ? fallback : s;
        }

        // ------- утиліти виводу -------
        private static void Ok(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nНатисніть Enter, щоб продовжити...");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void ListAllNoPause()
        {
            Console.WriteLine("Поточні об'єкти:");
            for (int i = 0; i < Items.Count; i++)
                Console.WriteLine((i + 1) + ". " + Items[i].ShortInfo());
        }
    }
}
