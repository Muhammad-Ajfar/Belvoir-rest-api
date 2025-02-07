using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.Mesurements
{
    public class MesurmentGuidGet
    {
        public Guid guide_id { get; set; }
        public string measurement_name { get; set; }
        public string description { get; set; }
    }
}
