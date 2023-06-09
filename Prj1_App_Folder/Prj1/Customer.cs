using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj1
{
    public class Customer
    {
        public Customer() { }

        public Customer(string fname, string lname)
        {
            Fname = fname;
            Lname = lname;
        }

        //field
        public readonly int age = 0;
        public Guid CustomerId { get; set; } = Guid.NewGuid(); // when you create a property with a setter, C# creates a private backing variable of the same name for you. YOu don't see it.

        private string fname = string.Empty;
        public string Fname
        {
            get { return fname; }
            set
            {
                if (value.Length < 2)
                {
                    throw new FormatException(); // TODO find out why this exception didn't crash the program
                }
                else
                {
                    this.fname = value;
                }
            }
        }

        public string Lname { get; set; } = string.Empty;// this is a "property"

        //TODO add a speak method to inherit
    }
}
