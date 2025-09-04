using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.TimeTable
{
    public class BulkStaffTimeTableViewModel
    {
        public int MenuId { get; set; }

        [Display(Name = "Staff Members")]
        public List<int> StaffIds { get; set; } = new();
        public List<StaffTimeTableViewModel.DayTimeSlot> TimeSlots { get; set; } = new();

        public List<SelectListItem> AvailableStaff { get; set; } = new();
    }
}
