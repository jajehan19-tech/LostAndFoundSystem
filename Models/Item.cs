using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LostAndFoundSystem.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public DateTime DateReported { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public string? PhotoPath { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}