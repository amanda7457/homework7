using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HW7_Barron_Amanda.Models
{
    public class ProductSupplier
    {
        public Int32 ProductSupplierID { get; set; }

        //navagation properties
        public Product Product { get; set; }
        public Supplier Supplier { get; set; }
    }
}
