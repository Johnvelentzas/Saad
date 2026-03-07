
using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class FabricPatterns : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
