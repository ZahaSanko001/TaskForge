using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskForge.Data;
using TaskForge.Models;
using TaskForge.ViewModels;

namespace TaskForge.Controllers
{
    [Authorize]
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsTaskOwner(TaskItems taskItems)
        {
            return taskItems.AssignedToUserId == User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: TaskItems
        public async Task<IActionResult> Index(int projectId)
        {
            var tasks = _context.TaskItems
                .Include(t => t.AssignedUser)
                .Where(t => t.ProjectId == projectId);

            ViewData["ProjectId"] = projectId;
            return View(await tasks.ToListAsync());
        }

        // GET: TaskItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItems = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItems == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && !IsTaskOwner(taskItems))
            {
                return Forbid();
            }

            return View(taskItems);
        }

        // GET: TaskItems/Create
        public IActionResult Create(int projectId)
        {
            var vm = new CreateTaskViewModels
            {
                ProjectId = projectId
            };
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", projectId);
            ViewData["AssignedToUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View(vm);
        }

        // POST: TaskItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskViewModels vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", vm.ProjectId);
                ViewData["AssignedToUserId"] = new SelectList(_context.Users, "Id", "Email", vm.AssignedToUserId);
                return View(vm);
            }

            var task = new TaskItems
            {
                Title = vm.Title,
                Description = vm.Description,
                DueDate = vm.DueDate,
                Status = (Models.TaskStatus)vm.Status,
                ProjectId = vm.ProjectId,
                AssignedToUserId = vm.AssignedToUserId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { projectId = vm.ProjectId });




            //if (ModelState.IsValid)
            //{
            //    _context.Add(taskItems);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction("Index", new {projectId = taskItems.ProjectId});
            //}
            //ViewData["AssignedToUserId"] = new SelectList(_context.Users, "Id", "Email", taskItems.AssignedToUserId);
            //return View(taskItems);
        }

        // GET: TaskItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItems = await _context.TaskItems.FindAsync(id);
            if (taskItems == null)
            {
                return NotFound();
            }

            if(!User.IsInRole("Admin") && !IsTaskOwner(taskItems))
            {
                return Forbid();
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", taskItems.ProjectId);
            return View(taskItems);
        }

        // POST: TaskItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,DueDate,ProjectId,AssignedToUserId")] TaskItems taskItems)
        {
            if (id != taskItems.Id)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && !IsTaskOwner(taskItems))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItems);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemsExists(taskItems.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", taskItems.ProjectId);
            return View(taskItems);
        }

        // GET: TaskItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItems = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItems == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && !IsTaskOwner(taskItems))
            {
                return Forbid();
            }

            return View(taskItems);
        }

        // POST: TaskItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItems = await _context.TaskItems.FindAsync(id);
            if (taskItems != null)
            {
                _context.TaskItems.Remove(taskItems);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemsExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}
