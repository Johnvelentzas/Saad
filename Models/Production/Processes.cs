using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Processes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public required ProcessesType Type { get; set; }

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
