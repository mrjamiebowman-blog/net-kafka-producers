using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBasedProducer.Models
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }

        public int CustomerId { get; set; }

        public string PoNumber { get; set; }

        public decimal Amount { get; set; }
    }
}
