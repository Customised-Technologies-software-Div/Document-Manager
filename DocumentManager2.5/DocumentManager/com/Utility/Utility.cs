using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DocumentManager.com.Utility
{
    public class Utility
    {
        public static string IncreaseStringByOne(string str)
        {
            string newString=Regex.Replace(str, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
            return newString;
        }
        public static void MakeAllColumnsWidthSame(DataGrid dataGrid)
        {
            //System.Diagnostics.Trace.WriteLine("Column Count "+gridCities.Columns.Count);
            foreach (var column in dataGrid.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        public static string GetExtension(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }

        public static bool IsNumber(string str)
        {
            return str.All(Char.IsDigit);
        }

        public static string ConvertfloatToString(float number,bool showDecimal=true)
        {
            NumberFormatInfo nfo = new NumberFormatInfo();
            nfo.CurrencyGroupSeparator = ",";
            // you are interested in this part of controlling the group sizes
            nfo.CurrencyGroupSizes = new int[] { 3, 2 };
            nfo.CurrencySymbol = "";

            if (showDecimal == true) return number.ToString("c2",nfo);
            else return number.ToString("c0",nfo);
        }

        public static string CapitaliseString(string str)
        {
            string pattern = @"\b\w";
            StringBuilder result = new StringBuilder(str);
            RegexOptions options = RegexOptions.Multiline;
            foreach(Match m in Regex.Matches(str,pattern,options))
            {
                result[m.Index] = m.Value.ToUpper()[0];
            }
            return result.ToString() ;
        }

        public static string repeatString(string str,int count)
        {
            return string.Concat(Enumerable.Repeat(str, count));
        }

        /// <summary>
        /// This can convert numeric value equivalent to rupees in words
        /// This can only be true till 1 Crore value, after that there are some irregularities
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertNumbertoWords(int number)
        {
            if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }

            if ((number / 10000000) > 0)
            {
                words += ConvertNumbertoWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " Lakh ";
                number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "")
                    words += "AND ";
                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }
    }

}
