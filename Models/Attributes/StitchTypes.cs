

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class StitchTypes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        public required string StitchTypeName { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? Comments { get; set; }
    }
}
