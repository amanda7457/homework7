using HW7_Barron_Amanda;
using HW7_Barron_Amanda.DAL;
using System;
using System.Linq;


namespace HW7_Barron_Amanda.Utilities
{
    public class GenerateSKU
    {
        public static Int32 GetNextSKU(AppDbContext db)
        {
            Int32 intMaxSKU; //the current maximum course number
            Int32 intNextSKU; //the course number for the next class

            if (db.Products.Count() == 0) //there are no courses in the database yet
            {
                intMaxSKU = 5000; //course numbers start at 3001
            }
            else
            {
                intMaxSKU = db.Products.Max(c => c.SKU); //this is the highest number in the database right now
            }

            //add one to the current max to find the next one
            intNextSKU = intMaxSKU + 1;

            //return the value
            return intNextSKU;
        }
    }
}
