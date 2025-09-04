namespace SmartMenu.Data.Entities
{
    public class LanguageText
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Text { get; set; }

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }

    }
}
