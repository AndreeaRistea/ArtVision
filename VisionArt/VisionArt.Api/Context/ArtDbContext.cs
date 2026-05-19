using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VisionArt.Api.Models;

namespace VisionArt.Api.Context;

public class ArtDbContext : IdentityDbContext<User>
{
    public ArtDbContext(DbContextOptions<ArtDbContext> options) : base(options) { }

    public DbSet<ArtWork> ArtWorks { get; set; }
    public DbSet<FavoriteArt> Favorites { get; set; }
}

