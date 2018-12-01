using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace HW7_Barron_Amanda.Models
{
    public class Order
    {
        public const Decimal TAX_RATE = 0.0825m;

        [Display(Name = "Order ID")]
        public Int32 OrderID { get; set; }

        //Order number starts w 10001
        [Display(Name = "Order Number")]
        public Int32 OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Notes")]
        public String OrderNotes { get; set; }

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderSubtotal
        {
            get { return OrderDetails.Sum(rd => rd.ExtendedPrice); }
        }

        [Display(Name = "TAX (.0825%)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal SalesTax
        {
            get { return OrderSubtotal * TAX_RATE; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + SalesTax; }
        }

        //navagation properties

        public List<OrderDetail> OrderDetails { get; set; }
        public AppUser Customer { get; set; }

        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}
