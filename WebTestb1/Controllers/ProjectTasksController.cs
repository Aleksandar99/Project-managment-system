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
    public class ProjectTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public ProjectTasksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        private async Task<bool> CanAccessProjectTaskAsync(int? id)
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

            ProjectTask projectTask = await _context.ProjectTask.FirstOrDefaultAsync(a => a.Id == id);

            if (projectTask == null)
            {
                return false;
            }

            if (projectTask.Username != identityUser.UserName)
            {
                return false;
            }

            return true;

        }

        // GET: ProjectTasks
        public async Task<IActionResult> Index()
        {
            return View(User.IsInRole("Admin") ? await _context.ProjectTask.Where(a => a.IsDeleted == false).ToListAsync() : await _context.ProjectTask.Where(a => a.IsDeleted == false).Where(a => User.Identity.Name == a.Username).ToListAsync());
        }

        // GET: ProjectTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectTask == null)
            {
                return NotFound();
            }




            if (!(await CanAccessProjectTaskAsync(id)))
            {
                return NotFound();
            }




            return View(projectTask);
        }

        [Authorize(Roles = "Admin")]
        // GET: ProjectTasks/Create
        public IActionResult Create()
        {
            ProjectTask projectTask = new ProjectTask();

            projectTask.Projects = new SelectList(_context.Project.Where(a => !a.IsDeleted), "Id", "Name", projectTask.ProjectId);

            projectTask.Usernames = new SelectList(_context.Users, "UserName", "UserName", projectTask.UsernameId);

            return View(projectTask);
        }

        // POST: ProjectTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,From,To,Username,ProjectTaskState,Projects,ProjectId,UsernameId")] ProjectTask projectTask)
        {
            if (ModelState.IsValid)
            {
                projectTask.Username = projectTask.UsernameId;

                Project project = _context.Project.FirstOrDefault(a => a.Id == projectTask.ProjectId);

                _context.Entry(project).Collection(a => a.ProjectTasks).Load();

                project.ProjectTasks.Add(projectTask);

                _context.Entry(project).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            projectTask.Projects = new SelectList(_context.Project.Where(a => !a.IsDeleted), "Id", "Name", projectTask.ProjectId);

            projectTask.Usernames = new SelectList(_context.Users, "UserName", "UserName", projectTask.UsernameId);

            return View(projectTask);
        }

        [Authorize(Roles = "Admin")]
        // GET: ProjectTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTask.FindAsync(id);
            if (projectTask == null)
            {
                return NotFound();
            }

            projectTask.UsernameId = projectTask.Username;

            return View(projectTask);
        }

        // POST: ProjectTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,From,To,Username,UsernameId,ProjectTaskState")] ProjectTask projectTask)
        {
            if (id != projectTask.Id)
            {
                return NotFound();
            }

            //projectTask.UsernameId

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectTaskExists(projectTask.Id))
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
            return View(projectTask);
        }

        [Authorize(Roles = "Admin")]
        // GET: ProjectTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectTask == null)
            {
                return NotFound();
            }

            return View(projectTask);
        }

        [Authorize(Roles = "Admin")]
        // POST: ProjectTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectTask = await _context.ProjectTask.FindAsync(id);
            projectTask.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectTaskExists(int id)
        {
            return _context.ProjectTask.Any(e => e.Id == id);
        }
    }
}
