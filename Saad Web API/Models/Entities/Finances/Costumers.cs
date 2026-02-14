namespace Saad_Web_API.Models.Entities.Finances
{
    public class Costumers
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? LastName { get; set; }
    }
}
