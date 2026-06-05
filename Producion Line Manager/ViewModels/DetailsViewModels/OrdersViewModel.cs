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
    public partial class OrdersViewModel : BaseEntityViewModel
    {

        private readonly RestService restService;

        private int Page = 1;
        private int TotalPages = 1;
        private int PageSize = 20;


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
        [ObservableProperty]
        private DateTime _createdDate;

        [ObservableProperty]
        private int _numberOfProducts = 0;
        [ObservableProperty]
        private int _numberOfFinishedProducts = 0;
        [ObservableProperty]
        private int _numberOfIncompleteProducts = 0;
        [ObservableProperty]
        private ObservableCollection<Products> _Products = new ObservableCollection<Products>();
        [ObservableProperty]
        private ObservableCollection<SaleChannel> _salesChannels = new ObservableCollection<SaleChannel>();
        [ObservableProperty]
        private ObservableCollection<ProductCategories> _productCategories = new ObservableCollection<ProductCategories>();
        public OrdersViewModel()
        {
            restService = ServiceHelper.GetService<RestService>();
            //this.ProductCategories = restService.Get<ProductCategories>();
            SalesChannels.Add(SaleChannel.Email);
            SalesChannels.Add(SaleChannel.InStore);
            SalesChannels.Add(SaleChannel.Phone);
            SalesChannels.Add(SaleChannel.Online);
            SalesChannels.Add(SaleChannel.SocialMedia);
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
            await restService.Post(newProduct);
            WeakReferenceMessenger.Default.Send(new OpenEntityMessage(newProduct));
        }


        public async void LoadEntity(Orders order)
        {
            Customers? customer = await restService.Get<Customers>(order.CustomerId);
            base.LoadEntity(order);
            CustomerId = order.CustomerId;
            if (customer != null)
            {
                CustomerName = customer.FirstName ?? String.Empty;
                CustomerLastName = customer.LastName ?? String.Empty;
                RequestResult<Orders>? otherOrders = await restService.Get<Customers, Orders>(customer.Id);
                CustomerOrders = otherOrders?.TotalCount ?? 0;
            }

            IsComplete = order.IsCompleted;
            SaleChannelValue = order.SaleChannel;
            CreatedDate = order.CreatedDate;

            await RefreshOrders();
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
                    null,
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
                foreach(Products product in result.Items)
                {
                    Products.Add(product);
                }
                Page++;
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}
