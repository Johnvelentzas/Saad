
using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class Fabrics : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
