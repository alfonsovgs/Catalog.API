using System.Linq;
using Catalog.Domain;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Responses;
using Catalog.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Catalog.API.ResponseModels;
using Xunit;

namespace Catalog.API.Tests.Controllers
{
    public class ItemControllerTest : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        private readonly InMemoryApplicationFactory<Startup> _factory;

        public ItemControllerTest(InMemoryApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/items")]
        public async Task get_should_return_success(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task get_by_id_should_return_item_data()
        {
            const string id = "86bff4f7-05a7-46b6-ba73-d43e2c45840f";
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync($"/api/items/{id}");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity = JsonConvert.DeserializeObject<Item>(responseContent);
            responseEntity.ShouldNotBeNull();
        }

        [Theory]
        [LoadData("item")]
        public async Task add_should_create_new_record(AddItemRequest request)
        {
            //var request = new AddItemRequest
            //{
            //    Name = "Test album",
            //    Description = "Description",
            //    LabelName = "Label name",
            //    Price = new Price { Amount = 13, Currency = "EUR" },
            //    PictureUri = "https://mycdn.com/pictures/32423423",
            //    ReleaseDate = DateTimeOffset.Now,
            //    AvailableStock = 6,
            //    GenreId = new Guid("c04f05c0-f6ad-44d1-a400-3375bfb5dfd6"),
            //    ArtistId = new Guid("f08a333d-30db-4dd1-b8ba-3b0473c7cdab"),
            //};

            var client = _factory.CreateClient();

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/items", httpContent);

            response.EnsureSuccessStatusCode();
            response.Headers.Location.ShouldNotBeNull();
        }

        [Theory]
        [LoadData("item")]
        public async Task update_should_modify_existing_item(EditItemRequest request)
        {
            //var request = new EditItemRequest
            //{
            //     Id = new Guid("b5b05534-9263-448c-a69e-0bbd8b3eb90e"),
            //    Name = "Test album",
            //    Description = "Description updated",
            //    LabelName = "Label name",
            //    Price = new Price { Amount = 50, Currency = "EUR" },
            //    PictureUri = "https://mycdn.com/pictures/32423423",
            //    ReleaseDate = DateTimeOffset.Now,
            //    AvailableStock = 6,
            //    GenreId = new Guid("c04f05c0-f6ad-44d1-a400-3375bfb5dfd6"),
            //    ArtistId = new Guid("f08a333d-30db-4dd1-b8ba-3b0473c7cdab"),
            //};

            var client = _factory.CreateClient();

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/items/{request.Id}", httpContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity = JsonConvert.DeserializeObject<Item>(responseContent);
            responseEntity.Name.ShouldBe(request.Name);
            responseEntity.Description.ShouldBe(request.Description);
            responseEntity.GenreId.ShouldBe(request.GenreId);
            responseEntity.ArtistId.ShouldBe(request.ArtistId);
        }

        [Theory]
        [LoadData("item")]
        public async Task get_by_id_should_return_right_data(Item request)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/api/items/{request.Id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity = JsonConvert.DeserializeObject<ItemResponse>(responseContent);
            responseEntity.Name.ShouldBe(request.Name);
            responseEntity.Description.ShouldBe(request.Description);
            responseEntity.Price.Amount.ShouldBe(request.Price.Amount);
            responseEntity.Price.Currency.ShouldBe(request.Price.Currency);
            responseEntity.Format.ShouldBe(request.Format);
            responseEntity.PictureUri.ShouldBe(request.PictureUri);
            responseEntity.GenreId.ShouldBe(request.GenreId);
            responseEntity.ArtistId.ShouldBe(request.ArtistId);
        }

        [Theory]
        [InlineData("/api/item/?pageSize=1&pageIndex=0", 1, 0)]
        [InlineData("/api/item/?pageSize=2&pageIndex=0", 2, 0)]
        [InlineData("/api/item/?pageSize=1&pageIndex=1", 1, 0)]
        public async Task get_should_return_paginated_data(string url, int pageSize, int pageIndex)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity =
                JsonConvert.DeserializeObject<PaginatedItemsResponseModel<ItemResponse>>(responseContent);

            responseEntity.PageIndex.ShouldBe(pageIndex);
            responseEntity.PageSize.ShouldBe(pageSize);
            responseEntity.Data.Count().ShouldBe(pageSize);
        }
    }
}
