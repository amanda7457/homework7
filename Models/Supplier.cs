using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HW7_Barron_Amanda.Models
{
    public class Supplier
    {
        [Display(Name = "Supplier ID")]
        public Int32 SupplierID { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Supplier Name")]
        public String SupplierName { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Supplier Email")]
        public String SupplierEmail { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your established date")]
        [Display(Name = "Established Date")]
        [DataType(DataType.Date)]
        public DateTime EstablishedDate { get; set; }

        [Required(ErrorMessage = "Please enter if you are a preferred supplier")]
        [Display(Name = "Preferred Supplier")]
        public Boolean PreferredSupplier { get; set; }

        [Display(Name = "Notes")]
        public String Notes { get; set; }

        //navagation properties

        public List<ProductSupplier> ProductSuppliers { get; set; }

        public Supplier()
        {
            if (ProductSuppliers == null)
            {
                ProductSuppliers = new List<ProductSupplier>();
            }
        }
    }
}