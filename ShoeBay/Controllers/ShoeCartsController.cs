using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeBay.Data;
using ShoeBay.Models;
using System.IO;
using static NuGet.Packaging.PackagingConstants;


namespace ShoeBay.Controllers
{
    public class ShoeCartsController : Controller
    {
        private readonly ShoeBayDBContext _context;
        //private readonly Shoe _shoes;
        //private readonly ShoeOrderHistory _ShoeCHist;

        public ShoeCartsController(ShoeBayDBContext context )
        {
            _context = context;
            //_shoes = shoe;
            //_ShoeCHist = orderHist;
        }

        [HttpPost]
        public ActionResult SaveCartItems(int id)
        {
            var shoeCart = _context.Shoes.FirstOrDefault(m => m.Id == id);
            ShoeCart cart = new ShoeCart();
            cart.CreatedDate = DateTime.Now;
            cart.ShoeId = id;
            cart.TransactionStatus = "P"; //pending to check out
            cart.Amount = Convert.ToDecimal(shoeCart.Cost);
            cart.Email = User.Identity.Name;
            _context.Add(cart);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));


        }
        [HttpPost]
        public IActionResult CheckOut()
        {

            List<ShoeCart> cart = _context.ShoeOrders.Where(c => c.Email == User.Identity.Name && c.TransactionStatus=="P").ToList();
           
            foreach (var item in cart)
            {
                ShoeCart ordr = _context.ShoeOrders.Where(c => c.Id==item.Id).FirstOrDefault();
                if (ordr!=null)
                {
                    ordr.TransactionStatus = "C";
                    _context.Update(ordr);
                    _context.SaveChanges();
                }
              
                // var shoes = _context.Shoes.FirstOrDefault(m => m.Id == item.ShoeId);
                // ShoeOrderHistory hist = new ShoeOrderHistory();
                // hist.CreatedDate = DateTime.Now;
                // hist.ShoeId = shoes.Id;
                // hist.Amount = Convert.ToDecimal(item.Amount);
                //// Hist.TransactionStatus = "C"; //confirm order
                // hist.Email = User.Identity.Name;
                // _context.Add(hist);
                // _context.SaveChanges();



            }

            return RedirectToAction(nameof(Index));


        }
        [HttpPost]

        public IActionResult DeleteOrder(int Id)
        {
            ShoeCart ordr = _context.ShoeOrders.Where(c => c.Id ==c.Id ).FirstOrDefault();
            if (ordr != null)
            {
                ordr.TransactionStatus = "D";
                _context.Update(ordr);
                _context.SaveChanges();
            }

            //var ordr = _context.ShoeOrders.Where(c => c.Id == Id).FirstOrDefault();
            //_context.ShoeOrders.Remove(ordr);
            //_context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ProcessOrder(int id)
        {

            ShoeCart cart = _context.ShoeOrders.Where(c => c.Id== id).FirstOrDefault();

           
                var shoes = _context.Shoes.FirstOrDefault(m => m.Id == cart.ShoeId);
                OrderHistory hist = new OrderHistory();
                hist.CreatedDate = DateTime.Now;
                hist.ShoeId = shoes.Id;
                hist.CartOrderId = id;
                hist.Amount = Convert.ToDecimal(cart.Amount);
                hist.Email = User.Identity.Name;
                _context.Add(hist);
                _context.SaveChanges();


            ShoeCart ordr = _context.ShoeOrders.Where(c => c.Id == cart.Id).FirstOrDefault();
            if (ordr != null)
            {
                ordr.TransactionStatus = "A";
                _context.Update(ordr);
                _context.SaveChanges();
            }


            return RedirectToAction(nameof(GetOrders));


        }
        //[HttpGet]

        //public IActionResult Delete(int Id)
        //{
        //    var ordr = _context.ShoeOrders.Where(c=> c.Id ==Id).FirstOrDefault();
        //    _context.ShoeOrders.Remove(ordr);
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet]
        public IActionResult GoToCheckout()//,string desc, string amount, string imageurl
        {
            var odr = _context.ShoeOrders.Include(c=> c.Shoes).Where(c=> c.Email== User.Identity.Name && c.TransactionStatus=="P").ToList();
            return View(odr);
        }
        // GET: ShoeCarts
        public async Task<IActionResult> Index()
        {
            Shoe shoes = new Shoe();
            return _context.Shoes != null ?
                          View(await _context.Shoes.ToListAsync()) :
                          Problem("Entity set 'ShoeBayDBContext.Shoes'  is null.");
            //var shoeBayDBContext = _context.Shoes();
            //return View(await shoeBayDBContext.ToListAsync());
        }
        public async Task<IActionResult> GetOrders()
        {
            ShoeCart Cart = new ShoeCart();
            return _context.ShoeOrders != null ?
                          View(await _context.ShoeOrders.Where(c => c.TransactionStatus == "C").Include(c=> c.Shoes).ToListAsync()) :
                          Problem("Entity set 'ShoeBayDBContext.Shoes'  is null.");
            //var shoeBayDBContext = _context.Shoes();
            //return View(await shoeBayDBContext.ToListAsync());
        }
        // GET: ShoeCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ShoeOrders == null)
            {
                return NotFound();
            }

            var shoeCart = await _context.ShoeOrders
                .Include(s => s.Shoes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoeCart == null)
            {
                return NotFound();
            }

            return View(shoeCart);
        }

        // GET: ShoeCarts/Create
        public IActionResult Create()
        {
            ViewData["ShoeId"] = new SelectList(_context.Shoes, "Id", "Id");
            return View();
        }

        // POST: ShoeCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,ShoeId,Amount,CreatedDate")] ShoeCart shoeCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoeCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShoeId"] = new SelectList(_context.Shoes, "Id", "Id", shoeCart.ShoeId);
            return View(shoeCart);
        }

        // GET: ShoeCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ShoeOrders == null)
            {
                return NotFound();
            }

            var shoeCart = await _context.ShoeOrders.FindAsync(id);
            if (shoeCart == null)
            {
                return NotFound();
            }
            ViewData["ShoeId"] = new SelectList(_context.Shoes, "Id", "Id", shoeCart.ShoeId);
            return View(shoeCart);
        }

        // POST: ShoeCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,ShoeId,Amount,CreatedDate")] ShoeCart shoeCart)
        {
            if (id != shoeCart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoeCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoeCartExists(shoeCart.Id))
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
            ViewData["ShoeId"] = new SelectList(_context.Shoes, "Id", "Id", shoeCart.ShoeId);
            return View(shoeCart);
        }

        // GET: ShoeCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ShoeOrders == null)
            {
                return NotFound();
            }

            var shoeCart = await _context.ShoeOrders
                .Include(s => s.Shoes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoeCart == null)
            {
                return NotFound();
            }

            return View(shoeCart);
        }

        // POST: ShoeCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ShoeOrders == null)
            {
                return Problem("Entity set 'ShoeBayDBContext.ShoeOrders'  is null.");
            }
            var shoeCart = await _context.ShoeOrders.FindAsync(id);
            if (shoeCart != null)
            {
                _context.ShoeOrders.Remove(shoeCart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoeCartExists(int id)
        {
          return (_context.ShoeOrders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
