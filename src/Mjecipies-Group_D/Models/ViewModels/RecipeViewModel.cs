using System.Collections.Generic;

namespace Mjecipies_Group_D.Models.ViewModels
{
    public class RecipeViewModel
    {
        public class Create
        {
            public string CreatorId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public ICollection<Direction> Directions { get; set; }
        }
        public class Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public ICollection<Direction> Directions { get; set; }
        }
    }
}
