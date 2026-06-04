using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Processes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;
        public required ProcessesType Type { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }

    public enum ProcessesType
    {
        Customers,
        Orders,
        Products,
        Users,
        Models,
        Patterns,
        ProductCategories,
        PickUpApt,
        FoamFix,
        FoamAdapt,
        FoamGel,
        FoamAnatomical,
        CoverRemove,
        CustomPattern,
        Cut,
        Sew,
        Embroider,
        Bolt,
        Inspect,
        DeliverApt,
        Tasks,
        Foam,
        Calendar
    }
}
