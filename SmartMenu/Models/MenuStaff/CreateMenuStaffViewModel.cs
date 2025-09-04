using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuStaff
{
    public class CreateMenuStaffViewModel
    {
        [Required(ErrorMessage = "Menu is required.")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }
    }
}
