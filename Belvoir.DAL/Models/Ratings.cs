﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class Ratings
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public decimal ratingvalue { get; set; }

        public string  message { get; set; }
    }
}
