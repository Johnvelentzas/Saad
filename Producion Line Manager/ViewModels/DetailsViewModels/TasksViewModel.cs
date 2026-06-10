using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models.Attributes;
using Models.Management;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Messages;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views;
using Producion_Line_Manager.Views.DetailsViews;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class TasksViewModel : BaseEntityViewModel
    {
        private readonly RestService restService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasComments))]
        private Products? _product;

        [ObservableProperty]
        private Tasks? _currentTask;

        [ObservableProperty] private string _categoryName = string.Empty;
        [ObservableProperty] private string _modelName = string.Empty;
        [ObservableProperty] private string _brandName = string.Empty;
        [ObservableProperty] private string _patternName = string.Empty;
        [ObservableProperty] private string _stitchTypeName = string.Empty;
        [ObservableProperty] private string _firstYarnColorName = string.Empty;
        [ObservableProperty] private string _secondYarnColorName = string.Empty;
        [ObservableProperty] private string _aFabricName = string.Empty;
        [ObservableProperty] private string _bFabricName = string.Empty;
        [ObservableProperty] private string _cFabricName = string.Empty;
        [ObservableProperty] private string _dFabricName = string.Empty;

        [ObservableProperty]
        private string _taskName = String.Empty;
        [ObservableProperty]
        private ProcessesType? _currentProcessesType;

        // True only if the task exists and is NOT marked completed yet
        public bool IsTaskIncomplete => CurrentTask != null && !CurrentTask.IsCompleted;

        // True only if the task has already been completed
        public bool IsTaskComplete => CurrentTask != null && CurrentTask.IsCompleted;

        private int UserId;

        // Comment visibility
        public bool HasComments => !string.IsNullOrWhiteSpace(Product?.Comments);

        // --- DYNAMIC UI TRIGGERS (Fixed with DB +1 Offset) ---

        // 1. Appointment (Drop Off, Test Try, Pick Up)
        public bool IsDropOffTask => CurrentTask?.ProcessId == (int)ProcessesType.DropOffApt + 1;
        public bool IsTestTryTask => CurrentTask?.ProcessId == (int)ProcessesType.TestTryApt + 1;
        public bool IsPickUpTask => CurrentTask?.ProcessId == (int)ProcessesType.PickUpApt + 1;

        // 2. Foam Type (Fix, Adapt, Anatomical)
        public bool IsFoamTypeTask => CurrentTask?.ProcessId == (int)ProcessesType.FoamFix + 1 ||
                                      CurrentTask?.ProcessId == (int)ProcessesType.FoamAdapt + 1 ||
                                      CurrentTask?.ProcessId == (int)ProcessesType.FoamAnatomical + 1;

        // 3. Cover Remove
        public bool IsCoverRemoveTask => CurrentTask?.ProcessId == (int)ProcessesType.CoverRemove + 1;

        // 4. Cut
        public bool IsCutTask => CurrentTask?.ProcessId == (int)ProcessesType.Cut + 1;

        // 5. Sew
        public bool IsSewTask => CurrentTask?.ProcessId == (int)ProcessesType.Sew + 1;

        public TasksViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
            var savedUserId = Preferences.Default.Get("UserId", "NoUser");
            if (savedUserId != "NoUser")
            {
                UserId = Int32.Parse(savedUserId);
            }
        }

        public async void LoadEntity(Tasks task)
        {
            CurrentTask = task;

            // Re-evaluate your new granular dynamic properties
            OnPropertyChanged(nameof(IsDropOffTask));
            OnPropertyChanged(nameof(IsTestTryTask));
            OnPropertyChanged(nameof(IsPickUpTask));
            OnPropertyChanged(nameof(IsFoamTypeTask));
            OnPropertyChanged(nameof(IsCoverRemoveTask));
            OnPropertyChanged(nameof(IsCutTask));
            OnPropertyChanged(nameof(IsSewTask));
            OnPropertyChanged(nameof(IsTaskComplete));
            OnPropertyChanged(nameof(IsTaskIncomplete));

            var result = await restService.Get<Products>(CurrentTask.ProductId);
            if (result != null)
            {
                Product = result;

                // Fills all your string strings concurrently right when the page initializes
                await LoadProductDetailsAsync(Product);
            }
            CurrentProcessesType = (ProcessesType)(CurrentTask.ProcessId - 1);
            TaskName = CurrentProcessesType switch
            {
                ProcessesType.DropOffApt => "Drop Off Appointment",
                ProcessesType.CoverRemove => "Cover Removal / Ripping",
                ProcessesType.FoamFix => "Foam Fix",
                ProcessesType.FoamAdapt => "Foam Adaptation",
                ProcessesType.FoamAnatomical => "Anatomical Foam Shaping",
                ProcessesType.FoamGel => "Foam Gel Insertion",
                ProcessesType.TestTryApt => "Test Try Appointment",
                ProcessesType.CustomPattern => "Custom Pattern Making",
                ProcessesType.Cut => "Fabric Cutting",
                ProcessesType.Embroider => "Embroidery",
                ProcessesType.Sew => "Sewing",
                ProcessesType.Bolt => "Bolting",
                ProcessesType.Inspect => "Final Inspection",
                ProcessesType.PickUpApt => "Pick Up Appointment",
                _ => $"Task #{CurrentTask.ProcessId}" // Fallback safety catch
            };
        }

        public override void SaveEntity()
        {
            // Left intentionally blank as tasks are updated explicitly via the execution buttons
        }

        [RelayCommand]
        public async Task GoToProduct()
        {
            if (Product == null) return;
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Product));
        }

        // --- NEW: TASK COMPLETION ACTION ---
        [RelayCommand]
        public async Task CompleteTask()
        {
            if (IsBusy || CurrentTask == null) return;

            IsBusy = true;
            
            try
            {
                // 1. Flip the status flags locally
                CurrentTask.IsCompleted = true;
                OnPropertyChanged(nameof(IsTaskComplete));
                OnPropertyChanged(nameof(IsTaskIncomplete));

                // 2. Push update payload to your Azure Backend
                bool success = await restService.CompleteTask(CurrentTask.Id, UserId);

                if (success)
                {
                    // Optional: You can send an update message out if your TabList needs to instantly refresh
                    // WeakReferenceMessenger.Default.Send(new RefreshListMessage());

                    // 3. Kick the user back to their main task terminal list

                }
                else
                {
                    // Revert state if backend database rejected it
                    CurrentTask.IsCompleted = false;
                    OnPropertyChanged(nameof(IsTaskComplete));
                    OnPropertyChanged(nameof(IsTaskIncomplete));
                }
            }
            catch (Exception ex)
            {
                if (CurrentTask != null) CurrentTask.IsCompleted = false;
                System.Diagnostics.Debug.WriteLine($"Error completing task: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadProductDetailsAsync(Products product)
        {
            var fetchTasks = new List<Task>();

            // 1. Category Lookup
            if (product.CategoryId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<ProductCategories>(product.CategoryId);
                    CategoryName = res?.CategoryName ?? "Unknown";
                }));

            // 2. Model Lookup
            if (product.ModelId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Models.Attributes.Models>(product.ModelId);
                    ModelName = res?.ModelName ?? "Unknown";
                }));

            // 3. Brand Lookup
            if (product.BrandId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Brands>(product.BrandId);
                    BrandName = res?.BrandName ?? "Unknown";
                }));

            // 4. Pattern Lookup
            if (product.PatternId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Patterns>(product.PatternId);
                    PatternName = res?.Name ?? "Unknown";
                }));

            // 5. Stitch Type Lookup
            if (product.StitchTypeId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<StitchTypes>(product.StitchTypeId);
                    StitchTypeName = res?.StitchTypeName ?? "Unknown";
                }));

            // 6. Yarn Colors (First and Second)
            if (product.FirstYarnColorId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<YarnColors>(product.FirstYarnColorId);
                    FirstYarnColorName = res?.YarnColorName ?? "None";
                }));

            if (product.SecondYarnColorId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<YarnColors>(product.SecondYarnColorId);
                    SecondYarnColorName = res?.YarnColorName ?? "None";
                }));

            // 7. Fabrics (A, B, C, D)
            if (product.AFabricId > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Fabrics>(product.AFabricId);
                    AFabricName = res?.FabricName ?? "Unknown";
                }));

            if (product.BFabricId is > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Fabrics>(product.BFabricId.Value);
                    BFabricName = res?.FabricName ?? "None";
                }));

            if (product.CFabricId is > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Fabrics>(product.CFabricId.Value);
                    CFabricName = res?.FabricName ?? "None";
                }));

            if (product.DFabricId is > 0)
                fetchTasks.Add(Task.Run(async () => {
                    var res = await restService.Get<Fabrics>(product.DFabricId.Value);
                    DFabricName = res?.FabricName ?? "None";
                }));

            // Execute all active HTTP requests simultaneously
            await Task.WhenAll(fetchTasks);
        }
    }
}