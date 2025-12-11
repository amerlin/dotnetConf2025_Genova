using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryFilter.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsPrivate { get; set; } = false;
        public decimal Price { get; set; }
    }
}