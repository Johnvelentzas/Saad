using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Attributes;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views.DetailsViews;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class BrandsViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private string _brandName = String.Empty;
        [ObservableProperty]
        private string _imageUrl = String.Empty;

        public BrandsViewModel()
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
                var uploadedUrl = await restService.UploadImage(stream, $"Brand_{Id}_{BrandName}.jpg");

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

        partial void OnBrandNameChanged(String value)
        {
            if (Item is Brands brand)
            {
                brand.BrandName = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnImageUrlChanged(String value)
        {
            if (Item is Brands brand)
            {
                brand.ImageUrl = value;
                TriggerImmediateSave();
            }
        }

        public async void LoadEntity(Brands brand)
        {
            BrandName = brand.BrandName;
            ImageUrl = brand.ImageUrl ?? String.Empty;
            base.LoadEntity(brand);
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Brands brand)
            {
                brand.BrandName = BrandName;
                brand.ImageUrl = ImageUrl;
            }
        }

    }
}

