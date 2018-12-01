using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

//TODO: Change this namespace to match your project
namespace HW7_Barron_Amanda.Models
{
    public class AppUser : IdentityUser
    {
        //TODO: Put any additional fields that you need for your user here
        //Identity creates several of the important ones for you
        //Full documentation of the IdentityUser class can be found at
        //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identityuser?view=aspnetcore-1.1&viewFallbackFrom=aspnetcore-2.1

        //Here is an example of first name
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        //navgational properties
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public AppUser()
        {
            if (Orders == null)
            {
                Orders = new List<Order>();
            }

        }
    }
}