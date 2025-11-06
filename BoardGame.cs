using System;
using System.Globalization;

namespace Lab_2
{

    public sealed class BoardGame
    {
        private string _title = "Без назви";
        private string _genre = "Family";
        private int _minPlayers = 1;
        private int _maxPlayers = 4;
        private int _durationMinutes = 30;
        private int _minAge = 8;
        private decimal _price = 0m;

        public string? Publisher { get; set; } = null;

        public bool IsPartyGame => _maxPlayers >= 6 && _durationMinutes <= 45;

        public int TimesPlayed { get; private set; } = 0;

        public Guid Id { get; } = Guid.NewGuid();

        public string Title
        {
            get => _title;
            set
            {
                var newVal = RequireString(value, 2, 60, nameof(Title));
                NotifyIfChanged(nameof(Title), _title, newVal);
                _title = newVal;
            }
        }

        public string Genre
        {
            get => _genre;
            set
            {
                var newVal = RequireString(value, 2, 40, nameof(Genre));
                NotifyIfChanged(nameof(Genre), _genre, newVal);
                _genre = newVal;
            }
        }

        public int MinPlayers
        {
            get => _minPlayers;
            set
            {
                var newVal = RequireRange(value, 1, 20, nameof(MinPlayers));
                if (newVal > _maxPlayers)
                    throw new ArgumentOutOfRangeException(nameof(MinPlayers), "MinPlayers не може бути більшим за MaxPlayers.");
                NotifyIfChanged(nameof(MinPlayers), _minPlayers.ToString(CultureInfo.InvariantCulture), newVal.ToString(CultureInfo.InvariantCulture));
                _minPlayers = newVal;
            }
        }

        public int MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                var newVal = RequireRange(value, 1, 20, nameof(MaxPlayers));
                if (newVal < _minPlayers)
                    throw new ArgumentOutOfRangeException(nameof(MaxPlayers), "MaxPlayers не може бути меншим за MinPlayers.");
                NotifyIfChanged(nameof(MaxPlayers), _maxPlayers.ToString(CultureInfo.InvariantCulture), newVal.ToString(CultureInfo.InvariantCulture));
                _maxPlayers = newVal;
            }
        }

        public int DurationMinutes
        {
            get => _durationMinutes;
            set
            {
                var newVal = RequireRange(value, 5, 600, nameof(DurationMinutes));
                NotifyIfChanged(nameof(DurationMinutes), _durationMinutes.ToString(CultureInfo.InvariantCulture), newVal.ToString(CultureInfo.InvariantCulture));
                _durationMinutes = newVal;
            }
        }

        public int MinAge
        {
            get => _minAge;
            set
            {
                var newVal = RequireRange(value, 3, 21, nameof(MinAge));
                NotifyIfChanged(nameof(MinAge), _minAge.ToString(CultureInfo.InvariantCulture), newVal.ToString(CultureInfo.InvariantCulture));
                _minAge = newVal;
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                var newVal = RequireRange(value, 0m, 10000m, nameof(Price));
                NotifyIfChanged(nameof(Price), _price.ToString("0.00", CultureInfo.InvariantCulture), newVal.ToString("0.00", CultureInfo.InvariantCulture));
                _price = newVal;
            }
        }

        public void PlayOnce()
        {
            TimesPlayed++; 
        }

        public string ShortInfo()
        {
            return $"[{Id}] {Title} — {Genre}, {MinPlayers}-{MaxPlayers} гравців, {DurationMinutes} хв, {MinAge}+ років, {Price:0.00}₴" +
                   (Publisher is { Length: > 0 } ? $", видавець: {Publisher}" : "") +
                   (IsPartyGame ? " (party)" : "");
        }

        private static string RequireString(string? value, int minLen, int maxLen, string propName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{propName}: значення порожнє.");
            var trimmed = value.Trim();
            if (trimmed.Length < minLen || trimmed.Length > maxLen)
                throw new ArgumentOutOfRangeException(propName, $"{propName}: довжина має бути {minLen}..{maxLen} символів.");
            return trimmed;
        }

        private static int RequireRange(int value, int min, int max, string propName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(propName, $"{propName}: допустимо {min}..{max}.");
            return value;
        }

        private static decimal RequireRange(decimal value, decimal min, decimal max, string propName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(propName, $"{propName}: допустимо {min}..{max}.");
            return value;
        }

        private static void NotifyIfChanged(string prop, string oldVal, string newVal)
        {
            if (!string.Equals(oldVal, newVal, StringComparison.Ordinal))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[i] {prop}: \"{oldVal}\" → \"{newVal}\"");
                Console.ResetColor();
            }
        }
    }
}
