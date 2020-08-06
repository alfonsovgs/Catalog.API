using Catalog.Domain.Responses;

namespace Catalog.Domain.Mappers
{
    public interface IGenreMapper
    {
        GenreResponse Map(Genre genre);
    }
}