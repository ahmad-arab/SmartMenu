using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.User
{
    public class ChangeUserPasswordViewModel
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
