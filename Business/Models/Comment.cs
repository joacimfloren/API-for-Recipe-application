using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mjecipies_Group_D.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Grade { get; set; }
        public string Image { get; set; }
        public int Created { get; set; }
        public Account Account { get; set; }
        public Recipe Recipe { get; set; }
    }
}