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
    public partial class OrdersViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        private int Page = 1;
        private int TotalPages = 1;
        private int PageSize = 20;

        private int DraftPage = 1;
        private int DraftTotalPages = 1;
        private int DraftPageSize = 20;


        [ObservableProperty]
        private int _customerId = 0;
        [ObservableProperty]
        private string _customerName = string.Empty;
        [ObservableProperty]
        private string _customerLastName = string.Empty;
        [ObservableProperty]
        private int _customerOrders = 0;
        [ObservableProperty]
        private bool _isComplete = false;
        [ObservableProperty]
        private SaleChannel? _saleChannelValue = null;


        private Customers Customer;

        [ObservableProperty]
        private int _numberOfDraftProducts = 0;
        [ObservableProperty]
        private int _numberOfProducts = 0;
        [ObservableProperty]
        private int _numberOfFinishedProducts = 0;
        [ObservableProperty]
        private int _numberOfIncompleteProducts = 0;
        [ObservableProperty]
        private ObservableCollection<Products> _draftProducts = new ObservableCollection<Products>();
        [ObservableProperty]
        private ObservableCollection<Products> _products = new ObservableCollection<Products>();
        [ObservableProperty]
        private ObservableCollection<SaleChannel> _salesChannels = new ObservableCollection<SaleChannel>();
        [ObservableProperty]
        private ObservableCollection<ProductCategories> _productCategories = new ObservableCollection<ProductCategories>();

        [ObservableProperty]
        private bool _canChangeCustomer = false;
        public bool CannotChangeCustomer => !CanChangeCustomer;
        [ObservableProperty]
        private string _searchQuerry = string.Empty;
        [ObservableProperty]
        private SearchType _searchType = SearchType.General;
        [ObservableProperty]
        private ObservableCollection<SearchType> _searchOptions = new();
        [ObservableProperty]
        private ObservableCollection<Customers> _suggestedCustomers = new();
        private CancellationTokenSource? _searchCancellationTokenSource;

        public OrdersViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
            //this.ProductCategories = restService.Get<ProductCategories>();
            SalesChannels.Add(SaleChannel.Email);
            SalesChannels.Add(SaleChannel.InStore);
            SalesChannels.Add(SaleChannel.Phone);
            SalesChannels.Add(SaleChannel.Online);
            SalesChannels.Add(SaleChannel.SocialMedia);

            SearchOptions.Add(SearchType.General);
            SearchOptions.Add(SearchType.Name);
            SearchOptions.Add(SearchType.Email);
            SearchOptions.Add(SearchType.TaxNumber);
            SearchOptions.Add(SearchType.PhoneNumber);

            Customer = new Customers();
            Customer.LastName = "No Customer";
        }

        partial void OnSaleChannelValueChanged(SaleChannel? value)
        {
            if (Item is Orders order)
            {
                order.SaleChannel = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnIsCompleteChanged(bool value)
        {
            if (Item is Orders order)
            {
                order.IsCompleted = value;
                TriggerDebouncedSave();
            }
        }

        partial void OnCustomerIdChanged(int value)
        {
            if (Item is Orders order)
            {
                order.CustomerId = value;
                TriggerDebouncedSave();
            }
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

        async partial void OnSearchTypeChanged(SearchType value)
        {
            await RefreshItems();
        }

        [RelayCommand]
        public async Task ChangeCustomer()
        {
            CanChangeCustomer = true;
            OnPropertyChanged(nameof(CannotChangeCustomer));
        }

        [RelayCommand]
        public async Task SelectCustomer(Customers customer)
        {
            Customer = customer;
            await LoadCustomer(customer);
        }

        [RelayCommand]
        public async Task RefreshItems()
        {
            if (IsBusy) { return; }
            IsBusy = true;
            try
            {
                SuggestedCustomers.Clear();
                var parameters = new RequestParameters(
                    null,
                    SearchType,
                    SearchQuerry,
                    1,
                    5,
                    SortType.IdDecending);
                RequestResult<Customers>? result = await restService.Get<Customers>(parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                foreach (var item in result.Items)
                {
                    SuggestedCustomers.Add(item);

                }
            }
            finally
            {
                IsBusy = false;
            }
            
        }


        [RelayCommand]
        public async Task CreateNewProduct()
        {
            if (Item == null) { return; }
            var newProduct = new Products
            {
                OrderId = Item.Id,
                CategoryId = 0,
                ExpectedFinishDate = DateTime.Now.AddDays(7),
                ExpectedStartDate = DateTime.Now,
                IsDraft = true,
                IsCompleted = false,
                CreatedDate = DateTime.Now,
            };
            var result = await restService.Post(newProduct);
            newProduct.Id = result?.Id ?? 0;
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(newProduct));
        }

        [RelayCommand]
        public async Task OpenProduct(Products product)
        {
            if (product == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(product));
        }

        [RelayCommand]
        public async Task OpenCostumer()
        {
            if (Customer == null) { return; }

            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(Customer));
        }

        public async void LoadEntity(Orders order)
        {
            Customers? customer = await restService.Get<Customers>(order.CustomerId);
            base.LoadEntity(order);
            await LoadCustomer(customer);

            IsComplete = order.IsCompleted;
            SaleChannelValue = order.SaleChannel;
            CreatedDate = order.CreatedDate;

            await RefreshOrders();
            if (CanChangeCustomer)
            {
                await RefreshItems();
            }
        }

        private async Task LoadCustomer(Customers? customer)
        {
            if (customer != null)
            {
                CanChangeCustomer = false;
                OnPropertyChanged(nameof(CannotChangeCustomer));
                CustomerId = customer.Id;
                Customer = customer;
                CustomerName = customer.FirstName ?? String.Empty;
                CustomerLastName = customer.LastName ?? String.Empty;
                RequestResult<Orders>? otherOrders = await restService.Get<Customers, Orders>(customer.Id);
                CustomerOrders = otherOrders?.TotalCount ?? 0;
            }
        }

        public override void SaveEntity()
        {
            base.SaveEntity();
            if (Item is Orders order)
            {
                order.SaleChannel = SaleChannelValue;
                order.IsCompleted = IsComplete;
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
                IRequestResult? result = await restService.Get<Orders, Products>(Id, parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                TotalPages = result.TotalPages;
                NumberOfProducts = result.TotalCount;
                foreach (Products product in result.Items)
                {
                    Products.Add(product);
                }
                parameters.PageSize = 1;
                parameters.Filters.Clear();
                parameters.Filters.Add(FilterType.Complete);
                IRequestResult? activeResult = await restService.Get<Orders, Products>(Id, parameters);
                NumberOfFinishedProducts = activeResult?.TotalCount ?? 0;
                parameters.Filters.Clear();
                parameters.Filters.Add(FilterType.Incomplete);
                IRequestResult? completeResult = await restService.Get<Orders, Products>(Id, parameters);
                NumberOfIncompleteProducts = completeResult?.TotalCount ?? 0;
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
                IRequestResult? result = await restService.Get<Orders, Products>(Id, parameters);
                if (result == null)
                {
                    IsBusy = false;
                    return;
                }
                DraftTotalPages = result.TotalPages;
                NumberOfDraftProducts = result.TotalCount;
                foreach (Products product in result.Items)
                {
                    DraftProducts.Add(product);
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

