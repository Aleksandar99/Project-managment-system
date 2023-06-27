using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTestb1.Data;
using WebTestb1.Models;

namespace WebTestb1.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
       
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;

            _userManager = userManager;

        }

        private async Task<bool> CanAccessProjectAsync(int? id)
        {
            if (User == null || User.Identity == null || User.Identity.Name == null || !User.Identity.IsAuthenticated)
            {
                return false;
            }

            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            if (identityUser == null)
            {
                return false;
            }

            if (await _userManager.IsInRoleAsync(identityUser, "Admin"))
            {
                return true;
            }

            Project project = await _context.Project.FirstOrDefaultAsync(a => a.Id == id);

            if (project == null)
            {
                return false;
            }

            if (!_context.ProjectTask.Where(a => a.IsDeleted == false).Where(a => identityUser.UserName == a.Username).Select(a => a.Project).Any(a => a.Id == id))
            {
                return false;
            }

            return true;

        }

        private async Task<List<Project>> GetWorkerProjectsAsync()
        {
            List<Project> projects = await _context.ProjectTask.Where(a => a.IsDeleted == false).Where(a => User.Identity.Name == a.Username).Select(a => a.Project).Distinct().ToListAsync();

            List<Project> result = new List<Project>();

            foreach (Project project in projects)
            {
                if (result.Any(a => a.Id == project.Id))
                {
                    continue;
                }

                result.Add(project);
            }

            return result;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {   

            return View(User.IsInRole("Admin") ? await _context.Project.Where(a => a.IsDeleted == false).ToListAsync() : await GetWorkerProjectsAsync());

        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            if (!(await CanAccessProjectAsync(id)))
            {
                return NotFound();
            }

            _context.Entry(project).Collection(a => a.ProjectTasks).Load();

            project.ProjectTasks = new List<ProjectTask>(project.ProjectTasks.Where(a => a.IsDeleted == false));

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,From,To")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,From,To")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);

            project.IsDeleted = true;

            _context.Entry(project).Collection(a => a.ProjectTasks).Load();

            foreach (var item in project.ProjectTasks)
            {
                var value = _context.ProjectTask.FirstOrDefault(a => a.Id == item.Id);

                value.IsDeleted = true;

                _context.SaveChanges();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
