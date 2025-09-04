using SmartMenu.Data.Enums;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuStaff
{
    public class EditMenuStaffViewModel
    {
        [Required(ErrorMessage = "Menu staff ID is required.")]
        public int MenuStaffId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public bool IsAvailable { get; set; }
    }
}
