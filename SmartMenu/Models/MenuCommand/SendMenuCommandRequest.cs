using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuCommand
{
    public class SendMenuCommandRequest
    {
        [Required]
        public int MenuCommandId { get; set; }
        public string? Identifier { get; set; }
        public string? CustomerMessage { get; set; }
        // ISO8601 string parses to DateTimeOffset in model binder
        [Required]
        public DateTimeOffset FiredAt { get; set; }
        public int? TimezoneOffsetMinutes { get; set; }
    }
}
