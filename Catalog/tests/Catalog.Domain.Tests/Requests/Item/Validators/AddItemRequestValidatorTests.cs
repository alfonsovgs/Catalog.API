using Catalog.Domain.Requests.Item;
using Catalog.Domain.Requests.Item.Validators;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Requests.Artists;
using Catalog.Domain.Requests.Genre;
using Catalog.Domain.Responses;
using Catalog.Domain.Services;
using Xunit;
using Moq;

namespace Catalog.Domain.Tests.Requests.Item.Validators
{
    public class AddItemRequestValidatorTests
    {
        private readonly Mock<IArtistService> _artistServiceMock;
        private readonly Mock<IGenreService> _genreServiceMock;
        private readonly AddItemRequestValidator _validator;

        public AddItemRequestValidatorTests()
        {
            _artistServiceMock = new Mock<IArtistService>();
            _artistServiceMock
                .Setup(x => x.GetArtistAsync(It.IsAny<GetArtistRequest>()))
                .ReturnsAsync(() => new ArtistResponse());

            _genreServiceMock = new Mock<IGenreService>();
            _genreServiceMock
                .Setup(x => x.GetGenreAsync(It.IsAny<GetGenreRequest>()))
                .ReturnsAsync(() => new GenreResponse());

            _validator = new AddItemRequestValidator(_artistServiceMock.Object, _genreServiceMock.Object);
        }

        [Fact]
        public void should_have_error_when_ArtistId_is_null()
        {
            var additemRequest = new AddItemRequest
            {
                Price = new Price(),
            };

            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, additemRequest);
        }

         [Fact]
        public void should_have_error_when_GenreId_is_null()
        {
            var additemRequest = new AddItemRequest
            {
                Price = new Price(),
            };

            _validator.ShouldHaveValidationErrorFor(x => x.GenreId, additemRequest);
        }

        [Fact]
        public void should_have_error_when_ArtistId_doesnt_exists()
        {
            _artistServiceMock
                .Setup(x => x.GetArtistAsync(It.IsAny<GetArtistRequest>()))
                .ReturnsAsync(() => null);

            var addItemRequest = new AddItemRequest {Price = new Price(), ArtistId = Guid.NewGuid()};
            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, addItemRequest);
        }

        [Fact]
        public void should_have_error_when_GenreId_doesnt_exist()
        {
            _genreServiceMock
                .Setup(x => x.GetGenreAsync(It.IsAny<GetGenreRequest>()))
                .ReturnsAsync(() => null);

            var addItemRequest = new AddItemRequest { Price = new Price(), GenreId = Guid.NewGuid() };
            _validator.ShouldHaveValidationErrorFor(x => x.GenreId, addItemRequest);
        }
    }
}
