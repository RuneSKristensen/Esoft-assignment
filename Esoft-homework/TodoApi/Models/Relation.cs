using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsoftApi.Models
{
    public partial class Relation
    {
        public Relation()
        {
        }

        public int Id { get; set; }

        public int FromPersonId { get; set; }
        public int ToPersonId { get; set; }
    }
}
