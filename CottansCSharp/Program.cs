using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CottansCSharp
{
    class Program
    {
        /*
         Вендор:                  ID:              Длина номера карты:
         American Express        34, 37                  15
         Maestro                50, 56-69               12-19
         MasterCard              51-55                   16
         VISA                      4                     16
         JCB                    3528-3589                16
         */

        static Random rand = new Random();

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

        static string GetCreditCardVendor(string number)
        {
            RemoveDashes(ref number);
            string vendor = "Unknown";

            string first4digit = number.Substring(0, 4);
            int[] cardNumber = new int[first4digit.Length];

            for (int i = 0; i < cardNumber.Length; ++i)
                cardNumber[i] = number[i] - '0';

            int tmp = cardNumber[0] * 10 + cardNumber[1]; // Получаем первые 2 цифры и записываем в переменную
                                                          // Алгоритм: Указываем диапазон (начало, ДО какой позиции проверить) и проверяем (если наше значение и длина номера карты подходит под условие, то назначаем вендор)

            if (LuhnAlgorithm(number) % 10 != 0)
                return vendor;

            if (Enumerable.Range(34, 4).Contains(tmp) && number.Length == 15)
                vendor = "American Express";
            else if (Enumerable.Range(50, 1).Contains(tmp) || Enumerable.Range(56, 14).Contains(tmp) && number.Length >= 12 && number.Length <= 19)
                vendor = "Maestro";
            else if (Enumerable.Range(51, 5).Contains(tmp) && number.Length >= 16 && number.Length <= 19)
                vendor = "Master Card";
            else if (tmp / 10 == 4 && number.Length >= 13) // Для VISA
                vendor = "VISA";
            else if (tmp == 35 && number.Length == 16) // Если начинается на 35 и длина 16, то возможно это JCB, дальше чекнем
            {
                tmp = cardNumber[0] * 1000 + cardNumber[1] * 100 + cardNumber[2] * 10 + cardNumber[3];
                if (tmp >= 3528 && tmp <= 3589)
                    vendor = "JCB";
            }

            return vendor;

        }

        static bool IsCreditCardNumberValid(string number)
        {
            RemoveDashes(ref number);

            if (GetCreditCardVendor(number) != "Unknown")
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
            if (originalVendor != "Unknown")
            {
                long nextCC = Int64.Parse(number);
                ++nextCC;
                while (!IsCreditCardNumberValid(Convert.ToString(nextCC)))
                    nextCC++;

                if (originalVendor == GetCreditCardVendor(Convert.ToString(nextCC)))
                    newVendor = Convert.ToString(nextCC);
            }

            return newVendor;
        }

        static void Main(string[] args)
        {
            string str = "35301113333000001";

            string s = GetCreditCardVendor(str);
            Console.WriteLine(s);

            Console.WriteLine(IsCreditCardNumberValid(str));

            string a = GenerateNextCreditCardNumber(str);
            Console.WriteLine(a);

            Console.WriteLine(GetCreditCardVendor(a));

        }
    }
}
