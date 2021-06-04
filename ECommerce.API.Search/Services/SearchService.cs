using ECommerce.API.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService _ordersServices;
        private readonly IProductsService _productsService;
        private readonly ICustomersService _customersService;

        public SearchService(IOrdersService ordersServices, IProductsService productsService, ICustomersService customersService)
        {
            _ordersServices = ordersServices;
            _productsService = productsService;
            _customersService = customersService;
        }


        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var customerResult = await _customersService.GetCustomerASync(customerId);
            var ordersResult = await _ordersServices.GetOrdersAsync(customerId);
            var productsResult = await _productsService.GetProductsAsync();


            if (ordersResult.IsSuccess)
            {

                foreach (var order  in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ?
                            productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId).Name
                            : "Product information is not available at the moment";
                    }
                }

                var result = new
                {
                    Customer = customerResult.IsSuccess ? customerResult.Customer : new { Name="Customer Information is not available at this moment" },
                    Orders = ordersResult.Orders
                };
                return (true, result);
            }
            return (false, null);
        }

        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchCustomerAsync()
        {
            var customersResult = await _customersService.GetCustomersASync();
            if (customersResult.IsSuccess)
            {
                return (true, customersResult.Customers);
            }
            return (false, null);
        }
    }
}
