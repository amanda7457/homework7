using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HW7_Barron_Amanda.DAL;
using HW7_Barron_Amanda.Models;
using HW7_Barron_Amanda.Utilities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace HW7_Barron_Amanda.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Orders.Include(r => r.OrderDetails).ToListAsync());
            List<Order> Orders = new List<Order>();
            if (User.IsInRole("Customer"))
            {
                Orders = _context.Orders.Where(o => o.Customer.UserName == User.Identity.Name).Include( o => o.OrderDetails).ToList();
            }
            else //user is manager and can see all orders
            {
                Orders = _context.Orders.Include(o => o.OrderDetails).ToList();

            }
            return View(Orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(r => r.OrderDetails).ThenInclude(r => r.Product).FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,OrderNumber,OrderDate,OrderNotes")] Order order)
        {

            order.OrderNumber = GenerateNextOrderNumber.GetNextOrderNumber(_context);
            order.OrderDate = System.DateTime.Today;


            if (ModelState.IsValid)
            {
                string userName = User.Identity.Name;
                AppUser user = _context.Users.FirstOrDefault(u => u.UserName == userName);
                order.Customer = user;
                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("AddToOrder", new { id = order.OrderID }); ;
            }
            return View(order);
        }

        [Authorize]
        public IActionResult AddToOrder(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "You must specify an order to add!" });
            }
            Order ord = _context.Orders.Find(id);
            if (ord == null)
            {
                return View("Error", new string[] { "Order not found!" });
            }

            OrderDetail od = new OrderDetail() { Order = ord };

            ViewBag.AllProducts = GetAllProducts();
            return View("AddToOrder", od);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddToOrder(OrderDetail od, int SelectedProduct)
        {
            //find the course associated with the selected course id
            Product product = _context.Products.Find(SelectedProduct);

            //set the registration detail's course equal to the course we just found
            od.Product = product;

            //find the registration based on the id
            Order ord = _context.Orders.Find(od.Order.OrderID);

            //set the registration detail's registration equal to the registration we just found
            od.Order = ord;

            //set the course fee for this detail equal to the current course fee
            od.OrderPrice = od.Product.ProductPrice;

            //add total fees
            od.ExtendedPrice = od.OrderQuantity * od.OrderPrice;

            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(od);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = od.Order.OrderID });
            }
            ViewBag.AllProducts = GetAllProducts();
            return View(od);
        }





        // GET: Order/Edit/5
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders.Include(r => r.OrderDetails)
                                            .ThenInclude(r => r.Product)
                                        .FirstOrDefault(r => r.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //QUESTION ------------- DOESNT LOOK LIKE THE CODE ON GITHUB
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public IActionResult Edit(Order order)
        {

            //Find the related registration in the database
            Order DbOrd = _context.Orders.Find(order.OrderID);

            //Update the notes
            DbOrd.OrderNotes = order.OrderNotes;

            //Update the database
            _context.Orders.Update(DbOrd);

            //Save changes
            _context.SaveChanges();

            //Go back to index
            return RedirectToAction(nameof(Index));
        }


        // remove for order was right here
        // ------------------------------

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        [Authorize]
        public IActionResult RemoveFromOrder(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "You need to specify an order id" });
            }

            Order ord = _context.Orders.Include(r => r.OrderDetails).ThenInclude(r => r.Product).FirstOrDefault(r => r.OrderID == id);

            if (ord == null || ord.OrderDetails.Count == 0)//registration is not found
            {
                return RedirectToAction("Details", new { id = id });
            }

            //pass the list of order details to the view
            return View(ord.OrderDetails);
        }


        private SelectList GetAllProducts()
        {
            List<Product> Products = _context.Products.ToList();
            SelectList allProducts = new SelectList(Products, "ProductID", "ProductName");
            return allProducts;
        }


    }
}

