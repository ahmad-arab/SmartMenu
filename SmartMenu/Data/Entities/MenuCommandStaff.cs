using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuCommandStaff
    {
        [Key]
        public int Id { get; set; }

        public int MenuCommandId { get; set; }
        public virtual MenuCommand MenuCommand { get; set; }

        public int MenuStaffId { get; set; }
        public virtual MenuStaff MenuStaff { get; set; }
    }
}
