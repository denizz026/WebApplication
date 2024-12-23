using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Token
    {
        [Key]
        public int TokenId { get; set; } 
        public string Value { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        // Foreign Key tanımı
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; }
    }
}