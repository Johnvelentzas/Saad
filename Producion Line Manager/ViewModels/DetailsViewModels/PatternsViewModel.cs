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
    public partial class PatternsViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        //Pattern
        [ObservableProperty]
        private string _PatternName = String.Empty;
        [ObservableProperty]
        private string _imageUrl = String.Empty;

        //Category
        [ObservableProperty]
        private ProductCategories? _category;
        [ObservableProperty]
        private ObservableCollection<ProductCategories> _categoryOptions = new();

        //Brand
        [ObservableProperty]
        private Brands? _brand;
        [ObservableProperty]
        private ObservableCollection<Brands> _brandOptions = new();


        //Model
        [ObservableProperty]
        private int _modelId = 0;
        [ObservableProperty]
        private Models.Attributes.Models? _model;
        [ObservableProperty]
        private ObservableCollection<Models.Attributes.Models> _modelOptions = new();
        [ObservableProperty]
        private string _searchQuerry = string.Empty;
        [ObservableProperty]
        private ObservableCollection<Models.Attributes.Models> _suggestedModels = new();
        [ObservableProperty]
        private bool _canChangeModel = false;
        public bool CannotChangeModel => !CanChangeModel;
        private CancellationTokenSource? _searchCancellationTokenSource;
        private int ResultsSize = 5;

        public PatternsViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
        }

        async partial void OnSearchQuerryChanged(string value)
        {
            if (value.Length < 4 && value.Length > 0) { return; }
            _searchCancellationTokenSource?.Cancel();
            _searchCancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, _searchCancellationTokenSource.Token);
                await RefreshItems();
            }
            catch (TaskCanceledException)
            {

            }
        }

        [RelayCommand]
        public async Task ChangeModel()
        {
            CanChangeModel = true;
            OnPropertyChanged(nameof(CannotChangeModel));
        }

        [RelayCommand]
        public async Task SelectModel(Models.Attributes.Models model)
        {
            Model = model;
            await LoadModel(model);
        }

        [RelayCommand]
        public async Task RefreshItems()
        {
            if (IsBusy) { return; }
            IsBusy = true;
            try
            {
                SuggestedModels.Clear();
                var parameters = new ModelsRequestParameters(
                    null,
                    SearchType.General,
                    SearchQuerry,
                    1,
                    ResultsSize,
                    SortType.IdDecending,
                    Brand?.Id,
                    Category?.Id);
                RequestResult<Models.Attributes.Models>? result = await restService.GetModels(parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                foreach (var item in result.Items)
                {
                    SuggestedModels.Add(item);

                }
            }
            finally
            {
                IsBusy = false;
            }

        }

        partial void OnPatternNameChanged(String value)
        {
            if (Item is Patterns pattern)
            {
                pattern.Name = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnImageUrlChanged(String value)
        {
            if (Item is Patterns pattern)
            {
                pattern.ImageUrl = value;
                TriggerImmediateSave();
            }
        }

        partial void OnModelIdChanged(int value)
        {
            if (Item is Patterns pattern)
            {
                pattern.ModelId = value;
                TriggerImmediateSave();
            }
        }

        partial void OnModelChanged(Models.Attributes.Models? value)
        {
            RefreshItems();
        }

        partial void OnBrandChanged(Brands? value)
        {
            RefreshItems();
        }

        [RelayCommand]
        public async Task OpenModel()
        {
            if (Model == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Model));
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
                var uploadedUrl = await restService.UploadImage(stream, $"Pattern_{Id}_{PatternName}.jpg");

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

        public async void LoadEntity(Patterns pattern)
        {
            PatternName = pattern.Name;
            ImageUrl = pattern.ImageUrl ?? String.Empty;
            ModelId = pattern.ModelId;
            base.LoadEntity(pattern);
            Model = await restService.Get<Models.Attributes.Models>(ModelId);
            var response = await restService.Get<ProductCategories>();
            if (response != null)
            {
                foreach (ProductCategories item in response.Items)
                {
                    CategoryOptions.Add(item);
                }
            }
            var responseB = await restService.Get<Brands>();
            if (responseB != null)
            {
                foreach (Brands item in responseB.Items)
                {
                    BrandOptions.Add(item);
                }
            }
            await RefreshItems();

        }

        private async Task LoadModel(Models.Attributes.Models? model)
        {
            if (model != null)
            {
                CanChangeModel = false;
                OnPropertyChanged(nameof(CannotChangeModel));
                ModelId = model.Id;
                Model = await restService.Get<Models.Attributes.Models>(ModelId);
            }
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Patterns pattern)
            {
                pattern.Name = PatternName;
                pattern.ImageUrl = ImageUrl;
                pattern.ModelId = ModelId;
            }
        }

    }
}

