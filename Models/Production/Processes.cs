using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Processes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

    }
}
