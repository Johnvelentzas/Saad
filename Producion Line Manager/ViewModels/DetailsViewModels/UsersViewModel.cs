using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Attributes;
using Models.Management;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views.DetailsViews;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class UsersViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;


        //User Info
        [ObservableProperty]
        private string _Name = String.Empty;
        [ObservableProperty]
        private string _imageUrl = String.Empty;


        //Permissions
        [ObservableProperty]
        private bool _isAdmin = true;
        [ObservableProperty]
        private bool _sync = false;

        //Production
        [ObservableProperty]
        private bool _customersPermission = false;
        [ObservableProperty]
        private bool _ordersPermission = false;
        [ObservableProperty]
        private bool _productsPermission = false;

        //Attributes
        [ObservableProperty]
        private bool _categoriesPermission = false;
        [ObservableProperty]
        private bool _brandsPermission = false;
        [ObservableProperty]
        private bool _modelsPermission = false;
        [ObservableProperty]
        private bool _patternsPermission = false;
        [ObservableProperty]
        private bool _stitchTypesPermission = false;
        [ObservableProperty]
        private bool _yarnColorsPermission = false;
        [ObservableProperty]
        private bool _fabricsPermission = false;

        //Appointments
        [ObservableProperty]
        private bool _dropOffAptPermission = false;
        [ObservableProperty]
        private bool _testTryAptPermission = false;
        [ObservableProperty]
        private bool _pickupAptPermission = false;

        //Foam Tasks
        [ObservableProperty]
        private bool _foamFixPermission = false;
        [ObservableProperty]
        private bool _foamAdaptPermission = false;
        [ObservableProperty]
        private bool _foamGelPermission = false;
        [ObservableProperty]
        private bool _foamAnatomicalPermission = false;

        //Tasks
        [ObservableProperty]
        private bool _removeCoverPermission = false;
        [ObservableProperty]
        private bool _customPatternPermission = false;
        [ObservableProperty]
        private bool _cuttingPermission = false;
        [ObservableProperty]
        private bool _sewingPermission = false;
        [ObservableProperty]
        private bool _embroideryPermission = false;
        [ObservableProperty]
        private bool _boltingPermission = false;
        [ObservableProperty]
        private bool _inspectionPermission = false;


        //Management
        [ObservableProperty]
        private bool _usersPermission = false;


        // Production
        partial void OnCustomersPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Customers, value);
        partial void OnOrdersPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Orders, value);
        partial void OnProductsPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Products, value);

        // Management
        partial void OnUsersPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Users, value);

        // Attributes
        partial void OnCategoriesPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.ProductCategories, value);
        partial void OnBrandsPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Brands, value);
        partial void OnModelsPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Models, value);
        partial void OnPatternsPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Patterns, value);
        partial void OnStitchTypesPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.StitchTypes, value);
        partial void OnYarnColorsPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.YarnColors, value);
        partial void OnFabricsPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Fabrics, value);

        // Appointments
        partial void OnDropOffAptPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.DropOffApt, value);
        partial void OnTestTryAptPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.TestTryApt, value);
        partial void OnPickupAptPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.PickUpApt, value);

        // Foam Tasks
        partial void OnFoamFixPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.FoamFix, value);
        partial void OnFoamAdaptPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.FoamAdapt, value);
        partial void OnFoamGelPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.FoamGel, value);
        partial void OnFoamAnatomicalPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.FoamAnatomical, value);

        // Tasks
        partial void OnRemoveCoverPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.CoverRemove, value);
        partial void OnCustomPatternPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.CustomPattern, value);
        partial void OnCuttingPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Cut, value);
        partial void OnSewingPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Sew, value);
        partial void OnEmbroideryPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Embroider, value);
        partial void OnBoltingPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Bolt, value);
        partial void OnInspectionPermissionChanged(bool value) => HandlePermissionToggle(ProcessesType.Inspect, value);

        private async void HandlePermissionToggle(ProcessesType process, bool isEnabled)
        {
            if (!Sync || IsAdmin) return;
            int processId = (int)process + 1;
            try
            {
                await restService.ToggleUserProcess(Id, processId, isEnabled);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to sync {process} permission: {ex.Message}");
            }
        }
        public UsersViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
        }

        [RelayCommand]
        public void RemoveImage()
        {
            ImageUrl = String.Empty;
        }

        [RelayCommand]
        public async Task ChooseImage()
        {
            try
            {
                var results = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
                {
                    SelectionLimit = 1,
                    MaximumWidth = 1024,
                    MaximumHeight = 768,
                    CompressionQuality = 85,
                    RotateImage = true,
                    PreserveMetaData = true,
                });

                var selectedPhoto = results?.FirstOrDefault();
                if (selectedPhoto == null) { return; }
                
                using var stream = await selectedPhoto.OpenReadAsync();
                var uploadedUrl = await restService.UploadImage(stream, $"User{Id}_{Name}.jpg");

                if (!string.IsNullOrEmpty(uploadedUrl))
                {
                    ImageUrl = uploadedUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Immediate upload sequence failed: {ex.Message}");
            }
        }

        [RelayCommand]
        public void TakeImage()
        {

        }

        partial void OnNameChanged(String value)
        {
            if (Item is Users user)
            {
                if (value.ToLower() == "admin")
                {
                    value = "Can't be called admin";
                }

                user.Name = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnImageUrlChanged(String value)
        {
            if (Item is Users user)
            {
                user.ImageUrl = value;
                TriggerImmediateSave();
            }
        }

        public async void LoadEntity(Users user)
        {
            Name = user.Name;
            ImageUrl = user.ImageUrl ?? String.Empty;
            IsAdmin = user.Name.ToLower() == "admin";
            base.LoadEntity(user);
            await LoadProcesses();
            Sync = true;
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Users user)
            {
                user.Name = Name;
                user.ImageUrl = ImageUrl;
            }
        }

        private async Task LoadProcesses()
        {
            var processes = await restService.Get<Users, UserProcesses>(Id);
            if (processes == null) { return; }
            foreach (var userProcess in processes.Items)
            {
                var processType = (ProcessesType)(userProcess.ProcessId - 1);
                switch (processType)
                {
                    // Production
                    case ProcessesType.Customers: CustomersPermission = true; break;
                    case ProcessesType.Orders: OrdersPermission = true; break;
                    case ProcessesType.Products: ProductsPermission = true; break;

                    // Management
                    case ProcessesType.Users: UsersPermission = true; break;

                    // Attributes
                    case ProcessesType.ProductCategories: CategoriesPermission = true; break;
                    case ProcessesType.Brands: BrandsPermission = true; break;
                    case ProcessesType.Models: ModelsPermission = true; break;
                    case ProcessesType.Patterns: PatternsPermission = true; break;
                    case ProcessesType.StitchTypes: StitchTypesPermission = true; break;
                    case ProcessesType.YarnColors: YarnColorsPermission = true; break;
                    case ProcessesType.Fabrics: FabricsPermission = true; break;

                    // Appointments
                    case ProcessesType.DropOffApt: DropOffAptPermission = true; break;
                    case ProcessesType.TestTryApt: TestTryAptPermission = true; break;
                    case ProcessesType.PickUpApt: PickupAptPermission = true; break;

                    // Foam Tasks
                    case ProcessesType.FoamFix: FoamFixPermission = true; break;
                    case ProcessesType.FoamAdapt: FoamAdaptPermission = true; break;
                    case ProcessesType.FoamGel: FoamGelPermission = true; break;
                    case ProcessesType.FoamAnatomical: FoamAnatomicalPermission = true; break;

                    // Tasks
                    case ProcessesType.CoverRemove: RemoveCoverPermission = true; break;
                    case ProcessesType.CustomPattern: CustomPatternPermission = true; break;
                    case ProcessesType.Cut: CuttingPermission = true; break;
                    case ProcessesType.Sew: SewingPermission = true; break;
                    case ProcessesType.Embroider: EmbroideryPermission = true; break;
                    case ProcessesType.Bolt: BoltingPermission = true; break;
                    case ProcessesType.Inspect: InspectionPermission = true; break;

                    // Tabs
                    case ProcessesType.Tasks:
                    case ProcessesType.Foam:
                    case ProcessesType.Calendar:
                        break;
                }
            }
        }

    }
}

