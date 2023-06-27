using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebTestb1.Data;
using WebTestb1.Models;

namespace WebTestb1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly UserManager<IdentityUser> _userManager;

        public WorkersController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;

            _signInManager = signInManager;

            _userManager = userManager;
        }

        // GET: Workers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Worker.Where(a => !a.IsDeleted).ToListAsync());
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Pass,ConfirmPass,FirstName,LastName,BirthDate,Position,HireDate,FireDate,WorkerStatus,EducationType,UserType")] Worker worker)
        {
            if (ModelState.IsValid)
            {

                var userWithEmail = await _userManager.FindByEmailAsync(worker.Email);
                if (userWithEmail != null && userWithEmail.Id != worker.UserId)
                {
                    // Email is already taken by another user
                    ModelState.AddModelError("Email", "Email is already taken.");

                    return View(worker);
                }

                var user = new IdentityUser { UserName = worker.Email, Email = worker.Email };

                var result = await _userManager.CreateAsync(user, worker.Pass);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.ToLower().Contains("password".ToLower()))
                        {
                            ModelState.AddModelError("Pass", error.Description);

                        }

                    }

                    return View(worker);
                }

                if (worker.UserType == UserType.Admin)
                {
                    await _userManager.AddToRoleAsync(user, worker.UserType.ToString());
                }

                (await _context.Users.FirstOrDefaultAsync(a => a.Email == worker.Email)).EmailConfirmed = true;

                worker.UserId = (await _userManager.FindByEmailAsync(worker.Email)).Id;

                _context.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate,Position,HireDate,FireDate,WorkerStatus,EducationType,UserType")] Worker worker)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var user = await _userManager.FindByIdAsync(worker.UserId);

                    await _userManager.SetEmailAsync(user, worker.Email);

                    string userType = worker.UserType.ToString();

                    if (userType == "Admin")
                    {
                        if (!(await _userManager.IsInRoleAsync(user, userType)))
                        {
                            await _userManager.AddToRoleAsync(user, userType);
                        }
                    }
                    else
                    {
                        if ((await _userManager.IsInRoleAsync(user, "Admin")))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Admin");
                        }
                    }

                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Id))
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
            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.Worker.FindAsync(id);

            await _userManager.UpdateSecurityStampAsync(await _userManager.FindByEmailAsync(worker.Email));

            worker.IsDeleted = true;

            List<ProjectTask> projectTasks = await _context.ProjectTask.Where(a => a.IsDeleted == false).Where(a => worker.Email == a.Username).ToListAsync();

            foreach (var item in projectTasks)
            {
                var value = _context.ProjectTask.FirstOrDefault(a => a.Id == item.Id);

                value.IsDeleted = true;

                _context.SaveChanges();
            }

            await _context.SaveChangesAsync();



            if (User.Identity.Name == worker.Email)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction("Index", "Home");

            }


            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
            return _context.Worker.Any(e => e.Id == id);
        }
    }
}
