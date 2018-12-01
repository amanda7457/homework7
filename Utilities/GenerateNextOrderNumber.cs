using HW7_Barron_Amanda.DAL;
using System;
using System.Linq;


namespace HW7_Barron_Amanda.Utilities
{
    public class GenerateNextOrderNumber
    {
        public static Int32 GetNextOrderNumber(AppDbContext db)
        {
            Int32 intMaxOrderNumber; //the current maximum course number
            Int32 intNextOrderNumber; //the course number for the next class

            if (db.Orders.Count() == 0) //there are no registrations in the database yet
            {
                intMaxOrderNumber = 10000; //registration numbers start at 101
            }
            else
            {
                intMaxOrderNumber = db.Orders.Max(c => c.OrderNumber); //this is the highest number in the database right now
            }

            //add one to the current max to find the next one
            intNextOrderNumber = intMaxOrderNumber + 1;

            //return the value
            return intNextOrderNumber;
        }

    }
}
