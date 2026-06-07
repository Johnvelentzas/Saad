

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class Brands : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        public required string BrandName { get; set; }
        public string? ImageUrl { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? Comments { get; set; }
    }
}
