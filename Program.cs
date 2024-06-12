using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MatUlohy {
    
    using Ukol1;
    using Ukol3;
    using Ukol4;
    
    internal class Program {
        public static void Main(string[] args) {
            {
                Validation val = new Validation();

                val._settings.ValidateDomain = true;
                val._settings.ValidateEmail = false;

                val.AddDomain("centrum.cz");
                val.AddDomain("seznam.cz");
                val.AddDomain("gmail.com");

                Console.WriteLine(val.Validate("adam@centrum.cz"));
                Console.WriteLine(val.Validate("adam@yahoo.com"));

                val._settings.ValidateEmail = true;

                Console.WriteLine(val.Validate("adam@centrum.cz"));
                Console.WriteLine(val.Validate("adam@yahoo.com"));

                val.AddSet(new[]
                { "adam@centrum.cz", "pepa@centrum.cz", "pepa@yahoo.cz" });

                Console.WriteLine(val.Validate("adam@centrum.cz"));
                Console.WriteLine(val.Validate("pepa@yahoo.cz"));
                Console.WriteLine(val.Validate("pepa@mensa.cz"));

                val.AddDomain("mensa.cz");

                val._settings.ValidateDomain = false;

                Console.WriteLine(val.Validate("adam@centrum.cz"));
                Console.WriteLine(val.Validate("pepa@yahoo.cz"));
                Console.WriteLine(val.Validate("pepa@mensa.cz"));
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
    class Validation {
        public Settings _settings;

        private List<string> domains;
        private List<(string, List<string>)> emails;

        public Validation() {
            domains = new List<string>();
            emails = new List<(string,  List<string>)>();
        }
        
        public void AddSet(string[] set) {
            foreach (var email in set) {
                string[] emailSplit = email.Split('@');

                if (emailSplit.Length != 2) 
                    continue;
                
                if (emails.All(t => t.Item1 != emailSplit[1]))
                    emails.Add((emailSplit[1], new List<string>()));

                var tuple = emails.FirstOrDefault(t => t.Item1 == emailSplit[1]);
                    if (!tuple.Item2.Contains(emailSplit[0]))
                        tuple.Item2.Add(emailSplit[0]);
            }

            foreach (var line in emails) {
                foreach (var a in line.Item2) {
                    Console.Write(a + " ,");
                }
                
                Console.WriteLine();
            }
        }

        public void AddDomain(string domain) {
            if (domains.Contains(domain))
                return;
            
            domains.Add(domain);
        }
        
        public bool Validate(string email) {
            Console.Write($"email: {email} ");
            string[] emailSplit = email.Split('@');

            if (emailSplit.Length != 2)
                return false;

            if (emailSplit[1][emailSplit[1].Length - 3] != '.')
                return false;
            
            if (_settings.ValidateDomain)
                if (!domains.Contains(emailSplit[1]))
                    return false;

            if (_settings.ValidateEmail) {
                if (emails.All(t => t.Item1 != emailSplit[1])) {
                    return false;
                }
                else if (!emails.FirstOrDefault(t => t.Item1 == emailSplit[1]).Item2.Contains(emailSplit[0]))
                    return false;
            }

            return true;
        }
    }


    public struct Settings {
        public bool ValidateDomain;
        public bool ValidateEmail;
    }
    
}

namespace Ukol2 {
    
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