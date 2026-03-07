

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class Models : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required string ModelName { get; set; }
    }
}
