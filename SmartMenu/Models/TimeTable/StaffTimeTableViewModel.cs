namespace SmartMenu.Models.TimeTable
{
    public class StaffTimeTableViewModel
    {
        public int MenuStaffId { get; set; }
        public List<DayTimeSlot> TimeSlots { get; set; } = new();

        public class DayTimeSlot
        {
            public DayOfWeek Day { get; set; }
            public List<TimeRange> Ranges { get; set; } = new();
        }

        public class TimeRange
        {
            public TimeSpan Start { get; set; }
            public TimeSpan End { get; set; }
        }
    }
}
