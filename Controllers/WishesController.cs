using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Birthminder.Data;
using System.Text.RegularExpressions;
using Birthminder.Models.Wishes;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Birthminder.Models;
using Microsoft.AspNetCore.Authorization;

namespace Birthminder.Controllers
{
    public class WishesController : Controller
    {
        private readonly birthminderContext _context;
        private IWebHostEnvironment webHostEnvironment;

        public WishesController(birthminderContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: Wishes
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var birthminderContext = _context.Wishes.Include(w => w.User);
            return View(await birthminderContext.ToListAsync());
        }

        // GET: Wishes/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wishes
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wish == null)
            {
                return NotFound();
            }

            return View(wish);
        }

        // GET: Wishes/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Wishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Link,Description,PriorityIndex,Gifted,IsDeleted,UserId,ImageLink")] Wish wish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", wish.UserId);
            return View(wish);
        }
        [Authorize(Roles = "Employee")]
        public IActionResult MyWishList()
        {
            try
            {
                int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // ViewBag.FullName = "a" + User.FindFirstValue("FullName");

                User user = _context.Users.Find(id);
                if (user != null)
                {
                    Team team = _context.Teams.Find(user.TeamId);
                    if (team == null)
                        return NotFound();

                    ViewBag.Team = team;
                    ViewBag.User = user;
                    ViewBag.Wishes = _context.Wishes.Where(wish => wish.UserId == id && wish.IsDeleted == false).OrderBy(wish => wish.PriorityIndex);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return View();
        }


        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> New(WishFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    string uniqueFileName = UploadedFile(model);
                    Wish wish = new Wish
                    {
                        Description = model.Description,
                        ImageLink = uniqueFileName,
                        Title = model.Title,
                        Link = model.Link,
                        UserId = id,
                    };
                    _context.Add(wish);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return NotFound();
                }
            }
            return View();
        }
        [Authorize(Roles = "Employee")]
        public IActionResult WishAdd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> WishAdd(WishFormModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                Wish wish = new Wish
                {
                    Description = model.Description,
                    ImageLink = uniqueFileName,
                    Title = model.Title,
                    Link = model.Link,
                    UserId = userId,
                    PriorityIndex = _context.Wishes.Where(wish => wish.UserId == userId && wish.IsDeleted == false).Count(),
                };
                _context.Add(wish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyWishList));
            }
            return View();
        }

        public IActionResult UsersWish(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                ViewBag.Wish = _context.Wishes.Where(wish => wish.Id == id).First();
            }
            catch
            {
                return NotFound();
            }
            return View();
        }

        private string UploadedFile(WishFormModel model)
        {
            string uniqueFileName = null;

            if (model.WishImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "userImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.WishImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.WishImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [Authorize(Roles = "Employee")]
        public IActionResult Wishlist(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                ViewBag.Wishes = _context.Wishes.Where(wish => wish.UserId == id && wish.IsDeleted == false).ToArray().OrderBy(wish => wish.PriorityIndex);

                User user = _context.Users.Where(user => user.Id == id).First();

                if (user != null)
                {
                    ViewBag.User = _context.Users.Where(user => user.Id == id).First();
                    ViewBag.TeamName = _context.Teams.Where(team => user.TeamId == team.Id).First().Name;
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }
            return View();
        }

        // POST: Wishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Link,Description,PriorityIndex,Gifted,IsDeleted,UserId,ImageLink")] Wish wish)
        {
            if (id != wish.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishExists(wish.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", wish.UserId);
            return View(wish);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> WishEdit(int id, WishFormModel wishModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Wish wish = _context.Wishes.Find(id);
                    wish.Description = wishModel.Description;
                    wish.Link = wishModel.Link;
                    wish.Title = wishModel.Title;
                    wish.ImageLink = wishModel.ImagePath == null ? UploadedFile(wishModel) : (wishModel.WishImage == null ? wishModel.ImagePath : (getImageName(wishModel.ImagePath).ToLower().Equals(wishModel.WishImage.FileName.ToLower()) ? wishModel.ImagePath : UploadedFile(wishModel)));

                    _context.Update(wish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishExists(id))
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(MyWishList));
            }
            return View(wishModel);
        }

        private string getImageName(string imagePath)
        {
            var imagePathArr = imagePath.Split("_");
            return imagePathArr[imagePathArr.Length - 1];
        }

        // GET: Wishes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wishes.FindAsync(id);
            if (wish == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", wish.UserId);
            return View(wish);
        }
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> WishEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wishes.FindAsync(id);
            var wishModel = new WishFormModel()
            {
                Description = wish.Description,
                Link = wish.Link,
                Title = wish.Title,
            };
            if (wish == null || wishModel == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            ViewBag.ImageLink = wish.ImageLink;
            return View(wishModel);
        }

        // GET: Wishes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wishes
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wish == null)
            {
                return NotFound();
            }

            return View(wish);
        }

        // POST: Wishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wish = await _context.Wishes.FindAsync(id);
            _context.Wishes.Remove(wish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Wishes/Delete/5
        [HttpPost, ActionName("WishDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> WishDelete(int id)
        {
            Wish delWish = _context.Wishes.Find(id);

            Wish[] wishes = _context.Wishes.Where(wish => wish.UserId == delWish.UserId && delWish.PriorityIndex < wish.PriorityIndex && wish.IsDeleted == false).OrderBy(wish => wish.PriorityIndex).ToArray();
            for (int i = 0; i < wishes.Length; i++)
            {
                wishes[i].PriorityIndex--;

                _context.Update(wishes[i]);
            }



            delWish.IsDeleted = true;

            _context.Update(delWish);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyWishList));
        }

        private bool WishExists(int id)
        {
            return _context.Wishes.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Members()
        {

            try
            {
                int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                User user = _context.Users.First(u => u.Id == id);


                if (user != null)
                {
                    var context = _context.Users.Where(u => u.TeamId == user.TeamId).Include(u => u.Team).OrderBy(x => x.BirthDate.Date);
                    ViewBag.TeamName = context.First().Team.Name;
                    return View(await context.ToListAsync());
                }
                else
                {
                    return NotFound(); // try to get logged user's id before returning 404
                }
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> IncreasePriority(int id)
        {
            Wish mainWish = _context.Wishes.Find(id);
            if (mainWish.PriorityIndex <= 0)
            {
                return RedirectToAction(nameof(MyWishList)); ;
            }

            Wish[] wishes = await _context.Wishes.Where(wish => wish.UserId == mainWish.UserId && wish.IsDeleted == false && wish.PriorityIndex == (mainWish.PriorityIndex - 1)).ToArrayAsync(); ;
            Wish prevWish = wishes[0];

            mainWish.PriorityIndex--;
            prevWish.PriorityIndex++;

            _context.Update(prevWish);
            _context.Update(mainWish);


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyWishList));
        }


        public async Task<IActionResult> DecreasePriority(int id)
        {
            Wish mainWish = _context.Wishes.Find(id);


            Wish[] wishes = await _context.Wishes.Where(wish => wish.UserId == mainWish.UserId && wish.IsDeleted == false && wish.PriorityIndex == (mainWish.PriorityIndex + 1)).ToArrayAsync(); ;

            if (wishes.Length == 0)
            {
                return RedirectToAction(nameof(MyWishList)); ;
            }

            Wish prevWish = wishes[0];

            mainWish.PriorityIndex++;
            prevWish.PriorityIndex--;

            _context.Update(prevWish);
            _context.Update(mainWish);


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyWishList));
        }


    }
}
