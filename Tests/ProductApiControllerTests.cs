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
    public class ProductApiControllerTests
    {
        private readonly Mock<IRepository<Product>> mockRepo;
        private readonly ProductApiController controller;
        private List<Product> products;

        public ProductApiControllerTests()
        {
            mockRepo = new Mock<IRepository<Product>>();
            controller = new ProductApiController(mockRepo.Object);

            products = new List<Product>()
            {
                new Product{ Id=1, Name="Product 1", Price = 10, Stock = 5, Color = "Red" },
                new Product{ Id=2, Name="Product 2", Price = 20, Stock = 15, Color = "Yellow" },
                new Product{ Id=3, Name="Product 3", Price = 30, Stock = 25, Color = "Black" }
            };
        }

        [Fact]
        public async void GetProducts_ActionExecutes_ReturnOkResultWithProduct()
        {
            mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);
            var result = await controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(3, returnProducts.Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;
            mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await controller.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            Product product = products.FirstOrDefault(x => x.Id == productId);
            mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await controller.GetProduct(productId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
            Assert.NotEmpty(returnProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = products.First(x => x.Id == productId);
            var result = controller.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);
            mockRepo.Setup(x => x.Update(product));

            var result = controller.PutProduct(productId, product);

            mockRepo.Verify(x => x.Update(product), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PostProduct_ActionExecutes_ReturnCeratedAtAction()
        {
            var product = products.First();
            mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await controller.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
            mockRepo.Verify(x => x.Create(product), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;
            mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await controller.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_IdValid_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);
            mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await controller.DeleteProduct(productId);

            Assert.IsType<NoContentResult>(result.Result);
            mockRepo.Verify(x => x.Delete(product), Times.Once);
        }
    }
}