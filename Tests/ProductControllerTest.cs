using Microsoft.AspNetCore.Mvc;
using Moq;
using MvcWebApp.Controllers;
using MvcWebApp.Models;
using MvcWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            products = new List<Product>()
            {
                new Product{ Id=1, Name="Product 1", Price = 10, Stock = 5, Color = "Red" },
                new Product{ Id=2, Name="Product 2", Price = 20, Stock = 15, Color = "Yellow" },
                new Product{ Id=3, Name="Product 3", Price = 30, Stock = 25, Color = "Black" }
            };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);

            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(3, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInvalid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int produtcId)
        {
            Product product = products.FirstOrDefault(x => x.Id == produtcId);
            _mockRepo.Setup(x => x.GetById(produtcId)).ReturnsAsync(product);

            var result = await _controller.Details(produtcId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var productResult = Assert.IsType<Product>(viewResult.Model);

            Assert.Equal(product.Id, productResult.Id);
            Assert.Equal(product.Price, productResult.Price);
        }
    }
}
