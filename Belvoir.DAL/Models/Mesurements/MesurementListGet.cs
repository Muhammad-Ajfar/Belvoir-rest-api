using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.Mesurements
{
    public class MesurementListGet
    {
        public Guid id { get; set; }
        public string description { get; set; }
        public string measurement_name { get; set; }

    }
}
