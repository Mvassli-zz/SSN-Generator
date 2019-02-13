using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSN_Generator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SSN_Generator.Tests
{
    [TestClass()]
    public class PersonTests
    {
        [TestMethod()]
        public void GenerateSSNTest()
        {
            DateTime dt = DateTime.ParseExact("05061999", "ddMMyyyy", CultureInfo.InvariantCulture);
            string SSN = Person.GenerateSSN(dt, "1940-1999", 'B');
            Console.WriteLine(SSN);


            throw new NotImplementedException();
        }
    }
}