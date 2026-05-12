using System.ComponentModel.DataAnnotations;

namespace LostAndFoundSystem.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [StringLength(200)]
        public string RoleDescription { get; set; }
    }
}