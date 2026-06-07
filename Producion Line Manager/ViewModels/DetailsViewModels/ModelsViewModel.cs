using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using Models.Attributes;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Messages;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views.DetailsViews;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class ModelsViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        private int Page = 1;
        private int TotalPages = 1;
        private int PageSize = 20;

        private int DraftPage = 1;
        private int DraftTotalPages = 1;
        private int DraftPageSize = 20;

        //Model
        [ObservableProperty]
        private string _modelName = String.Empty;
        [ObservableProperty]
        private string _imageUrl = String.Empty;

        //Category
        [ObservableProperty]
        private int _categoryId = 0;
        [ObservableProperty]
        private ProductCategories? _category;
        [ObservableProperty]
        private ObservableCollection<ProductCategories> _categoryOptions = new();

        //Brand
        [ObservableProperty]
        private int _brandId = 0;
        [ObservableProperty]
        private Brands? _brand;
        [ObservableProperty]
        private ObservableCollection<Brands> _brandOptions = new();

        //Patterns
        [ObservableProperty]
        private int _numberOfDraftPatterns = 0;
        [ObservableProperty]
        private int _numberOfPatterns = 0;
        [ObservableProperty]
        private ObservableCollection<Patterns> _patterns = new ObservableCollection<Patterns>();
        [ObservableProperty]
        private ObservableCollection<Patterns> _draftPatterns = new ObservableCollection<Patterns>();


        public ModelsViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
        }

        [RelayCommand]
        public async Task CreateNewPattern()
        {
            if (Item == null) { return; }
            var newPattern = new Patterns
            {
                ModelId = Item.Id,
                IsDraft = true,
                Name = String.Empty,
                CreatedDate = DateTime.Now,
            };
            var result = await restService.Post(newPattern);
            newPattern.Id = result?.Id ?? 0;
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(newPattern));
        }

        [RelayCommand]
        public async Task OpenPattern(Patterns pattern)
        {
            if (pattern == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(pattern));
        }

        [RelayCommand]
        public async Task RefreshPatterns()
        {
            Page = 1;
            PageSize = 20;
            TotalPages = 1;

            DraftPage = 1;
            DraftPageSize = 20;
            DraftTotalPages = 1;

            await LoadMoreDrafts();
            await LoadMoreItems();
        }

        [RelayCommand]
        public async Task LoadMoreItems()
        {
            if (IsBusy) { return; }
            if (Page > TotalPages) { return; }
            IsBusy = true;
            try
            {
                var parameters = new RequestParameters(
                    new List<FilterType>(),
                    null,
                    null,
                    Page,
                    PageSize,
                    SortType.IdDecending);
                IRequestResult? result = await restService.Get<Models.Attributes.Models, Patterns>(Id, parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                TotalPages = result.TotalPages;
                NumberOfPatterns = result.TotalCount;
                foreach (Patterns pattern in result.Items)
                {
                    Patterns.Add(pattern);
                }
                Page++;
            }
            finally
            {
                IsBusy = false;
            }

        }

        [RelayCommand]
        public async Task LoadMoreDrafts()
        {
            if (IsBusy) { return; }
            if (DraftPage > DraftTotalPages) { return; }
            IsBusy = true;
            try
            {
                var parameters = new RequestParameters(
                    new List<FilterType>(),
                    null,
                    null,
                    DraftPage,
                    DraftPageSize,
                    SortType.IdDecending);
                parameters.Filters.Add(FilterType.Draft);
                IRequestResult? result = await restService.Get<Models.Attributes.Models, Patterns>(Id, parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                DraftTotalPages = result.TotalPages;
                NumberOfDraftPatterns = result.TotalCount;
                foreach (Patterns pattern in result.Items)
                {
                    DraftPatterns.Add(pattern);
                }
                DraftPage++;
            }
            finally
            {
                IsBusy = false;
            }
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
                var uploadedUrl = await restService.UploadImage(stream, $"Model_{Id}_{ModelName}.jpg");

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

        partial void OnModelNameChanged(String value)
        {
            if (Item is Models.Attributes.Models model)
            {
                model.ModelName = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnImageUrlChanged(String value)
        {
            if (Item is Models.Attributes.Models model)
            {
                model.ImageUrl = value;
                TriggerImmediateSave();
            }
        }

        partial void OnCategoryIdChanged(int value)
        {
            if (Item is Models.Attributes.Models model)
            {
                model.CategoryId = value;
                TriggerImmediateSave();
            }
        }

        partial void OnBrandIdChanged(int value)
        {
            if (Item is Models.Attributes.Models model)
            {
                model.BrandId = value;
                TriggerImmediateSave();
            }
        }

        partial void OnCategoryChanged(ProductCategories? value)
        {
            if (value == null) { return; }
            CategoryId = value.Id;
        }

        partial void OnBrandChanged(Brands? value)
        {
            if (value == null) { return; }
            BrandId = value.Id;
        }

        [RelayCommand]
        public async Task OpenCategory()
        {
            if (Category == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Category));
        }

        [RelayCommand]
        public async Task OpenBrand()
        {
            if (Brand == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Brand));
        }

        public async void LoadEntity(Models.Attributes.Models model)
        {
            ModelName = model.ModelName;
            ImageUrl = model.ImageUrl ?? String.Empty;
            CategoryId = model.CategoryId;
            BrandId = model.BrandId;
            base.LoadEntity(model);
            Category = await restService.Get<ProductCategories>(CategoryId);
            var response = await restService.Get<ProductCategories>();
            if (response != null)
            {
                foreach(ProductCategories item in response.Items)
                {
                    CategoryOptions.Add(item);
                }
            }
            Brand = await restService.Get<Brands>(BrandId);
            var responseB = await restService.Get<Brands>();
            if (responseB != null)
            {
                foreach (Brands item in responseB.Items)
                {
                    BrandOptions.Add(item);
                }
            }
            
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Models.Attributes.Models model)
            {
                model.ModelName = ModelName;
                model.ImageUrl = ImageUrl;
                model.CategoryId = CategoryId;
                model.BrandId = BrandId;
            }
        }

    }
}

