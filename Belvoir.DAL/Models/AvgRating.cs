using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class AvgRating
    {
        public decimal averageRating {  get; set; }
        public int count {  get; set; }
        public IEnumerable<Ratings> ratings { get; set; }
    }
}
