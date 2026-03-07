using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Attributes
{
    public class Patterns : IEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Models")]
        public required int ModelId { get; set; }
        public required string Name { get; set; }
    }
}
