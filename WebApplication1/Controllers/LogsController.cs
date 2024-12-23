using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LogsController : Controller
    {
        private readonly AppDbContext c;

        public LogsController(AppDbContext context)
        {
            c = context;
        }
        [HttpGet]
        public IActionResult Loginlog()
        {
            return View(c.Logs.ToList());
        }

        public async Task AddLog( string Category, string Activity)
        {
            c.Logs.Add(new Logs
            {
                LogCategory = Category,
                Activity = Activity,
                LogDate = DateTime.Now
            });
            await c.SaveChangesAsync();
        }

        [HttpPost]
        public async Task DeleteLog(int id)
        {
            var log = c.Logs.Find(id);
            if (log != null)
            {
                c.Logs.Remove(log);
                c.SaveChanges();
            }
        }
    }
}