using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Address
{
    public class AddressGetDTO
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string BuildingName { get; set; } // Building/house name
        public string ContactName { get; set; }  // Name for contacting
        public string ContactNumber { get; set; } // Number for contacting
    }
}
