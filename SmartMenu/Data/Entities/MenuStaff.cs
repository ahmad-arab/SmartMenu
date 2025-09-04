using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuStaff
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public ICollection<MenuStaffTimeSlot> TimeSlots { get; set; } = new List<MenuStaffTimeSlot>();

        // Navigation to commands mapping
        public ICollection<MenuCommandStaff> MenuCommandStaffs { get; set; } = new List<MenuCommandStaff>();
    }
}
