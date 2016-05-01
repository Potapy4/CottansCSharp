﻿using System;
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

        static int LuhnAlgorithm(ref int[] cardNumber)
        {
            int sum = 0;
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
            string first4digit = number.Substring(0, 4);
            int[] cardNumber = new int[first4digit.Length];
            string vendor = "Unknown";

            for (int i = 0; i < cardNumber.Length; ++i)
                cardNumber[i] = number[i] - '0';

            int tmp = cardNumber[0] * 10 + cardNumber[1]; // Получаем первые 2 цифры и записываем в переменную
            // Алгоритм: Указываем диапазон (начало, ДО какой позиции проверить) и проверяем (если наше значение и длина номера карты подходит под условие, то назначаем вендор)
            if (Enumerable.Range(34, 4).Contains(tmp) && number.Length == 15)
                vendor = "American Express";
            else if (Enumerable.Range(50, 0).Contains(tmp) || Enumerable.Range(56, 14).Contains(tmp) && number.Length >= 12 && number.Length <= 19)
                vendor = "Maestro";
            else if (Enumerable.Range(51, 5).Contains(tmp) && number.Length >= 16 && number.Length <= 19)
                vendor = "Master Card";
            else if (tmp / 10 == 4 && number.Length >= 13 && number.Length <= 16) // Для VISA
                vendor = "VISA";
            else if (tmp == 35 && number.Length == 16) // Если начинается на 35 и длина 16, то возможно это JCB, дальше чекнем
            {
                tmp = first4digit[0] * 1000 + first4digit[1] * 100 + first4digit[2] * 10 + first4digit[3];
                if (tmp >= 3528 && tmp <= 3589)
                    vendor = "JCB";
            }

            // P.S. -  Есть карты разной длины и типа (Бизнес, Дебетовая, Кредитная и т.п) - т.е. может быть и VISA с длиной < 16 и MasterCard > 16.
            // Инфа взята с http://www.freeformatter.com/credit-card-number-generator-validator.html

            return vendor;
        }

        static bool IsCreditCardNumberValid(string number)
        {
            RemoveDashes(ref number);

            int[] cardNumber = new int[number.Length];

            for (int i = 0; i < cardNumber.Length; ++i)
                cardNumber[i] = number[i] - '0';

            int sum = LuhnAlgorithm(ref cardNumber);

            return (sum % 10 == 0);
        }

        static string GenerateNextCreditCardNumber(string number)
        {
            RemoveDashes(ref number);

            int pos = 2; // По дефолту префикс всегда 2 цифры
            int[] cardNumber = new int[number.Length];

            // Тут читаем префикс карты, в двух случаях они разные (либо 1, либо 4 цифры)
            if (GetCreditCardVendor(number) == "VISA")
                --pos;
            else if (GetCreditCardVendor(number) == "JCB")
                pos += 2;

            for (int i = 0; i < pos; ++i)
                cardNumber[i] = number[i] - '0';

            // Рандомим
            while (pos < cardNumber.Length - 1)
                cardNumber[pos++] = rand.Next(10);

            int sum = LuhnAlgorithm(ref cardNumber);

            // И вычисляем контрольную цифру для карты
            cardNumber[cardNumber.Length - 1] = (10 - (sum % 10)) % 10;

            return string.Join("", cardNumber);
        }

        static void Main(string[] args)
        {
            string s = GetCreditCardVendor("5893188858726104");
            Console.WriteLine(s);

            Console.WriteLine(IsCreditCardNumberValid("5893188858726104"));

            string a = GenerateNextCreditCardNumber("5893188858726108");
            Console.WriteLine(a);

            Console.WriteLine(IsCreditCardNumberValid(a));

        }
    }
}