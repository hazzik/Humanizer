using System;
using System.Collections.Generic;
using System.Linq;

namespace Humanizer.Localisation.NumberToWords
{
    internal class PolishNumberToWordsConverter : DefaultNumberToWordsConverter
    {
        private static readonly string[] HundredsMap = { string.Empty, "sto", "dwieście", "trzysta", "czterysta", "pięćset", "sześćset", "siedemset", "osiemset", "dziewięćset" };
        private static readonly string[] TensMap = { string.Empty, "dziesięć", "dwadzieścia", "trzydzieści", "czterdzieści", "pięćdziesiąt", "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt" };
        private static readonly string[] UnitsMap = { "zero", "jeden", "dwa", "trzy", "cztery", "pięć", "sześć", "siedem", "osiem", "dziewięć", "dziesięć", "jedenaście", "dwanaście", "trzynaście", "czternaście", "piętnaście", "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście" };
        private static readonly string[] Miliard = { "miliard", "miliardy", "miliardów" };
        private static readonly string[] Million = { "milion", "miliony", "milionów" };
        private static readonly string[] Thousand = { "tysiąc", "tysiące", "tysięcy" };

        private enum Numeral
        {
            One = 1,
            Thousand = 1000,
            Million = 1000000,//10^6
            Miliard = 1000000000,//10^9
        }

        private const string Negative = "minus";
        private const string Zero = "zero";

        private static void ConvertNumberUnderThousand(ICollection<string> parts, Numeral numeral, int number)
        {
            if (numeral != Numeral.One && number == 1)
                return;

            var hundreds = number / 100;
            if (hundreds > 0)
            {
                parts.Add(HundredsMap[hundreds]);
                number = number % 100;
            }

            var tens = number / 10;
            if (tens > 1)
            {
                parts.Add(TensMap[tens]);
                number = number % 10;
            }

            if (number > 0)
            {
                parts.Add(UnitsMap[number]);
            }
        }

        private static int GetMappingIndex(int number)
        {
            if (number == 1)
                return 0;

            var unity = number % 10;
            if (unity > 1 && unity < 5)
                return 1; //denominator (Paucal)

            return 2; //genitive (Plural)
        }

        private static string GetSuffix(Numeral numeral, int num)
        {
            switch (numeral)
            {
                case Numeral.Miliard:
                    return Miliard[GetMappingIndex(num)];
                case Numeral.Million:
                    return Million[GetMappingIndex(num)];
                case Numeral.Thousand:
                    return Thousand[GetMappingIndex(num)];
                default:
                    return string.Empty;
            }
        }

        public override string Convert(int number)
        {
            if (number == 0)
                return Zero;

            var parts = new List<string>();

            if (number < 0)
            {
                parts.Add(Negative);
                number = Math.Abs(number);
            }

            var numerals = ((Numeral[]) Enum.GetValues(typeof (Numeral))).OrderByDescending(x => (int) x);
            foreach (var numeral in numerals)
            {
                var num = number / (int)numeral;
                if (num > 0)
                {
                    ConvertNumberUnderThousand(parts, numeral, num);
                    parts.Add(GetSuffix(numeral, num));
                    number %= (int)numeral;
                }
            }

            return string.Join(" ", parts.Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}
