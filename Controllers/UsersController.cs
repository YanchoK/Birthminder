using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Birthminder.Data;
using Birthminder.Models;
using Birthminder.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Birthminder.Controllers
{
    public class UsersController : Controller
    {
        private readonly birthminderContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(birthminderContext context, UserManager<User> userManager,
                              SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var birthminderContext = _context.Users.Include(u => u.Team); 
            return View(await birthminderContext.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        public async Task<IActionResult> Colleagues(int? id)
        {
            User[] colleagues = await _context.Users.Where(user => user.TeamId == id).ToArrayAsync();
            return View(colleagues);
        }

        public IActionResult Register()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View();
        }

        public IActionResult UserInformation()
        {
            Team[] teams = _context.Teams.ToArray();
            return View(teams);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Id", user.TeamId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,FullName,BirthDate,IsDeleted,TeamId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Id", user.TeamId);
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LogInUserFormModel user)
        {

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }   
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterFormModel user)
        {
            if (ModelState.IsValid)
            {
                User appuser = new User
                {
                    Email = user.Email,
                    UserName = user.Email,
                    PasswordHash = user.Password,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    
                    TeamId = user.TeamId
                };

                var result = await _userManager.CreateAsync(appuser, user.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(appuser, isPersistent: false);
                    await _userManager.AddToRoleAsync(appuser, Roles.Employee.ToString());
                    await _signInManager.SignOutAsync();

                    return RedirectToAction("Login", "Users");
                }
                else
                {

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", user.TeamId);
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View();
        }


        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RegisterFormModel user)
        {
            if (ModelState.IsValid)
            {
                User dataUser = new User()
                {
                    Email = user.Email,
                    PasswordHash = user.Password,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    TeamId = user.TeamId
                };

                _context.Users.Add(dataUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Id", user.TeamId);
            return View(user);
        }
    }
}
