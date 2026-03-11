using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Attributes
{
    public class TaskAtributes : IEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Tasks")]
        public required int TaskId { get; set; }
        [ForeignKey("AttributeValues")]
        public required int AttributeId { get; set; }
    }
}
