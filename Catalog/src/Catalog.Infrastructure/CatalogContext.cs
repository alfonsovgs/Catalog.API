﻿using Catalog.Domain;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.SchemaDefinitions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Catalog.Infrastructure
{
    public class CatalogContext : IdentityDbContext<User>, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "catalog";
        public DbSet<Item> Items { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ItemEntitySchemaDefinition());
            modelBuilder.ApplyConfiguration(new GenreEntitySchemaConfiguration());
            modelBuilder.ApplyConfiguration(new ArtistEntitySchemaConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}