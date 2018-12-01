using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HW7_Barron_Amanda.DAL;
using HW7_Barron_Amanda.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace HW7_Barron_Amanda.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }


        // GET: OrderDetail/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(OrderDetail orderDetail)
        {
            //Find the related registration detail in the database
            OrderDetail DbOrdDet = _context.OrderDetails
                                        .Include(r => r.Product)
                                        .Include(r => r.Order)
                                        .FirstOrDefault(r => r.OrderDetailID ==
                                                            orderDetail.OrderDetailID);

            //update the related fields
            DbOrdDet.OrderQuantity = orderDetail.OrderQuantity;
            DbOrdDet.OrderPrice = DbOrdDet.Product.ProductPrice;
            DbOrdDet.ExtendedPrice = DbOrdDet.OrderPrice * DbOrdDet.OrderQuantity;

            //update the database
            if (ModelState.IsValid)
            {
                _context.OrderDetails.Update(DbOrdDet);
                _context.SaveChanges();
            }

            //return to the order details
            return RedirectToAction("Details", "Order", new { id = DbOrdDet.Order.OrderID });
        }


        // GET: OrderDetail/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            Order ord = _context.Orders.FirstOrDefault(r => r.OrderDetails.Any(rd => rd.OrderDetailID == id));
            return RedirectToAction("Details", "Orders", new { id = id });

        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }
    }
}
