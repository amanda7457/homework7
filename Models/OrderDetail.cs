using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HW7_Barron_Amanda.Models
{
    public class OrderDetail
    {
        public Int32 OrderDetailID { get; set; }

        [Display(Name = "Order Quantity")]
        [Range (1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public Int32 OrderQuantity { get; set; }

        [Display(Name = "Order Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderPrice { get; set; }

        [Display(Name = "Extended Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ExtendedPrice { get; set; }

        //navagation properties

        public Order Order { get; set; }
        public Product Product { get; set; }
        public AppUser Customer { get; set; }


    }
}
