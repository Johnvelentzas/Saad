
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Attributes
{
    internal class PatternAreas
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Pattern")]
        public required int PatternId { get; set; }
        public required string Name { get; set; }
    }
}
