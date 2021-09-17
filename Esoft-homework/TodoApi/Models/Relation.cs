using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{

    public enum Connection
    {
        Parent,
        Child

    }
    public partial class Relation
    {
        public Relation()
        {
        }

        public int Id { get; set; }

        public Connection Connection { get; set; }

        public virtual ICollection<Person> Persons { get; set; }

    }
}
