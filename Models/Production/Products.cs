using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Production
{
    public class Products : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDraft { get; set; } = false;
        public int FromId { get; set; } = 0;

        public string? ImageUrl { get; set; }

        //Foreign Keys
        [ForeignKey("Orders")]
        public required int OrderId { get; set; }
        [ForeignKey("ProductCategories")]
        public required int CategoryId { get; set; }
        [ForeignKey("Models")]
        public int ModelId { get; set; }
        [ForeignKey("Brands")]
        public int BrandId { get; set; }
        [ForeignKey("Patterns")]
        public int PatternId { get; set; }
        [ForeignKey("StitchTypes")]
        public int StitchTypeId { get; set; }
        [ForeignKey("YarnColors")]
        public int FirstYarnColorId { get; set; }
        [ForeignKey("YarnColors")]
        public int SecondYarnColorId { get; set; }
        [ForeignKey("Fabrics")]
        public int AFabricId { get; set; }
        [ForeignKey("Fabrics")]
        public int? BFabricId { get; set; }
        [ForeignKey("Fabrics")]
        public int? CFabricId { get; set; }
        [ForeignKey("Fabrics")]
        public int? DFabricId { get; set; }


        //Product Options
        public bool HasDropOffApt { get; set; } = false;
        public bool HasTestTryApt { get; set; } = false;
        public bool HasPickUpApt { get; set; } = false;
        public bool HasMultipleFabrics { get; set; } = false;
        public bool HasMultipleYarnColors { get; set; } = false;
        public bool HasCustomPatternTask { get; set; } = false;
        public bool HasEmbroideryTask { get; set; } = false;
        public bool HasRipTask { get; set; } = false;
        public bool HasFoamTask { get; set; } = false;
        public bool HasGelTask { get; set; } = false;
        public bool HasBoltTask { get; set; } = false;


        //Product Attributes
        public FoamType FoamType { get; set; } = FoamType.None;
        public RipAction RipAction { get; set; } = RipAction.None;


        //Status
        public required bool IsCompleted { get; set; } = false;
        public bool HasStartedManufacturing { get; set; } = false;

        //Time Details
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        public DateTime? DropOffApt { get; set; }
        [DataType(DataType.Date)]
        public DateTime? TestTryApt { get; set; }
        [DataType(DataType.Date)]
        public DateTime? PickUpApt { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExpectedStartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExpectedFinishDate { get; set; }

        //Comments
        public string? Comments { get; set; }
    }

    public enum FoamType
    {
        None,
        FoamFix,
        FoamAdapt,
        FoamAnatomical
    }

    public enum RipAction
    {
        None,
        Keep,
        ThrowAway
    }
}
