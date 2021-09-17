using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public partial class Person
    {
        public Person()
        {
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int BirthYear { get; set; }
    }
}
