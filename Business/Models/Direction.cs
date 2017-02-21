using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mjecipies_Group_D.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public Recipe Reciepe { get; set; }
    }
}
