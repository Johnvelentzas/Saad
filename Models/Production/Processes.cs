using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Processes
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

    }
}
