using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SSN_Generator

{
    public class Person
    {

        private char sex;
        private string ssn;
        private static Random r = new Random();

        public string Ssn
        {
            get
            {
                return ssn;
            }
            set
            {
                if (value.Length != 11 && !int.TryParse(value, out int res)) throw new ArgumentException($"Invalid Ssn: {value}");
                ssn = value;
            }
        }
        public char Sex
        {
            get
            {
                return sex;
            }
            set
            {
                sex = value;
            }
        }
        public DateTime Dob { get; set; }

        public Person(string ssn, char sex, DateTime dob)
        {
            Ssn = ssn;
            Sex = sex;
            Dob = dob;
        }



        /// <summary>
        ///Fastsettelse av individsifre
        ///000–499 omfatter personer født i perioden 1900–1999.
        ///500–749 omfatter personer født i perioden 1854–1899.
        ///500–999 omfatter personer født i perioden 2000–2039.
        ///900–999 omfatter personer født i perioden 1940–1999.
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="birthinterval"></param>
        /// <returns></returns>
        public static List<String> GenerateSSN(DateTime dob, int count, string birthinterval, char sex)
        {
            List<string> ssnList = new List<string>();
            string dobstring = dob.ToString("ddMMyy");
            string identityInterval = IdentityInterval(birthinterval);
            string ssn = "";
            string identitynum = "";
            int index = 0;

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
     
            while (index < count && !cancellationToken.IsCancellationRequested)
            {
     
                identitynum = RandomIdentityNumber(identityInterval, (sex == 'B') ? RandomSex() : sex);
                ssn = AddCheckDigits(dobstring + identitynum);

                if (ssn != "" && ssnList.IndexOf(ssn) == -1)
                {
                    ssnList.Add(ssn);
                    index++;

                }

            }


            return ssnList;
        }

        public static string GenerateSSN(DateTime dob, string birthinterval, char sex)
        {
            
            string dobstring = dob.ToString("ddMMyy");
            string identityInterval = IdentityInterval(birthinterval);
            string ssn = "";
            string identitynum = "";
            while (true)
            {
                char newSex = (sex == 'B') ? RandomSex() : sex;
                identitynum = RandomIdentityNumber(identityInterval, newSex);
                ssn = AddCheckDigits(dobstring + identitynum);
                if (ssn != "")
                {
                    
                    return ssn;
                }
            }
            
        }

        public static List<Person> GeneratePersonList(DateTime dob, int cntssn, string birthinterval, char sex)
        {
            List<Person> persons = new List<Person>();



            List<string> ssnList = GenerateSSN(dob, cntssn, birthinterval, sex);

            foreach (string s in ssnList)
            {
                persons.Add(new Person(s, FindSex(s), dob));
            }

            return persons;
        }

        public static char FindSex(string ssn)
        {
            return (int.Parse(ssn.Substring(8, 1)) % 2 == 0) ? 'F' : 'M';
        }



        public static List<Person> GenerateRandomList(int count, string birthinterval, char sex)
        {
            List<Person> persons = new List<Person>();
            int idx = 0;

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;

            while (idx < count && !cancellationToken.IsCancellationRequested)
            {

                DateTime rndDate = RandomDay(birthinterval);
                char newSex = (sex == 'B') ? RandomSex() : sex;
                string ssn = GenerateSSN(rndDate, birthinterval, sex);
                Person p = new Person(ssn, newSex, rndDate);
                if (persons.IndexOf(p) == -1)
                {
                    persons.Add(p);
                    idx++;
                }

            }
            return persons;

        }

        private static char RandomSex()
        {
            int tmp = r.Next(1, 3);
            return (tmp == 1) ? 'M' : 'F';
        }


        ///000–499 omfatter personer født i perioden 1900–1999.
        ///500–749 omfatter personer født i perioden 1854–1899.
        ///500–999 omfatter personer født i perioden 2000–2039.
        ///900–999 omfatter personer født i perioden 1940–1999.
        ///
        private static string IdentityInterval(string birthinterval)
        {
            string interval = "";

            switch (birthinterval)
            {
                case "1900-1999":
                    interval = "000-499";
                    break;
                case "1854-1899":
                    interval = "500-749";
                    break;
                case "2000-2039":
                    interval = "500-999";
                    break;
                case "1940-1999":
                    interval = "900-999";
                    break;
                default:
                    throw new ArgumentException($"Invalid Birthinterval {birthinterval}");
            }
            
            return interval;
        }



        public static DateTime RandomDay(string birthinterval)
        {

            List<int> dateRange = birthinterval.Split('-').Select(i => int.Parse(i)).ToList();
            DateTime fromdate = new DateTime(dateRange[0], 1, 1);
            int dumDaysDiff = (int)(new DateTime(dateRange[1], 12, 31) - fromdate).TotalDays;


            return fromdate.AddDays(r.Next(dumDaysDiff));

        }


        public static string RandomIdentityNumber(string identityRange, char sex)
        {



            List<int> from_to = identityRange.Split('-').Select(i => int.Parse(i)).ToList();

            Trace.WriteLine($"FROM:{ from_to[0]}, TO:{from_to[1]}");

            from_to[0] = (from_to[0] == 0) ? 0 : from_to[0] / 2;
            from_to[1] = (from_to[1] + 1) / 2;

            string identitynum = "";
            if (sex == 'M')
            {
                identitynum = (1 + 2 * r.Next(from_to[0], from_to[1])).ToString("000");

            }
            else if (sex == 'F')
            {
                identitynum = (2 * r.Next(from_to[0], from_to[1])).ToString("000");
            }
            else
            {
                throw new ArgumentException($"Invalid Sex:{ sex }");
            }

            return identitynum;
        }



        public static string AddCheckDigits(string number)
        {
            if (number.Length != 9) throw new ArgumentException($"Invalid length {number.Length}, expected 9");

            var numbers = number.ToCharArray().Select(i => int.Parse(i.ToString())).ToList();

            int sum = 3 * numbers[0] + 7 * numbers[1] + 6 * numbers[2] + 1 * numbers[3]
                + 8 * numbers[4] + 9 * numbers[5] + 4 * numbers[6] + 5 * numbers[7] + 2 * numbers[8];

            numbers.Add((11 - sum % 11)); //Checkdigit One

            if (numbers[9] == 11) numbers[9] = 0;
            if (numbers[9] == 10) return "";

            sum = 5 * numbers[0] + 4 * numbers[1] + 3 * numbers[2] + 2 * numbers[3] + 7 * numbers[4]
                + 6 * numbers[5] + 5 * numbers[6] + 4 * numbers[7] + 3 * numbers[8] + 2 * numbers[9];

            numbers.Add((11 - sum % 11)); //Checkdigit Two

            if (numbers[10] == 11) numbers[10] = 0;
            if (numbers[10] == 10) return "";

            return number + numbers[9] + numbers[10];
        }


        public override string ToString()
        {
            return $"{Ssn},{Sex},{Dob.ToString("yyyy-MM-dd")}";
        }




        public override bool Equals(object obj)
        {
            return ssn.Equals(((Person)obj).ssn);
        }

    }
}
