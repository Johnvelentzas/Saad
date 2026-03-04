using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class SewingTypes
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
