using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models.Mesurements
{
    public class MesurmentResponseList
    {
        public Guid set_id {  get; set; }
        public IEnumerable<MesurmentResponse> mesurements { get; set; }
    }
}
