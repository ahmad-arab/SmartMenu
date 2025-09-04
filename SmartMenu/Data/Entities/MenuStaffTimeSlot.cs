using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuStaffTimeSlot
    {
        [Key]
        public int Id { get; set; }

        public int MenuStaffId { get; set; }
        public virtual MenuStaff MenuStaff { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; } // System.DayOfWeek

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
