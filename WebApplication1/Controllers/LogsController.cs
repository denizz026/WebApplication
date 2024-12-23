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

        [HttpPost]
        public async Task LoginLog(string Mail, string Role, bool Status)
        {
            if (Role == "Admin")
            {
                if (Status)
                {
                    c.Logs.Add(new Logs
                    {
                        LogCategory = "AdminLogin",
                        Activity = "Admin " + Mail + " sisteme giris yaptı",
                        LogDate = DateTime.Now
                    });
                    await c.SaveChangesAsync();
                }
                else
                {
                    c.Logs.Add(new Logs
                    {
                        LogCategory = "AdminLogin",
                        Activity = Mail + " sisteme giris yapmaya çalıştı",
                        LogDate = DateTime.Now
                    });
                    await c.SaveChangesAsync();
                }

            }
            else if (Role == "User")
            {
                if (Status)
                {
                    c.Logs.Add(new Logs
                    {
                        LogCategory = "UserLogin",
                        Activity = "User " + Mail + " sisteme giris yaptı",
                        LogDate = DateTime.Now
                    });
                    await c.SaveChangesAsync();
                }
                else
                {
                    c.Logs.Add(new Logs
                    {
                        LogCategory = "UserLogin",
                        Activity = Mail + " sisteme giris yapmaya çalıştı",
                        LogDate = DateTime.Now
                    });
                    await c.SaveChangesAsync();
                }

            }
            else if (Role == "Seller")
            {
                if (Status)
                {
                    c.Logs.Add(new Logs
                    {
                        LogCategory = "SellerLogin",
                        Activity = "Seller " + Mail + " sisteme giris yaptı",
                        LogDate = DateTime.Now
                    });
                    await c.SaveChangesAsync();
                }
                else
                {
                    c.Logs.Add(new Logs
                    {
                        LogCategory = "SellerLogin",
                        Activity = Mail + " sisteme giris yapmaya çalıştı",
                        LogDate = DateTime.Now
                    });
                    await c.SaveChangesAsync();
                }
            }
        }

        public async Task AddLog(string mail, string Category, string Activity)
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