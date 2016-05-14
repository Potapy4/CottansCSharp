using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CottansCSharp
{
    class Program
    {
        enum CC
        {
            AmericanExpress,
            JCB,
            Maestro,
            MasterCard,
            VISA,
            Unknown
        }

        static void RemoveDashes(ref string number)
        {
            number = number.Replace(" ", "");
            number = number.Replace("-", "");
        }

        static int LuhnAlgorithm(string number)
        {
            int sum = 0;
            int[] cardNumber = new int[number.Length];
            for (int i = 0; i < cardNumber.Length; ++i)
                cardNumber[i] = number[i] - '0';

            for (int i = cardNumber.Length - 1, j = 0, k = cardNumber.Length % 2; i >= 0; i--)
            {
                if (i % 2 == k)
                {
                    j = cardNumber[i] * 2;
                    if (j > 9)
                        j -= 9;
                    sum += j;
                }
                else
                    sum += cardNumber[i];
            }
            return sum;
        }

        static bool GetContainPrefix(ref string number)
        {
            bool result = false;
            int tmp = Convert.ToInt32(number.Substring(0, 2));

            if (Enumerable.Range(34, 1).Contains(tmp) || Enumerable.Range(37, 1).Contains(tmp))
                result = true;
            else if (Enumerable.Range(50, 1).Contains(tmp) || Enumerable.Range(56, 14).Contains(tmp))
                result = true;
            else if (Enumerable.Range(51, 5).Contains(tmp))
                result = true;
            else if (tmp / 10 == 4)
                result = true;
            else if (tmp == 35)
            {
                tmp = Convert.ToInt32(number.Substring(0, 4));
                if (tmp >= 3528 && tmp <= 3589)
                    result = true;
            }

            return result;
        }

        static string GetCreditCardVendor(string number)
        {
            RemoveDashes(ref number);
            string vendor = CC.Unknown.ToString();

            int first2digit = 0;
            Int32.TryParse(number.Substring(0, 2), out first2digit);

            if (LuhnAlgorithm(number) % 10 != 0)
                return vendor;

            if (number.Length == 15 && Enumerable.Range(34, 1).Contains(first2digit) || Enumerable.Range(37, 1).Contains(first2digit))
                vendor = CC.AmericanExpress.ToString();
            else if (number.Length >= 12 && number.Length <= 19 && Enumerable.Range(50, 1).Contains(first2digit) || Enumerable.Range(56, 14).Contains(first2digit))
                vendor = CC.Maestro.ToString();
            else if (number.Length == 16 && Enumerable.Range(51, 5).Contains(first2digit))
                vendor = CC.MasterCard.ToString();
            else if (number.Length == 13 || number.Length == 16 || number.Length == 19 && first2digit / 10 == 4)
                vendor = CC.VISA.ToString();
            else if (number.Length == 16 && first2digit == 35)
            {
                first2digit = Convert.ToInt32(number.Substring(0, 4));
                if (first2digit >= 3528 && first2digit <= 3589)
                    vendor = CC.JCB.ToString();
            }

            return vendor;
        }

        static bool IsCreditCardNumberValid(string number)
        {
            RemoveDashes(ref number);

            if (GetCreditCardVendor(number) != CC.Unknown.ToString())
            {
                int sum = LuhnAlgorithm(number);

                return (sum % 10 == 0);
            }
            else
                return false;
        }

        static string GenerateNextCreditCardNumber(string number)
        {
            RemoveDashes(ref number);
            string originalVendor = GetCreditCardVendor(number);
            string newVendor = "No more CC numbers available for this vendor or invalid CC number";

            if (originalVendor != CC.Unknown.ToString())
            {
                long nextCC = Int64.Parse(number);
                ++nextCC;

                string tmpDigit = nextCC.ToString();

                while (!IsCreditCardNumberValid(tmpDigit) && GetContainPrefix(ref tmpDigit))
                {
                    nextCC++;
                    tmpDigit = nextCC.ToString();
                }

                if (originalVendor == GetCreditCardVendor(Convert.ToString(nextCC)))
                    newVendor = Convert.ToString(nextCC);
            }

            return newVendor;
        }

        static void Main(string[] args)
        {
            string str = "5199999999999991";

            string s = GetCreditCardVendor(str);
            Console.WriteLine(s);

            Console.WriteLine(IsCreditCardNumberValid(str));

            s = GenerateNextCreditCardNumber(str);
            Console.WriteLine(s);

            Console.WriteLine(GetCreditCardVendor(s));

        }
    }
}
