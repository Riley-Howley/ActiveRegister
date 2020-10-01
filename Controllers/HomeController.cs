using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Practice.Models;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BlogDataContext _dbcontext;

        public HomeController(ILogger<HomeController> logger, BlogDataContext dbcontext)
        {
            _logger = logger;
            this._dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            //This is the home page
            return View();
        }
        //Because that this is just posting the data you do not need to have parameters 
        public IActionResult Contact()
        {
            //Only need a where statement to validate data, In this case i am just getting the information
            //from the Database
            var temp = _dbcontext.phonebooks.ToList();
            return View(temp);
        }
        
        public async Task<IActionResult> RemoveContactAsync(int id)
        {
            try
            {
                var user = await _dbcontext.phonebooks.FindAsync(id);

                _dbcontext.phonebooks.Remove(user);

                await _dbcontext.SaveChangesAsync();

                return RedirectToAction("Contact");
            }
            catch (Exception e)
            {
                return RedirectToAction("Contact");
            }
        }

        public async Task<IActionResult> EditContact(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _dbcontext.phonebooks.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("EditContact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userToUpdate = await _dbcontext.phonebooks.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Phonebook>(userToUpdate, "", s => s.FirstName, s => s.LastName, s => s.Phone, s => s.Email, s => s.Address));
            {
                try
                {
                    await _dbcontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Contact));
                }
                catch (DbUpdateConcurrencyException /* ex */) 
                {
                    ModelState.AddModelError("", "Unable to save changes");
                }
            }
            return View(userToUpdate);


        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PostRegister(Phonebook phonebook)
        {
            if(!ModelState.IsValid)
            {
                return View(phonebook);
            }
            //Priority!
            _dbcontext.phonebooks.Update(phonebook);

            _dbcontext.SaveChanges();
            //await _dbcontext.SaveChangesAsync();
            return RedirectToAction("Contact");
            //return View("Contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
