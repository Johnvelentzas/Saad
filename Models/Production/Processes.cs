using System.ComponentModel.DataAnnotations;

namespace Models.Production
{
    public class Processes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public required ProcessesType Type { get; set; }

    }

    public enum ProcessesType
    {
        Customers,
        Orders,
        Products,
        Tasks,
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
        DeliverApt
    }
}
