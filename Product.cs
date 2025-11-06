using System;
using System.Globalization;

namespace Lab2
{

    public class Product
    {
        private string name = "Без назви";
        private string sku = "SKU-0000";
        private int quantity = 1;
        private decimal price = 0m;
        private int minAge = 0;

        public string Category { get; set; } = "Загальна";

        public string Name
        {
            get { return name; }
            set
            {
                string v = RequireString(value, 2, 60, "Name");
                v = NormalizeName(v);
                NotifyChange("Name", name, v);
                name = v;
                TouchEdited();
            }
        }

        public string Sku
        {
            get { return sku; }
            set
            {
                string v = RequireString(value, 3, 20, "Sku");
                if (!IsSku(v)) throw new ArgumentException("Sku: очікується шаблон типу ABC-1234.");
                NotifyChange("Sku", sku, v);
                sku = v.ToUpperInvariant();
                TouchEdited();
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                int v = RequireRange(value, 0, 100000, "Quantity");
                NotifyChange("Quantity", quantity.ToString(CultureInfo.InvariantCulture), v.ToString(CultureInfo.InvariantCulture));
                quantity = v;
                TouchEdited();
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                decimal v = RequireRange(value, 0m, 1000000m, "Price");
                NotifyChange("Price",
                    price.ToString("0.00", CultureInfo.InvariantCulture),
                    v.ToString("0.00", CultureInfo.InvariantCulture));
                price = Math.Round(v, 2, MidpointRounding.AwayFromZero);
                TouchEdited();
            }
        }

        public int MinAge
        {
            get { return minAge; }
            set
            {
                int v = RequireRange(value, 0, 21, "MinAge");
                NotifyChange("MinAge", minAge.ToString(CultureInfo.InvariantCulture), v.ToString(CultureInfo.InvariantCulture));
                minAge = v;
                TouchEdited();
            }
        }

        public decimal Total => price * quantity;

        public int TimesEdited { get; private set; } = 0;

        public Guid Id { get; } = Guid.NewGuid();

        public void ApplyDiscountPercent(int percent)
        {
            if (percent < 0 || percent > 90)
                throw new ArgumentOutOfRangeException(nameof(percent), "Відсоток знижки має бути 0..90.");
            Price = Math.Round(Price * (100 - percent) / 100m, 2, MidpointRounding.AwayFromZero);
        }

        public void Restock(int add)
        {
            if (add <= 0) throw new ArgumentOutOfRangeException(nameof(add), "Кількість повинна бути > 0.");
            Quantity = checked(Quantity + add);
        }

        public string ShortInfo()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[{0}] {1} ({2}) | {3} шт × {4:0.00}₴ = {5:0.00}₴ | Мін.вік {6}+ | Категорія: {7}",
                Id, Name, Sku, Quantity, Price, Total, MinAge, Category);
        }

        private static string RequireString(string value, int min, int max, string prop)
        {
            if (value == null) throw new ArgumentNullException(prop);
            string v = value.Trim();
            if (v.Length < min || v.Length > max)
                throw new ArgumentOutOfRangeException(prop, prop + ": довжина " + min + ".." + max + " символів.");
            return v;
        }

        private static int RequireRange(int value, int min, int max, string prop)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(prop, prop + ": допустимо " + min + ".." + max + ".");
            return value;
        }

        private static decimal RequireRange(decimal value, decimal min, decimal max, string prop)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(prop, prop + ": допустимо " + min + ".." + max + ".");
            return value;
        }

        private static bool IsSku(string s)
        {
            if (s == null) return false;
            s = s.Trim();
            if (s.Length < 5) return false;
            int dash = s.IndexOf('-');
            if (dash <= 0 || dash >= s.Length - 1) return false;
            string left = s.Substring(0, dash);
            string right = s.Substring(dash + 1);
            for (int i = 0; i < left.Length; i++)
            {
                char c = left[i];
                if (!char.IsLetter(c)) return false;
            }
            for (int i = 0; i < right.Length; i++)
            {
                char c = right[i];
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        private static string NormalizeName(string v)
        {
            if (v.Length == 0) return v;
            return char.ToUpperInvariant(v[0]) + (v.Length > 1 ? v.Substring(1) : "");
        }

        private static void NotifyChange(string prop, string oldVal, string newVal)
        {
            if (oldVal == newVal) return;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[i] " + prop + ": \"" + oldVal + "\" → \"" + newVal + "\"");
            Console.ResetColor();
        }

        private void TouchEdited()
        {
            TimesEdited++;
        }
    }
}
