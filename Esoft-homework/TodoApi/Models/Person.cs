using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsoftApi.Models
{
    public partial class Person
    {
        public Person()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int BirthYear { get; set; }
    }
}
