using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Saad_Web_API.Models.Entities.Finances
{
    public class Costumers
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Telephone { get; set; }
        public string? TaxNumber { get; set; }
        //public required enum('Retail','Whoesale') CustomerType {  get; set; }
        //TODO add timestamp
    }
}
