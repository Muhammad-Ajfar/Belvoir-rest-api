using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class ClothCategory
    {
        public IEnumerable<CategoryItem> Designtype { get; set; }

        public IEnumerable<CategoryItem> Materialtype { get; set; }

        public IEnumerable<CategoryItem> colors { get; set; }


    }
}
