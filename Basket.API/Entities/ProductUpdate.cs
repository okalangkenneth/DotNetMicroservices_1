using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ProductUpdate
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
