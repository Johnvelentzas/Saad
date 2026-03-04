

using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    internal class Models
    {
        [Key]
        public int Id { get; set; }
        public required string ModelName { get; set; }
    }
}
