using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MatUlohy {
    
    using Ukol1;
    using Ukol2;
    using Ukol3;
    using Ukol4;
    
    internal class Program {
        public static void Main(string[] args) {
            {
                Console.WriteLine(Validation.Validate("jan.novak@mensagymnazium.cz")); // true
                Console.WriteLine(Validation.Validate("petr.svoboda@gmail.com"));      // true
                Console.WriteLine(Validation.Validate("anicka123@yahoo.co.uk"));       // true
                Console.WriteLine(Validation.Validate("tomas@outlook.com"));           // true
                Console.WriteLine(Validation.Validate("lucie.nova@seznam.cz"));        // true
                Console.WriteLine(Validation.Validate("pavel_kral@centrum.cz"));       // true
                Console.WriteLine(Validation.Validate("katka.mala123@volny.cz"));      // true
                Console.WriteLine(Validation.Validate("michal@fit.cvut.cz"));          // true
                Console.WriteLine(Validation.Validate("eva.novotna@mail.com"));        // true
                Console.WriteLine(Validation.Validate("marta.svobodova@uni.edu"));     // true
        
                Console.WriteLine(Validation.Validate("jan.novak@mensagymnazium"));    // false
                Console.WriteLine(Validation.Validate("petr.svoboda@gmail"));          // false
                Console.WriteLine(Validation.Validate("@yahoo.co.uk"));                // false
                Console.WriteLine(Validation.Validate("tomas@.com"));                  // false
                Console.WriteLine(Validation.Validate("lucie.nova@seznam..cz"));       // false
                Console.WriteLine(Validation.Validate("pavel_kral@.cz"));              // false
                Console.WriteLine(Validation.Validate("katka.mala123volny.cz"));       // false
                Console.WriteLine(Validation.Validate("michal@fit@cvut.cz"));          // false
                Console.WriteLine(Validation.Validate("eva.novotnamail.com"));         // false
                Console.WriteLine(Validation.Validate("marta.svobodova@uni..edu"));    // false
            }

            {        
                var exchangeRates = new List<Tuple<string, string, double>> {
                Tuple.Create("CZK", "EUR", 25.3),
                Tuple.Create("CZK", "USD", 24.1),
                Tuple.Create("EUR", "USD", 0.95),
                Tuple.Create("USD", "JPY", 17.3),
                Tuple.Create("HRK", "NOK", 0.25),
                Tuple.Create("EUR", "NOK", 7.1)
                };

                CurrencyExchange.Exchanges(exchangeRates, "CZK", "NOK");
            }

            {
                ToBinary.convert(1709170370);
                ToBinary.convert(2324);
                ToBinary.convert(1541453647437347534);
                ToBinary.convert(354235);
                ToBinary.convert(1);
                ToBinary.convert(100);
            }

            {
                string input1 = "{START:outer}\n\ttext\n    {START:inner}text{END:inner}\n{END:outer}";
                string input2 = "{START:outer}\n    {START:inner}text\n{END:outer}\n    text{END:inner}";
                string input3 = "{START:outer}\n    {START:inner}\n        text\n    {END:outer}\n{END:inner};";
                string input4 = "{START:main}\n    {START:section1}\n        content of section 1\n    {END:section2}\n    {START:section2}\n        {START:subsection}\n            text\n        {END:subsection}\n    {END:section2}\n{END:main};";
                string input5 = "{START:main}\n    {START:section1}\n        content of section 1\n    {END:section1}\n    {START:section2}\n        {START:subsection}\n            text\n        {END:subsection}\n    {END:section2}\n{END:main};";
                string input6 = "{START:outer}\n    {START:inner1}\n        text\n    {END:inner1}\n    {START:inner2}\n        {START:inner3}\n            text\n        {END:inner3}\n    {END:inner2}\n{END:outer};";



                Console.WriteLine(Validator2.IsValid(input1));
                Console.WriteLine(Validator2.IsValid(input2));
                Console.WriteLine(Validator2.IsValid(input3));
                Console.WriteLine(Validator2.IsValid(input4));
                Console.WriteLine(Validator2.IsValid(input5));
                Console.WriteLine(Validator2.IsValid(input6));
            }
        }
    }
}

namespace Ukol1 {
    static class Validation {
        public static bool Validate(string email) {
            Console.Write(email + ": ");
            bool isValid = S1(0, email);
            Console.WriteLine(isValid);
            return isValid;
        }

        private static bool S1(int p, string email) {
            if (email.Length <= p)
                return false;

            if (char.IsLetter(email[p]) || char.IsNumber(email[p]))
                return S2(p + 1, email);

            return false;
        }

        private static bool S2(int p, string email) {
            if (email.Length <= p)
                return false;

            if (email[p] == '.')
                return S3(p + 1, email);

            if (email[p] == '@')
                return S6(p + 1, email);

            if (char.IsLetter(email[p]) || char.IsNumber(email[p]))
                return S2(p + 1, email);

            return false;
        }

        private static bool S3(int p, string email) {
            if (email.Length <= p)
                return false;

            if (char.IsLetter(email[p]) || char.IsNumber(email[p]))
                return S2(p + 1, email);

            return false;
        }

        private static bool S4(int p, string email, bool dot) {
            if (email.Length <= p)
                return dot;

            if (email[p] == '.')
                return S5(p + 1, email);

            if (char.IsLetter(email[p]) || char.IsNumber(email[p]))
                return S4(p + 1, email, dot);

            return false;
        }

        private static bool S5(int p, string email) {
            if (email.Length <= p)
                return false;

            if (char.IsLetter(email[p]) || char.IsNumber(email[p]))
                return S4(p + 1, email, true);

            return false;
        }

        private static bool S6(int p, string email) {
            if (email.Length <= p)
                return false;

            if (char.IsLetter(email[p]) || char.IsNumber(email[p]))
                return S4(p + 1, email, false);

            return false;
        }

    }
}

namespace Ukol2 {
    public static class CurrencyExchange {
        private static List<string> FindShortestExchangeSequence(
            Dictionary<string, List<Tuple<string, double>>> exchangeRates, string startCurrency,
            string targetCurrency) {
            var queue = new Queue<List<string>>();
            var visited = new HashSet<string>();

            queue.Enqueue(new List<string>
            { startCurrency });
            visited.Add(startCurrency);

            while (queue.Count > 0) {
                var path = queue.Dequeue();
                var lastCurrency = path.Last();

                if (lastCurrency == targetCurrency) {
                    return path;
                }

                if (!exchangeRates.ContainsKey(lastCurrency)) {
                    continue;
                }

                foreach (var next in exchangeRates[lastCurrency]) {
                    if (!visited.Contains(next.Item1)) {
                        var newPath = new List<string>(path)
                        { next.Item1 };
                        queue.Enqueue(newPath);
                        visited.Add(next.Item1);
                    }
                }
            }

            return null; // Neexistuje žádná cesta
        }

        public static void Exchanges(List<Tuple<string, string, double>> exchangeRates, string startCurrency, string targetCurrency) {
            var exchangeRatesDict = new Dictionary<string, List<Tuple<string, double>>>();

            foreach (var rate in exchangeRates) {
                if (!exchangeRatesDict.ContainsKey(rate.Item1)) {
                    exchangeRatesDict[rate.Item1] = new List<Tuple<string, double>>();
                }

                exchangeRatesDict[rate.Item1].Add(Tuple.Create(rate.Item2, rate.Item3));
            }

            var shortestSequence = FindShortestExchangeSequence(exchangeRatesDict, startCurrency, targetCurrency);

            if (shortestSequence != null) {
                Console.WriteLine("Nejkratší posloupnost směn:");
                for (int i = 0; i < shortestSequence.Count - 1; i++) {
                    Console.WriteLine($"[{shortestSequence[i]}, {shortestSequence[i + 1]}]");
                }
            } else 
                Console.WriteLine("Nelze najít cestu mezi zadanými měnami.");
        }
    }
}

namespace Ukol3 {
    static class ToBinary {
        public static char[] convert(UInt64 Int) {
            string binaryString = Convert.ToString((long)Int, 2);
            
            char[] binaryCharArray = binaryString.ToCharArray();

            int numOFOnes = 0;
            
            foreach (char ch in binaryCharArray)
                if (ch == '1')
                    numOFOnes++;
            
            Console.Write(Int);
            Console.Write($" => ({numOFOnes}),");
            Console.WriteLine(" " + binaryString);

            return binaryCharArray;
        }
    }
}

namespace Ukol4 {
    static class Validator2 {
        public static bool IsValid(string input) {
            Stack<string> stack = new Stack<string>();
            string pattern = @"\{(?:START|END):([^\s}]+)\}";

            foreach (Match match in Regex.Matches(input, pattern)) {
                string tag = match.Value;
                string name = match.Groups[1].Value;

                if (tag.StartsWith("{START:")) {
                    stack.Push(name);
                }
                else if (tag.StartsWith("{END:")) {
                    if (stack.Count == 0 || stack.Peek() != name) {
                        return false; 
                    }

                    stack.Pop(); 
                }
            }

            return stack.Count == 0; 
        }
    }
}