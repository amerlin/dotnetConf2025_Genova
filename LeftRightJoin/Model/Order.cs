using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeftRightJoin.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}