using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class AttributeValues : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
