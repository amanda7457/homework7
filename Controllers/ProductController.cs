

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HW7_Barron_Amanda.DAL;
using HW7_Barron_Amanda.Models;
using HW7_Barron_Amanda.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace HW7_Barron_Amanda.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Product
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(c => c.ProductSuppliers).ThenInclude(c => c.Supplier).FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(int[] SelectedSuppliers, [Bind("ProductID,SKU,ProductName,ProductPrice,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {

                //Generate the next SKU number

                product.SKU = GenerateSKU.GetNextSKU(_context);


                _context.Add(product);
                await _context.SaveChangesAsync();

                //add connections to suppliers
                //first, find the product you just added
                Product dbProduct = _context.Products.FirstOrDefault(c => c.SKU == product.SKU);

                //loop through selected suppliers and add them
                foreach (int i in SelectedSuppliers)
                {
                    Supplier dbSup = _context.Suppliers.Find(i);
                    ProductSupplier cd = new ProductSupplier();
                    cd.Product = dbProduct;
                    cd.Supplier = dbSup;
                    _context.ProductSuppliers.Add(cd);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }

            //repopulate the Viewbag
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.ProductSuppliers).ThenInclude(c => c.Product).FirstOrDefault(c => c.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, int[] SelectedSuppliers, [Bind("ProductID,SKU,ProductName,ProductPrice,ProductDescription")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    Product dbProduct = _context.Products.Include(c => c.ProductSuppliers)
                                                .ThenInclude(c => c.Supplier)
                                                .FirstOrDefault(c => c.ProductID == product.ProductID);

                    dbProduct.ProductPrice = product.ProductPrice;
                    dbProduct.ProductName = product.ProductName;
                    dbProduct.ProductDescription = product.ProductDescription;

                    _context.Update(dbProduct);
                    await _context.SaveChangesAsync();

                    //edit supplier/product relationships

                    //loop through selected suppliers and find ones that need to be removed
                    List<ProductSupplier> SupsToRemove = new List<ProductSupplier>();
                    foreach (ProductSupplier cd in dbProduct.ProductSuppliers)
                    {
                        if (SelectedSuppliers.Contains(cd.Supplier.SupplierID) == false)
                        //list of selected depts does not include this departments id
                        {
                            SupsToRemove.Add(cd);
                        }
                    }

                    //remove suppliers you found in list above - this has to be 2 separate steps because you can't 
                    //iterate over a list that you are removing items from
                    foreach (ProductSupplier cd in SupsToRemove)
                    {
                        _context.ProductSuppliers.Remove(cd);
                        _context.SaveChanges();
                    }

                    //now add the suppliers that are new
                    foreach (int i in SelectedSuppliers)
                    {
                        if (dbProduct.ProductSuppliers.Any(c => c.Supplier.SupplierID == i) == false)
                        //this supplier has not yet been added
                        {
                            //create a new course department
                            ProductSupplier cd = new ProductSupplier();

                            //connect the new course department to the department and course
                            cd.Supplier = _context.Suppliers.Find(i);
                            cd.Product = dbProduct;

                            //update the database
                            _context.ProductSuppliers.Add(cd);
                            _context.SaveChanges();
                        }
                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            //repop Viewbags
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }


        //THIS IS YO VIEWBAG
        private MultiSelectList GetAllSuppliers()
        {
            List<Supplier> allSup = _context.Suppliers.ToList();
            MultiSelectList supplierList = new MultiSelectList(allSup, "SupplierID", "SupplierName");
            return supplierList;
        }

        //overload for editing suppliers
        private MultiSelectList GetAllSuppliers(Product product)
        {
            //create a list of all the departments
            List<Supplier> allSup = _context.Suppliers.ToList();

            //create a list for the department ids that this course already belongs to
            List<int> currentSups = new List<int>();

            //loop through all the details to find the list of current departments
            foreach (ProductSupplier cd in product.ProductSuppliers)
            {
                currentSups.Add(cd.Supplier.SupplierID);
            }

            //create the multiselect with the overload for currently selected items
            MultiSelectList supplierList = new MultiSelectList(allSup, "SupplierID", "SupplierName", currentSups);

            //return the list
            return supplierList;
        }
    }
}



