using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.Mesurements
{
    public class MesurementSet
    {
        public Guid design_id { get; set; }
        public string set_name { get; set; }
        public List<MesurmentValues> values { get; set; }
    }
}
