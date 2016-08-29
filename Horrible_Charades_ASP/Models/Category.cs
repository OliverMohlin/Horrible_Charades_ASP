namespace Horrible_Charades_ASP.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string BelongsTo { get; set; }
    }
}