using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Logs
    {
        [Key]
        public int LogID { get; set; }
        public string LogCategory { get; set; }
        public string Activity { get; set; }
        public DateTime LogDate { get; set; }
    }
}