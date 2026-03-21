

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Producion_Line_Manager.ViewModels
{
    public partial class MainNavigationViewModel : BaseViewModel
    {

        private readonly RestService restService;

        [ObservableProperty]
        private Users _user;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayTabs))]
        private ObservableCollection<Processes> _processes;

        [ObservableProperty]
        private ObservableCollection<Tab> _displayTabs;

        public MainNavigationViewModel()
        {
            //var savedUser = Preferences.Default.Get("User", new Users() { Id = 1, Name = "Admin" });
            User = new Users {Id = 1, Name = "Admin" };
            Title = "Main Navigation";
            Processes = new ObservableCollection<Processes>();
            DisplayTabs = new ObservableCollection<Tab>();
            restService = ServiceHelper.GetService<RestService>();
        }

        [RelayCommand]
        public async Task FetchData()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var data = await restService.GetUserProcesses(User);

                Processes.Clear();
                DisplayTabs.Clear();
                foreach (var process in data)
                {
                    Processes.Add(process);
                }
                Tab? Tasks = null;
                Tab? Calendar = null;
                Tab? Foam = null;

                var hasBasic = false;
                var hasProduction = false;
                var hasCalendar = false;
                var hasTasks = false;

                foreach (var type in BasicTabs)
                {
                    foreach(var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasBasic)
                        {
                            hasBasic = true;
                            DisplayTabs.Add(new Tab());
                        }
                        DisplayTabs.Add(Tab.FromProcess(process));
                    }
                }
                foreach (var type in ProductionTabs)
                {
                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasProduction)
                        {
                            hasProduction = true;
                            DisplayTabs.Add(new Tab());
                        }
                        DisplayTabs.Add(Tab.FromProcess(process));
                    }
                }
                foreach (var type in CalendarTabs)
                {
                    
                    foreach (var process in Processes.Where(p => p.Type == type))
                    {
                        if (!hasCalendar)
                        {
                            hasCalendar = true;
                            DisplayTabs.Add(new Tab());
                        }
                        if (Calendar == null)
                        {
                            Calendar = new Tab(101, "Calendar", null, null);
                            DisplayTabs.Add(Calendar);
                        }
                        var tab = Tab.FromProcess(process);
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
                            DisplayTabs.Add(new Tab());
                        }
                        if (Tasks == null)
                        {
                            Tasks = new Tab(100, "Tasks", null, null);
                            DisplayTabs.Add(Tasks);
                        }
                        var tab = Tab.FromProcess(process);
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
                            Tasks = new Tab(100, "Tasks", null, null);
                            DisplayTabs.Add(Tasks);
                        }
                        if (Foam == null)
                        {
                            Foam = new Tab(102, "Foam", null, null);
                            Foam.IsSubTab = true;
                            Tasks.AddChild(Foam);
                        }
                        var tab = Tab.FromProcess(process);
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
            ProcessesType.Products
        };

        private readonly List<ProcessesType> ProductionTabs = new List<ProcessesType>
        {
            ProcessesType.Models,
            ProcessesType.Patterns,
            ProcessesType.ProductCategories
        };

        private readonly List<ProcessesType> CalendarTabs = new List<ProcessesType>
        {
            ProcessesType.PickUpApt,
            ProcessesType.DeliverApt
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
        public async Task ToggleTab(Tab tab)
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

        private async Task ExpandTab(Tab tab)
        {
            if (tab.HasChildren)
            {
                var index = DisplayTabs.IndexOf(tab);
                foreach(var child in tab.Children)
                {
                    if (!DisplayTabs.Contains(child))
                    {
                        index++;
                        DisplayTabs.Insert(index, child);
                    }
                }
            }
        }

        private async Task CollapseTab(Tab tab)
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
        public async Task GoToSettings()
        {
            return;
        }

        [RelayCommand]
        public async Task GoToUserSelection()
        {
            return;
        }

    }
}
