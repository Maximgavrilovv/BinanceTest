using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceApiTest.Models
{
    public class BinanceRecord
    {
        [Key]
        public int Id { get; set; }
        public string Pair { get; set; }
        public double Price { get; set; }
    }
}
