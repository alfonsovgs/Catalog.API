using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.SchemaDefinitions
{
    public class ArtistEntitySchemaConfiguration : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            builder.ToTable("Artist", CatalogContext.DEFAULT_SCHEMA);
            builder.HasKey(p => p.ArtistId);
            builder.Property(p => p.ArtistId);

            builder.Property(p => p.ArtistName)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
