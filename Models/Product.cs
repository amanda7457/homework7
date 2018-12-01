using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HW7_Barron_Amanda.Models
{
    public class Product
    {
        [Display(Name = "Product ID")]
        public Int32 ProductID { get; set; }

        //SKU should start with 5001 and go up by one from there
        public Int32 SKU { get; set; }

        [Display(Name = "Product Name")]
        public String ProductName { get; set; }

        [Display(Name = "Product Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ProductPrice { get; set; }

        [Display(Name = "Product Description")]
        public String ProductDescription { get; set; }

        //navagationproperies

        public List<OrderDetail> OrderDetails { get; set; }
        public List<ProductSupplier> ProductSuppliers { get; set; }

        public Product()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }

            if (ProductSuppliers == null)
            {
                ProductSuppliers = new List<ProductSupplier>();
            }
        }
    }
}
