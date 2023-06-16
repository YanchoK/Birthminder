using Birthminder.Data;
using Birthminder.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Birthminder.Controllers
{
    public class HomeController : Controller
    {
        private readonly birthminderContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, birthminderContext context)
        {
            _logger = logger;
            _context = context;
        }
      

        public IActionResult WishList(int? id)
        {
            // if null load logged user id
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Wishes = _context.Wishes.Where(wish => wish.UserId == id).ToArray();
            ViewBag.User = _context.Users.Where(user => user.Id == id).First();

            return View();
        }
        public IActionResult Index()
        {
            //checkUsersDates();

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            Team[] teams = _context.Teams.ToArray();
            return View(teams);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    
    }
}
