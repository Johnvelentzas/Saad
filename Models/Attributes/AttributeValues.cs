using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class AttributeValues : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
