using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using Models.Attributes;
using Models.Finances;
using Models.Production;
using Producion_Line_Manager.Helpers;
using Producion_Line_Manager.Messages;
using Producion_Line_Manager.Services;
using Producion_Line_Manager.Views.DetailsViews;
using System.Collections.ObjectModel;

namespace Producion_Line_Manager.ViewModels.DetailsViewModels
{
    public partial class CustomersViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        private int Page = 1;
        private int TotalPages = 1;
        private int PageSize = 20;

        private int DraftPage = 1;
        private int DraftTotalPages = 1;
        private int DraftPageSize = 20;


        [ObservableProperty]
        private string _customerName = string.Empty;
        [ObservableProperty]
        private string _customerLastName = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _telephone = string.Empty;
        [ObservableProperty]
        private string _taxNumber = string.Empty;
        [ObservableProperty]
        private CustomerType _customerType = CustomerType.Retail;

        [ObservableProperty]
        private int _numberOfDraftOrders = 0;
        [ObservableProperty]
        private int _numberOfOrders = 0;
        [ObservableProperty]
        private int _numberOfActiveOrders = 0;
        [ObservableProperty]
        private int _numberOfCompletedOrders = 0;
        [ObservableProperty]
        private ObservableCollection<Orders> _orders = new ObservableCollection<Orders>();
        [ObservableProperty]
        private ObservableCollection<Orders> _draftOrders = new ObservableCollection<Orders>();
        [ObservableProperty]
        private ObservableCollection<CustomerType> _customerTypes = new ObservableCollection<CustomerType>();
        public CustomersViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
            CustomerTypes.Add(CustomerType.Retail);
            CustomerTypes.Add(CustomerType.Wholesale);
        }

        [RelayCommand]
        public async Task CreateNewOrder()
        {
            if (Item == null) { return; }
            var newOrder = new Orders
            {
                CustomerId = Item.Id,
                IsDraft = true,
                IsCompleted = false,
                CreatedDate = DateTime.Now,
            };
            await restService.Post(newOrder);
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(newOrder));
        }

        [RelayCommand]
        public async Task OpenOrder(Orders order)
        {
            if (order == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(order));
        }


        public void LoadEntity(Customers customer)
        {
            base.LoadEntity(customer);
            CustomerName = customer.FirstName ?? string.Empty;
            CustomerLastName = customer.LastName ?? string.Empty;
            Email = customer.Email ?? string.Empty;
            Telephone = customer.Telephone ?? string.Empty;
            TaxNumber = customer.TaxNumber ?? string.Empty;
            CustomerType = customer.Type ?? CustomerType.Retail;

            RefreshOrders();
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Customers customer)
            {
                customer.FirstName = CustomerName;
                customer.LastName = CustomerLastName;
                customer.Email = Email;
                customer.Telephone = Telephone;
                customer.TaxNumber = TaxNumber;
                customer.Type = CustomerType;

            }
        }


        [RelayCommand]
        public async Task RefreshOrders()
        {
            Page = 1;
            PageSize = 20;
            TotalPages = 1;

            DraftPage = 1;
            DraftPageSize = 20;
            DraftTotalPages = 1;

            await LoadDrafts();
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
                IRequestResult? result = await restService.Get<Customers, Orders>(Id, parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                TotalPages = result.TotalPages;
                NumberOfOrders = result.TotalCount;
                foreach(Orders order in result.Items)
                {
                    Orders.Add(order);
                }
                parameters.PageSize = 1;
                parameters.Filters.Clear();
                parameters.Filters.Add(FilterType.Active);
                IRequestResult? activeResult = await restService.Get<Customers, Orders>(Id, parameters);
                NumberOfActiveOrders = activeResult?.TotalCount ?? 0;
                parameters.Filters.Clear();
                parameters.Filters.Add(FilterType.Complete);
                IRequestResult? completeResult = await restService.Get<Customers, Orders>(Id, parameters);
                NumberOfCompletedOrders = completeResult?.TotalCount ?? 0;
                Page++;
            }
            finally
            {
                IsBusy = false;
            }

        }

        public async Task LoadDrafts()
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
                IRequestResult? result = await restService.Get<Customers, Orders>(Id, parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                DraftTotalPages = result.TotalPages;
                NumberOfDraftOrders = result.TotalCount;
                foreach (Orders order in result.Items)
                {
                    DraftOrders.Add(order);
                }
                DraftPage++;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
