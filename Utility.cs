namespace CompanyFundingAPI
{
    public enum ConfigurationKeys
    {
        SECAPI,
        LocalStorage,
        SeedDataFile,
        YearRangeForFunding,
        IncomeThreshold,
        GreaterThanPercentage,
        LessThanPercentage,
        AddSpecialPercentage,
        SubtractSpecialPercentage
    }
    public class Utility
    {

        public static async Task<List<string>> ReadCIKNumbersFromFile(string fileName)
        {
            List<string> cikNumbers = new();

            try
            {
                using (StreamReader sr = new(fileName))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        string[] numbers = line.Split(',');
                        foreach (string number in numbers)
                        {
                            // Prepend leading zeros to make 10 digits
                            string paddedNumber = number.PadLeft(10, '0');
                            cikNumbers.Add(paddedNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CIK numbers from file: {ex.Message}");
            }

            return cikNumbers;
        }

        public static List<string> GetYears(string yearRange)
        {
            List<string> years = new List<string>();

            string[] parts = yearRange.Split('-');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid year range format. Expected format: 'YYYY-YYYY'");
            }

            int startYear, endYear;
            if (!int.TryParse(parts[0], out startYear) || !int.TryParse(parts[1], out endYear))
            {
                throw new ArgumentException("Invalid year range format. Years must be integers.");
            }

            if (startYear > endYear)
            {
                throw new ArgumentException("Invalid year range. Start year cannot be greater than end year.");
            }

            for (int year = startYear; year <= endYear; year++)
            {
                years.Add(year.ToString());
            }

            return years;
        }

        public static bool ContainsVowel(string name)
        {
            // Convert the name to lowercase to simplify the comparison
            string lowercaseName = name.ToLower();

            // Define an array of vowels
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };

            // Iterate through each character in the name
            foreach (char c in lowercaseName)
            {
                // Check if the character is a vowel
                if (Array.IndexOf(vowels, c) != -1)
                {
                    // If a vowel is found, return true
                    return true;
                }
            }

            // If no vowel is found, return false
            return false;
        }

        public static bool IsValueGreaterEqualThanAmount(decimal value,decimal amount)
        {
         

            // Check if the value of parameter 1 is greater than parameter 2 amount
            return value >= amount;
        }

        public static decimal CalculatePercentage(decimal amount, decimal percentage)
        {
            // Convert percentage to decimal
            decimal percentageDecimal = percentage / 100;

            // Calculate the result
            decimal result = amount * percentageDecimal;

            return result;
        }
    }
}
