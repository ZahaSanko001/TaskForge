using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using TaskForge.Data;
using TaskForge.Models;
using TaskForge.ViewModels;

namespace TaskForge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = new DashboardViewModel
            {
                ProjectCount = await _context.Projects.CountAsync(),
                TotalTasks = await _context.TaskItems.CountAsync(),
                MyTasks = await _context.TaskItems.CountAsync(t => t.AssignedToUserId == userId),
                CompletedTasks = await _context.TaskItems.CountAsync(t => t.Status == TaskForge.Models.TaskStatus.Done)
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
