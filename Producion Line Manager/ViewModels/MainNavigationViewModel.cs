

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
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var savedUserId = Preferences.Default.Get("UserId", "NoUser");
                if (savedUserId == "NoUser")
                {
                    await navigationService.NavigateTo<UserSelectionPage>();
                    IsBusy = false;
                    return;
                }
                var foundUser = await restService.Get<Users>(int.Parse(savedUserId));
                if (foundUser != null)
                {
                    User = foundUser;
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




            try
            {
                IsBusy = true;
                if (User == null)
                {
                    return;
                }

                var results = await restService.Get<Users, Processes>(User.Id);
                if (results == null || results.Items.Count() < 1)
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
                TabItem? Tasks = null;
                TabItem? Calendar = null;
                TabItem? Foam = null;

                var hasBasic = false;
                var hasProduction = false;
                var hasCalendar = false;
                var hasTasks = false;

                foreach (var type in BasicTabs)
                {
                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasBasic)
                        {
                            hasBasic = true;
                            DisplayTabs.Add(new TabItem());
                        }
                        DisplayTabs.Add(TabItem.FromProcess(process));
                    }
                }
                foreach (var type in ProductionTabs)
                {
                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasProduction)
                        {
                            hasProduction = true;
                            DisplayTabs.Add(new TabItem());
                        }
                        DisplayTabs.Add(TabItem.FromProcess(process));
                    }
                }
                foreach (var type in CalendarTabs)
                {

                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasCalendar)
                        {
                            hasCalendar = true;
                            DisplayTabs.Add(new TabItem());
                        }
                        if (Calendar == null)
                        {
                            Calendar = new TabItem(101, "Calendar", null, ProcessesType.Calendar);
                            DisplayTabs.Add(Calendar);
                        }
                        var tab = TabItem.FromProcess(process);
                        tab.IsSubTab = true;
                        Calendar.AddChild(tab);

                    }
                }

                foreach (var type in TaskTabs)
                {

                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasTasks)
                        {
                            hasTasks = true;
                            DisplayTabs.Add(new TabItem());
                        }
                        if (Tasks == null)
                        {
                            Tasks = new TabItem(100, "Tasks", null, ProcessesType.Tasks);
                            DisplayTabs.Add(Tasks);
                        }
                        var tab = TabItem.FromProcess(process);
                        tab.IsSubTab = true;
                        Tasks.AddChild(tab);

                    }
                }

                foreach (var type in FoamTabs)
                {

                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (Tasks == null)
                        {
                            Tasks = new TabItem(100, "Tasks", null, ProcessesType.Tasks);
                            DisplayTabs.Add(Tasks);
                        }
                        if (Foam == null)
                        {
                            Foam = new TabItem(102, "Foam", null, ProcessesType.Foam);
                            Foam.IsSubTab = true;
                            Tasks.AddChild(Foam);
                        }
                        var tab = TabItem.FromProcess(process);
                        tab.IsSubTab = true;
                        Foam.AddChild(tab);

                    }
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
