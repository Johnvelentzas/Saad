using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models.Attributes;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Messages;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views.DetailsViews;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class ProductsViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private string _imageUrl = String.Empty;
        [ObservableProperty]
        private bool _isComplete = false;
        [ObservableProperty]
        private string _comments = String.Empty;

        [ObservableProperty]
        private Orders? _order;
        [ObservableProperty]
        private int _totalProductsCount = 0;
        [ObservableProperty]
        private ObservableCollection<Orders> _orderOptions = new ObservableCollection<Orders>();
        [ObservableProperty]
        private ProductCategories? _category;
        [ObservableProperty]
        private ObservableCollection<ProductCategories> _categoryOptions = new ObservableCollection<ProductCategories>();
        [ObservableProperty]
        private Brands? _brand;
        [ObservableProperty]
        private ObservableCollection<Brands> _brandOptions = new ObservableCollection<Brands>();
        [ObservableProperty]
        private Models.Attributes.Models? _model;
        [ObservableProperty]
        private ObservableCollection<Models.Attributes.Models> _modelOptions = new ObservableCollection<Models.Attributes.Models>();
        [ObservableProperty]
        private Patterns? _pattern;
        [ObservableProperty]
        private ObservableCollection<Patterns> _patternOptions = new ObservableCollection<Patterns>();
        [ObservableProperty]
        private StitchTypes? _stitchType;
        [ObservableProperty]
        private ObservableCollection<StitchTypes> _stitchTypeOptions = new ObservableCollection<StitchTypes>();
        [ObservableProperty]
        private YarnColors? _firstYarnColor;
        [ObservableProperty]
        private YarnColors? _secondYarnColor;
        [ObservableProperty]
        private ObservableCollection<YarnColors> _yarnColorOptions = new ObservableCollection<YarnColors>();
        [ObservableProperty]
        private Fabrics? _fabricA;
        [ObservableProperty]
        private Fabrics? _fabricB;
        [ObservableProperty]
        private Fabrics? _fabricC;
        [ObservableProperty]
        private Fabrics? _fabricD;
        [ObservableProperty]
        private ObservableCollection<Fabrics> _fabricOptions = new ObservableCollection<Fabrics>();

        //Product Options
        [ObservableProperty]
        private bool _hasDropOffApt;
        [ObservableProperty]
        private bool _hasTestTryApt;
        [ObservableProperty]
        private bool _hasPickUpApt;
        [ObservableProperty]
        private bool _hasMultipleFabrics;
        [ObservableProperty]
        private bool _hasMultipleYarnColors;
        [ObservableProperty]
        private bool _hasCustomPatternTask;
        [ObservableProperty]
        private bool _hasEmbroideryTask;
        [ObservableProperty]
        private bool _hasRipTask;
        [ObservableProperty]
        private bool _hasFoamTask;
        [ObservableProperty]
        private bool _hasGelTask;
        [ObservableProperty]
        private bool _hasCutTask;
        [ObservableProperty]
        private bool _hasSewTask;
        [ObservableProperty]
        private bool _hasCheckTask;
        [ObservableProperty]
        private bool _hasBoltTask;


        [ObservableProperty]
        private FoamType _foamType;
        public IReadOnlyList<FoamType> FoamTypeOptions { get; } = Enum.GetValues(typeof(FoamType)).Cast<FoamType>().ToList();
        [ObservableProperty]
        private RipAction _ripAction;
        public IReadOnlyList<RipAction> RipActionOptions { get; } = Enum.GetValues(typeof(RipAction)).Cast<RipAction>().ToList();


        [ObservableProperty]
        private DateTime _createdDate;
        [ObservableProperty]
        private DateTime _dropOffApt;
        [ObservableProperty]
        private DateTime _testTryApt;
        [ObservableProperty]
        private DateTime _pickUpApt;
        [ObservableProperty]
        private DateTime _expectedStartDate;
        [ObservableProperty]
        private DateTime _expectedFinishDate;

        public bool CanManufacture => !IsBusy &&
                                      Item is Products product &&
                                      !product.HasStartedManufacturing;

        public bool CanUpdateOrPause => !IsBusy &&
                                        Item is Products product &&
                                        product.HasStartedManufacturing;

        private bool AutoRefreshOptions = false;
        public ProductsViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
        }

        [RelayCommand]
        public async Task Manufacture()
        {
            // Check lock
            if (IsBusy || Item == null) return;

            IsBusy = true;
            Autosave = false;
            OnPropertyChanged(nameof(CanManufacture));
            OnPropertyChanged(nameof(CanUpdateOrPause));
            try
            {
                base.SaveEntity();

                // Force publish state and resolve identity
                if (Item.IsDraft)
                {
                    Item.IsDraft = false;
                    IsDraft = false;

                    if (Item.FromId > 0)
                    {
                        if (Item.Id > 0)
                        {
                            await restService.DeleteEntity(Item);
                        }
                        Item.Id = Item.FromId;
                    }
                }

                // 1. Flip the state in memory before saving
                ((Products)Item).HasStartedManufacturing = true;

                // Guarantee the database has the latest options
                await restService.Put<Products>((Products)Item);

                // Run the Task Generator
                bool success = await restService.ManufactureProduct(Item.Id);

                if (!success)
                {
                    ((Products)Item).HasStartedManufacturing = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during manufacture: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                Autosave = true;
                OnPropertyChanged(nameof(CanManufacture));
                OnPropertyChanged(nameof(CanUpdateOrPause));
            }
        }

        [RelayCommand]
        public async Task PauseProduction()
        {
            // 1. Safely check and cast in the guard clause
            if (IsBusy || !(Item is Products p) || !p.HasStartedManufacturing) return;

            IsBusy = true;
            Autosave = false;
            OnPropertyChanged(nameof(CanManufacture));
            OnPropertyChanged(nameof(CanUpdateOrPause));
            try
            {
                bool success = await restService.PauseProduct(Item.Id);

                if (success)
                {
                    // 2. Cast to flip the state back to false
                    ((Products)Item).HasStartedManufacturing = false;


                }
            }
            finally
            {
                IsBusy = false;
                Autosave = true;
                OnPropertyChanged(nameof(CanManufacture));
                OnPropertyChanged(nameof(CanUpdateOrPause));
            }
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
        }

        public async void LoadEntity(Products product)
        {
            if (IsBusy) { return; }
            base.LoadEntity(product);
            Autosave = false;
            if (product == null) return;

            try
            {
                // Turn on the loading spinner while we fetch the related objects
                IsBusy = true;

                // 1. Load standard text, boolean, enum, and date properties
                ImageUrl = product.ImageUrl ?? string.Empty;
                IsComplete = product.IsCompleted;
                Comments = product.Comments ?? string.Empty;

                Order = await restService.Get<Orders>(product.OrderId);
                if( Order != null)
                {
                    var result = await restService.Get<Orders, Products>(product.OrderId);
                    TotalProductsCount = result?.TotalCount ?? 0;
                }
                

                HasDropOffApt = product.HasDropOffApt;
                HasTestTryApt = product.HasTestTryApt;
                HasPickUpApt = product.HasPickUpApt;
                HasMultipleFabrics = product.HasMultipleFabrics;
                HasMultipleYarnColors = product.HasMultipleYarnColors;
                HasCustomPatternTask = product.HasCustomPatternTask;
                HasEmbroideryTask = product.HasEmbroideryTask;
                HasRipTask = product.HasRipTask;
                HasFoamTask = product.HasFoamTask;
                HasGelTask = product.HasGelTask;
                HasBoltTask = product.HasBoltTask;

                FoamType = product.FoamType;
                RipAction = product.RipAction;

                CreatedDate = product.CreatedDate;
                DropOffApt = product.DropOffApt ??DateTime.Now;
                TestTryApt = product.TestTryApt ?? DateTime.Now;
                PickUpApt = product.PickUpApt ?? DateTime.Now;
                ExpectedStartDate = product.ExpectedStartDate ?? DateTime.Now;
                ExpectedFinishDate = product.ExpectedFinishDate ?? DateTime.Now;

                AutoRefreshOptions = false;
                await RefreshCategoryOptions();
                await RefreshBrandOptions();
                await RefreshModelOptions();
                await RefreshFabricOptions();
                await RefreshStitchTypeOptions();
                await RefreshYarnColorOptions();
                AutoRefreshOptions = true;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading product relations: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                Autosave = true;
                OnPropertyChanged(nameof(CanManufacture));
                OnPropertyChanged(nameof(CanUpdateOrPause));
            }
        }

        [RelayCommand]
        public async Task CreateNewCategory()
        {
            if (Item == null) { return; }
            var category = new ProductCategories
            {
                CategoryName = string.Empty,
                IsDraft = true,
                CreatedDate = DateTime.Now,
            };
            var result = await restService.Post(category);
            if (result == null) { return; }
            category.Id = result.Id;
            if (Item is Products product)
            {
                product.CategoryId = category.Id;
            }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(result));
        }

        [RelayCommand]
        public async Task CreateNewBrand()
        {
            if (Item == null) { return; }
            var brand = new Brands
            {
                BrandName = string.Empty,
                IsDraft = true,
                CreatedDate = DateTime.Now,
            };
            var result = await restService.Post(brand);
            if (result == null) { return; }
            brand.Id = result.Id;
            if (Item is Products product)
            {
                product.BrandId = brand.Id;
            }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(result));
        }

        [RelayCommand]
        public async Task CreateNewModel()
        {
            if (Item == null) { return; }
            var model = new Models.Attributes.Models
            {
                ModelName = string.Empty,
                CategoryId = Category?.Id ?? 0,
                BrandId = Brand?.Id ?? 0,
                IsDraft = true,
                CreatedDate = DateTime.Now,
            };
            var result = await restService.Post(model);
            if (result == null) { return; }
            model.Id = result.Id;
            if (Item is Products product)
            {
                product.ModelId = model.Id;
            }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(result));
        }


        public async Task RefreshCategoryOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var categories = await restService.Get<ProductCategories>();
                    CategoryOptions.Clear();
                    if (categories == null) return;

                    // 1. Build the list completely
                    foreach (var category in categories.Items)
                    {
                        CategoryOptions.Add(category);
                    }

                    // 2. Select the item from the fully built list
                    if (product.CategoryId > 0)
                    {
                        Category = CategoryOptions.FirstOrDefault(c => c.Id == product.CategoryId);
                    }
                    else
                    {
                        Category = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing category options: {ex.Message}");
            }
        }

        public async Task RefreshBrandOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var brands = await restService.Get<Brands>();
                    BrandOptions.Clear();
                    if (brands == null) return;

                    // 1. Build the list completely
                    foreach (var brand in brands.Items)
                    {
                        BrandOptions.Add(brand);
                    }

                    // 2. Select the item from the fully built list using BrandId
                    if (product.BrandId > 0)
                    {
                        Brand = BrandOptions.FirstOrDefault(b => b.Id == product.BrandId);
                    }
                    else
                    {
                        Brand = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing brand options: {ex.Message}");
            }
        }

        public async Task RefreshModelOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var models = await restService.GetModels(new ModelsRequestParameters
                    {
                        BrandId = product.BrandId,
                        CategoryId = product.CategoryId,
                    });
                    ModelOptions.Clear();
                    if (models == null) return;

                    // 1. Build the list completely
                    foreach (var model in models.Items)
                    {
                        ModelOptions.Add(model);
                    }
                    
                    // 2. Select the item from the fully built list using ModelId
                    if (product.ModelId > 0)
                    {
                        Model = ModelOptions.FirstOrDefault(m => m.Id == product.ModelId);
                    }
                    else
                    {
                        Model = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing model options: {ex.Message}");
            }
        }

        public async Task RefreshPatternOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var patterns = await restService.Get<Models.Attributes.Models, Patterns>(product.ModelId);
                    PatternOptions.Clear();
                    if (patterns == null) return;

                    // 1. Build the list completely
                    foreach (var pattern in patterns.Items)
                    {
                        PatternOptions.Add(pattern);
                    }

                    // 2. Select the item from the fully built list using PatternId
                    if (product.PatternId > 0)
                    {
                        Pattern = PatternOptions.FirstOrDefault(p => p.Id == product.PatternId);
                    }
                    else
                    {
                        Pattern = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing pattern options: {ex.Message}");
            }
        }

        public async Task RefreshFabricOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var fabrics = await restService.Get<Fabrics>();
                    FabricOptions.Clear();
                    if (fabrics == null) return;
                    foreach (var fabric in fabrics.Items)
                    {
                        FabricOptions.Add(fabric);
                    }
                    if (product.AFabricId > 0)
                    {
                        FabricA = FabricOptions.FirstOrDefault(p => p.Id == product.AFabricId);
                    }
                    else
                    {
                        FabricA = null;
                    }
                    if (product.BFabricId > 0)
                    {
                        FabricB = FabricOptions.FirstOrDefault(p => p.Id == product.BFabricId);
                    }
                    else
                    {
                        FabricB = null;
                    }
                    if (product.CFabricId > 0)
                    {
                        FabricC = FabricOptions.FirstOrDefault(p => p.Id == product.CFabricId);
                    }
                    else
                    {
                        FabricC = null;
                    }
                    if (product.DFabricId > 0)
                    {
                        FabricD = FabricOptions.FirstOrDefault(p => p.Id == product.DFabricId);
                    }
                    else
                    {
                        FabricD = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing pattern options: {ex.Message}");
            }
        }

        public async Task RefreshStitchTypeOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var stitchTypes = await restService.Get<StitchTypes>();
                    StitchTypeOptions.Clear();
                    if (stitchTypes == null) return;
                    foreach (var stitchType in stitchTypes.Items)
                    {
                        StitchTypeOptions.Add(stitchType);
                    }
                    if (product.StitchTypeId > 0)
                    {
                        StitchType = StitchTypeOptions.FirstOrDefault(p => p.Id == product.StitchTypeId);
                    }
                    else
                    {
                        StitchType = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing pattern options: {ex.Message}");
            }
        }

        public async Task RefreshYarnColorOptions()
        {
            try
            {
                if (Item is Products product)
                {
                    var yarnColors = await restService.Get<YarnColors>();
                    YarnColorOptions.Clear();
                    if (yarnColors == null) return;
                    foreach (var yarnColor in yarnColors.Items)
                    {
                        YarnColorOptions.Add(yarnColor);
                    }
                    if (product.FirstYarnColorId > 0)
                    {
                        FirstYarnColor = YarnColorOptions.FirstOrDefault(p => p.Id == product.FirstYarnColorId);
                    }
                    else
                    {
                        FirstYarnColor = null;
                    }
                    if (product.SecondYarnColorId > 0)
                    {
                        SecondYarnColor = YarnColorOptions.FirstOrDefault(p => p.Id == product.SecondYarnColorId);
                    }
                    else
                    {
                        SecondYarnColor = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing pattern options: {ex.Message}");
            }
        }
        /*
        public override void SaveEntity()
        {
            if (Item is Products product)
            {
                Autosave = false;
                AutoRefreshOptions = false;
                base.SaveEntity();
                // 1. Map standard properties back to the model
                product.ImageUrl = ImageUrl;
                product.IsCompleted = IsComplete;
                product.Comments = Comments;

                product.HasDropOffApt = HasDropOffApt;
                product.HasTestTryApt = HasTestTryApt;
                product.HasPickUpApt = HasPickUpApt;
                product.HasMultipleFabrics = HasMultipleFabrics;
                product.HasMultipleYarnColors = HasMultipleYarnColors;
                product.HasCustomPatternTask = HasCustomPatternTask;
                product.HasEmbroideryTask = HasEmbroideryTask;
                product.HasRipTask = HasRipTask;
                product.HasFoamTask = HasFoamTask;
                product.HasGelTask = HasGelTask;
                product.HasBoltTask = HasBoltTask;

                product.FoamType = FoamType;
                product.RipAction = RipAction;

                product.CreatedDate = CreatedDate;
                product.DropOffApt = DropOffApt;
                product.TestTryApt = TestTryApt;
                product.PickUpApt = PickUpApt;
                product.ExpectedStartDate = ExpectedStartDate;
                product.ExpectedFinishDate = ExpectedFinishDate;

                // 2. Map the Selected Object IDs back to the model
                // The null-coalescing operator (??) ensures that if the user didn't pick anything, it defaults to 0
                product.OrderId = Order?.Id ?? 0;
                product.CategoryId = Category?.Id ?? 0;
                product.BrandId = Brand?.Id ?? 0;
                product.ModelId = Model?.Id ?? 0;
                product.PatternId = Pattern?.Id ?? 0;
                product.StitchTypeId = StitchType?.Id ?? 0;

                product.FirstYarnColorId = FirstYarnColor?.Id ?? 0;
                product.SecondYarnColorId = SecondYarnColor?.Id ?? 0;

                product.AFabricId = FabricA?.Id ?? 0;
                product.BFabricId = FabricB != null ? FabricB.Id : null;
                product.CFabricId = FabricC != null ? FabricC.Id : null;
                product.DFabricId = FabricD != null ? FabricD.Id : null;
                Autosave = true;
                AutoRefreshOptions = true;
            }

        }
        */

        [RelayCommand]
        public async Task OpenOrder()
        {
            if (Order == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Order));
        }

        [RelayCommand]
        public async Task OpenCategory()
        {
            if (Category == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Category));
        }

        [RelayCommand]
        public async Task OpenModel()
        {
            if (Model == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Model));
        }

        [RelayCommand]
        public async Task OpenBrand()
        {
            if (Brand == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Brand));
        }

        [RelayCommand]
        public async Task OpenPattern()
        {
            if (Pattern == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Pattern));
        }

        [RelayCommand]
        public async Task OpenStitchType()
        {
            if (StitchType == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(StitchType));
        }

        [RelayCommand]
        public async Task OpenFirstYarnColor()
        {
            if (FirstYarnColor == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(FirstYarnColor));
        }

        [RelayCommand]
        public async Task OpenSecondYarnColor()
        {
            if (SecondYarnColor == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(SecondYarnColor));
        }

        [RelayCommand]
        public async Task OpenFabricA()
        {
            if (FabricA == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(FabricA));
        }

        [RelayCommand]
        public async Task OpenFabricB()
        {
            if (FabricB == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(FabricB));
        }

        [RelayCommand]
        public async Task OpenFabricC()
        {
            if (FabricC == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(FabricC));
        }

        [RelayCommand]
        public async Task OpenFabricD()
        {
            if (FabricD == null) { return; }
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(FabricD));
        }


        // --- Strings & Booleans ---

        partial void OnImageUrlChanged(string value)
        {
            if (Item is Products product)
            {
                product.ImageUrl = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnIsCompleteChanged(bool value)
        {
            if (Item is Products product)
            {
                product.IsCompleted = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnCommentsChanged(string value)
        {
            if (Item is Products product)
            {
                product.Comments = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasDropOffAptChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasDropOffApt = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasTestTryAptChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasTestTryApt = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasPickUpAptChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasPickUpApt = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasMultipleFabricsChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasMultipleFabrics = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasMultipleYarnColorsChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasMultipleYarnColors = value;
                TriggerDebouncedSave();
            }
        }

        async partial void OnHasCustomPatternTaskChanged(bool value)
        {
            if (Item is Products product)
            {
                if (value) { product.PatternId = 0; }
                await RefreshPatternOptions();
                product.HasCustomPatternTask = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasEmbroideryTaskChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasEmbroideryTask = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasRipTaskChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasRipTask = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasFoamTaskChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasFoamTask = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasGelTaskChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasGelTask = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnHasBoltTaskChanged(bool value)
        {
            if (Item is Products product)
            {
                product.HasBoltTask = value;
                TriggerDebouncedSave();
            }
        }

        // --- Dates ---

        partial void OnCreatedDateChanged(DateTime value)
        {
            if (Item is Products product)
            {
                product.CreatedDate = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnDropOffAptChanged(DateTime value)
        {
            if (Item is Products product)
            {
                product.DropOffApt = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnTestTryAptChanged(DateTime value)
        {
            if (Item is Products product)
            {
                product.TestTryApt = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnPickUpAptChanged(DateTime value)
        {
            if (Item is Products product)
            {
                product.PickUpApt = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnExpectedStartDateChanged(DateTime value)
        {
            if (Item is Products product)
            {
                product.ExpectedStartDate = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnExpectedFinishDateChanged(DateTime value)
        {
            if (Item is Products product)
            {
                product.ExpectedFinishDate = value;
                TriggerDebouncedSave();
            }
        }

        // --- Enums ---

        partial void OnFoamTypeChanged(FoamType value)
        {
            if (Item is Products product)
            {
                product.FoamType = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnRipActionChanged(RipAction value)
        {
            if (Item is Products product)
            {
                product.RipAction = value;
                TriggerDebouncedSave();
            }
        }

        // --- Relational Objects ---

        partial void OnOrderChanged(Orders? value)
        {
            if (Item is Products product)
            {
                product.OrderId = value?.Id ?? 0;
                TriggerDebouncedSave();
            }
        }

        async partial void OnCategoryChanged(ProductCategories? value)
        {
            if (Item is Products product)
            {
                product.CategoryId = value?.Id ?? 0;
                if (AutoRefreshOptions) { await RefreshModelOptions(); }
                TriggerDebouncedSave();
            }
        }

        async partial void OnModelChanged(Models.Attributes.Models? value)
        {
            if (Item is Products product)
            {
                product.ModelId = value?.Id ?? 0;
                if (AutoRefreshOptions) { await RefreshPatternOptions(); }
                TriggerDebouncedSave();
            }
        }

        async partial void OnBrandChanged(Brands? value)
        {
            if (Item is Products product)
            {
                product.BrandId = value?.Id ?? 0;
                if (AutoRefreshOptions) { await RefreshModelOptions(); }
                TriggerDebouncedSave();
            }
        }

        partial void OnPatternChanged(Patterns? value)
        {
            if (Item is Products product)
            {
                product.PatternId = value?.Id ?? 0;
                TriggerDebouncedSave();
            }
        }

        partial void OnStitchTypeChanged(StitchTypes? value)
        {
            if (Item is Products product)
            {
                product.StitchTypeId = value?.Id ?? 0;
                TriggerDebouncedSave();
            }
        }

        partial void OnFirstYarnColorChanged(YarnColors? value)
        {
            if (Item is Products product)
            {
                product.FirstYarnColorId = value?.Id ?? 0;
                TriggerDebouncedSave();
            }
        }

        partial void OnSecondYarnColorChanged(YarnColors? value)
        {
            if (Item is Products product)
            {
                product.SecondYarnColorId = value?.Id ?? 0;
                TriggerDebouncedSave();
            }
        }

        partial void OnFabricAChanged(Fabrics? value)
        {
            if (Item is Products product)
            {
                product.AFabricId = value?.Id ?? 0;
                TriggerDebouncedSave();
            }
        }

        partial void OnFabricBChanged(Fabrics? value)
        {
            if (Item is Products product)
            {
                product.BFabricId = value != null ? value.Id : null;
                TriggerDebouncedSave();
            }
        }

        partial void OnFabricCChanged(Fabrics? value)
        {
            if (Item is Products product)
            {
                product.CFabricId = value != null ? value.Id : null;
                TriggerDebouncedSave();
            }
        }

        partial void OnFabricDChanged(Fabrics? value)
        {
            if (Item is Products product)
            {
                product.DFabricId = value != null ? value.Id : null;
                TriggerDebouncedSave();
            }
        }

    }
}

