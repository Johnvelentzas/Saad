

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Management;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Producion_Line_Manager.ViewModels
{
    public partial class MainNavigationViewModel : BaseViewModel
    {

        private readonly RestService restService;

        private readonly NavigationService navigationService;

        [ObservableProperty]
        private ContentView _activeView;

        [ObservableProperty]
        private Users? _user;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayTabs))]
        private ObservableCollection<Processes> _processes;

        [ObservableProperty]
        private ObservableCollection<TabItem> _displayTabs;

        [ObservableProperty]
        private TabItem _activeTab = new TabItem();

        [ObservableProperty]
        private bool _isOverviewActive = false;

        public MainNavigationViewModel()
        {
            Title = "Main Navigation";
            Processes = new ObservableCollection<Processes>();
            DisplayTabs = new ObservableCollection<TabItem>();
            restService = ServiceHelper.GetService<RestService>();
            navigationService = ServiceHelper.GetService<NavigationService>();
        }

        [RelayCommand]
        public async Task FetchData()
        {
            // --- 1. FETCH USER ---
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var savedUserId = Preferences.Default.Get("UserId", "NoUser");
                if (savedUserId == "NoUser")
                {
                    await navigationService.NavigateTo<UserSelectionPage>();
                    return;
                }
                var foundUser = await restService.Get<Users>(int.Parse(savedUserId));
                if (foundUser != null) User = foundUser;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching user: {ex.Message}");
            }
            finally { IsBusy = false; }

            // --- 2. FETCH PROCESSES & BUILD UI ---
            try
            {
                IsBusy = true;
                if (User == null) return;

                var results = await restService.Get<Users, Processes>(User.Id);
                if (results == null || !results.Items.Any())
                {
                    Debug.WriteLine("No processes found for the user.");
                    return;
                }

                Processes.Clear();
                DisplayTabs.Clear();
                foreach (var process in results.Items)
                {
                    Processes.Add(process);
                }

                // 1. Build Flat Groups
                BuildFlatTabGroup(BasicTabs);
                BuildFlatTabGroup(ProductionTabs);

                // 2. Build Calendar Group
                var calendarProcesses = Processes.Where(p => CalendarTabs.Contains(p.Type)).ToList();
                if (calendarProcesses.Any())
                {
                    DisplayTabs.Add(new TabItem()); // Visual Separator

                    // Try to find the actual DB process for the icon/color, otherwise use a seamless fallback
                    var calProcess = Processes.FirstOrDefault(p => p.Type == ProcessesType.Calendar)
                                     ?? new Processes { Id = 101, Type = ProcessesType.Calendar, IconText = "\uebcc", Color = "#455A64" };

                    var calendarParent = TabItem.FromProcess(calProcess);

                    foreach (var p in calendarProcesses.OrderBy(p => CalendarTabs.IndexOf(p.Type)))
                    {
                        var child = TabItem.FromProcess(p);
                        child.IsSubTab = true;
                        calendarParent.AddChild(child);
                    }
                    DisplayTabs.Add(calendarParent);
                }

                // 3. Build Tasks & Foam Group (Nested Hierarchy)
                var taskProcesses = Processes.Where(p => TaskTabs.Contains(p.Type)).ToList();
                var foamProcesses = Processes.Where(p => FoamTabs.Contains(p.Type)).ToList();

                if (taskProcesses.Any() || foamProcesses.Any())
                {
                    DisplayTabs.Add(new TabItem()); // Visual Separator

                    var tasksProcess = Processes.FirstOrDefault(p => p.Type == ProcessesType.Tasks)
                                       ?? new Processes { Id = 100, Type = ProcessesType.Tasks, IconText = "\uf045", Color = "#455A64" };

                    var tasksParent = TabItem.FromProcess(tasksProcess);

                    // Add standard tasks
                    foreach (var p in taskProcesses.OrderBy(p => TaskTabs.IndexOf(p.Type)))
                    {
                        var child = TabItem.FromProcess(p);
                        child.IsSubTab = true;
                        tasksParent.AddChild(child);
                    }

                    // Add nested Foam tasks
                    if (foamProcesses.Any())
                    {
                        var foamProcess = Processes.FirstOrDefault(p => p.Type == ProcessesType.Foam)
                                          ?? new Processes { Id = 102, Type = ProcessesType.Foam, IconText = "\ue2bd", Color = "#455A64" };

                        var foamParent = TabItem.FromProcess(foamProcess);
                        foamParent.IsSubTab = true; // Foam is a sub-folder of Tasks

                        foreach (var p in foamProcesses.OrderBy(p => FoamTabs.IndexOf(p.Type)))
                        {
                            var foamChild = TabItem.FromProcess(p);
                            foamChild.IsSubTab = true;
                            foamParent.AddChild(foamChild);
                        }

                        tasksParent.AddChild(foamParent);
                    }

                    DisplayTabs.Add(tasksParent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching processes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Add this helper method inside your ViewModel to keep things clean
        private void BuildFlatTabGroup(List<ProcessesType> allowedTypes)
        {
            var matchingProcesses = Processes.Where(p => allowedTypes.Contains(p.Type)).ToList();
            if (!matchingProcesses.Any()) return;

            DisplayTabs.Add(new TabItem()); // Visual Separator

            // Order them exactly how you defined them in your hardcoded lists
            foreach (var process in matchingProcesses.OrderBy(p => allowedTypes.IndexOf(p.Type)))
            {
                DisplayTabs.Add(TabItem.FromProcess(process));
            }
        }

        private readonly List<ProcessesType> BasicTabs = new List<ProcessesType>
        {
            ProcessesType.Customers,
            ProcessesType.Orders,
            ProcessesType.Products,
            ProcessesType.Users
        };

        private readonly List<ProcessesType> ProductionTabs = new List<ProcessesType>
        {
            ProcessesType.ProductCategories,
            ProcessesType.Brands,
            ProcessesType.Models,
            ProcessesType.Patterns,
            ProcessesType.YarnColors,
            ProcessesType.Fabrics,
            ProcessesType.StitchTypes,
        };

        private readonly List<ProcessesType> CalendarTabs = new List<ProcessesType>
        {
            ProcessesType.PickUpApt,
            ProcessesType.TestTryApt,
            ProcessesType.DropOffApt
        };

        private readonly List<ProcessesType> FoamTabs = new List<ProcessesType>
        {
            ProcessesType.FoamFix,
            ProcessesType.FoamAdapt,
            ProcessesType.FoamGel,
            ProcessesType.FoamAnatomical
        };

        private readonly List<ProcessesType> TaskTabs = new List<ProcessesType>
        {
            ProcessesType.CoverRemove,
            ProcessesType.CustomPattern,
            ProcessesType.Cut,
            ProcessesType.Sew,
            ProcessesType.Embroider,
            ProcessesType.Bolt,
            ProcessesType.Inspect
        };

        [RelayCommand]
        public async Task ToggleTab(TabItem tab)
        {
            if (tab == null || !tab.HasChildren) return;

            // 1. Flip the boolean state
            tab.IsExpanded = !tab.IsExpanded;

            // 2. Logic to Add or Remove items from your DisplayTabs list

            if (tab.IsExpanded)
            {
                await ExpandTab(tab);
            }
            else
            {
                await CollapseTab(tab);
            }

        }

        private async Task ExpandTab(TabItem tab)
        {
            if (tab.HasChildren)
            {
                var index = DisplayTabs.IndexOf(tab);
                foreach (var child in tab.Children)
                {
                    if (!DisplayTabs.Contains(child))
                    {
                        index++;
                        DisplayTabs.Insert(index, child);
                    }
                }
            }
        }

        private async Task CollapseTab(TabItem tab)
        {
            if (tab.HasChildren)
            {
                foreach (var child in tab.Children)
                {
                    if (child.HasChildren)
                    {
                        await CollapseTab(child);
                    }
                    DisplayTabs.Remove(child);
                }
            }
        }

        [RelayCommand]
        public async Task NavigateToUserSelection()
        {
            await navigationService.NavigateTo<UserSelectionPage>();
        }

        [RelayCommand]
        public async Task NavigateToSettings()
        {
            await navigationService.NavigateTo<SettingsPage>();
        }

        [RelayCommand]
        public async Task AttachOverviewView()
        {
            ActiveTab.IsActive = false;
            IsOverviewActive = true;
            var overviewView = ServiceHelper.GetService<OverviewView>();
            ActiveView = overviewView;
        }

        [RelayCommand]
        public async Task AttachListView(TabItem tab)
        {
            ActiveTab.IsActive = false;
            ActiveTab = tab;
            ActiveTab.IsActive = true;
            var listView = ServiceHelper.GetService<TabListView>();
            await listView.OpenTab(ActiveTab);
            ActiveView = listView;
        }
    }
}
