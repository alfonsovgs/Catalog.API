using Catalog.Domain.Mappers;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Services;
using Catalog.Fixtures;
using Catalog.Infrastructure.Repositories;
using Shouldly;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Catalog.Domain.Tests.Services
{
    public class ItemServiceTests : IClassFixture<CatalogContextFactory>
    {
        private readonly ItemRepository _itemRepository;
        private readonly IItemMapper _mapper;
        private readonly Mock<LoggerAbstraction<ItemService>> _logger;

        public ItemServiceTests(CatalogContextFactory catalogContextFactory, ITestOutputHelper outputHelper)
        {
            _itemRepository = new ItemRepository(catalogContextFactory.ContextInstance);
            _mapper = catalogContextFactory.ItemMapper;
            _logger = new Mock<LoggerAbstraction<ItemService>>();
            _logger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<Exception>(), It.IsAny<string>()))
                .Callback((LogLevel logLevel, Exception exception, string information) =>
                    outputHelper.WriteLine($"{logLevel}:{information}"));
        }

        [Fact]
        public async Task getitems_should_return_right_data()
        {
            ItemService stu = new ItemService(_itemRepository, _mapper, null, null, null);

            var result = await stu.GetItemsAsync();
            result.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("b5b05534-9263-448c-a69e-0bbd8b3eb90e")]
        public async Task getitem_should_return_right_data(string guid)
        {
            ItemService stu = new ItemService(_itemRepository, _mapper, null, null, null);
            var result = await stu.GetItemAsync(new GetItemRequest { Id = new Guid(guid) });
            result.Id.ShouldBe(new Guid(guid));
        }

        [Fact]
        public void getitem_should_thrown_exception_with_null_id()
        {
            ItemService stu = new ItemService(_itemRepository, _mapper, null, null, null);
            stu.GetItemAsync(null).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async Task additem_should_add_right_entity()
        {
            var testItem = new AddItemRequest
            {
                Name = "Test album",
                GenreId = new Guid("c04f05c0-f6ad-44d1-a400-3375bfb5dfd6"),
                ArtistId = new Guid("f08a333d-30db-4dd1-b8ba-3b0473c7cdab"),
                Price = new Price { Amount = 13, Currency = "EUR" },
            };

            IItemService stu = new ItemService(_itemRepository, _mapper, null, null, null);
            var result = await stu.AddItemAsync(testItem);

            result.Name.ShouldBe(testItem.Name);
            result.Description.ShouldBe(testItem.Description);
            result.GenreId.ShouldBe(testItem.GenreId);
            result.ArtistId.ShouldBe(testItem.ArtistId);
            result.Price.Amount.ShouldBe(testItem.Price.Amount);
            result.Price.Currency.ShouldBe(testItem.Price.Currency);
        }

        [Fact]
        public async Task edititem_should_add_right_entity()
        {
            var testItem = new EditItemRequest
            {
                Id = new Guid("b5b05534-9263-448c-a69e-0bbd8b3eb90e"),
                Name = "Test album",
                GenreId = new Guid("c04f05c0-f6ad-44d1-a400-3375bfb5dfd6"),
                ArtistId = new Guid("f08a333d-30db-4dd1-b8ba-3b0473c7cdab"),
                Price = new Price { Amount = 13, Currency = "EUR" },
            };

            IItemService stu = new ItemService(_itemRepository, _mapper, null, null, null);
            var result = await stu.EditItemAsync(testItem);

            result.Name.ShouldBe(testItem.Name);
            result.Description.ShouldBe(testItem.Description);
            result.GenreId.ShouldBe(testItem.GenreId);
            result.ArtistId.ShouldBe(testItem.ArtistId);
            result.Price.Amount.ShouldBe(testItem.Price.Amount);
            result.Price.Currency.ShouldBe(testItem.Price.Currency);
        }

        [Fact]
        public async Task additem_should_log_information()
        { 
            var request = new AddItemRequest
            {
                Name = "Test album",
                GenreId = new Guid("c04f05c0-f6ad-44d1-a400-3375bfb5dfd6"),
                ArtistId = new Guid("f08a333d-30db-4dd1-b8ba-3b0473c7cdab"),
                Price = new Price { Amount = 13, Currency = "EUR" },
            };

            var sut = new ItemService(_itemRepository, _mapper, null, _logger.Object, null);
            await sut.AddItemAsync(request);

            _logger
                .Verify(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<Exception>(), It.IsAny<string>()), Times.AtMost(2));
        }

    }
}
