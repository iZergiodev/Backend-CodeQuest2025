using CodeQuestBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<UserSubcategoryFollow> UserSubcategoryFollows { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<StarDustPointsHistory> StarDustPointsHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure unique constraint for Likes (one like per user per post)
        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.PostId })
            .IsUnique();

        // Configure cascade delete behaviors
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Subcategory relationships
        modelBuilder.Entity<Subcategory>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure UserSubcategoryFollow relationships
        modelBuilder.Entity<UserSubcategoryFollow>()
            .HasOne(usf => usf.User)
            .WithMany(u => u.FollowedSubcategories)
            .HasForeignKey(usf => usf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSubcategoryFollow>()
            .HasOne(usf => usf.Subcategory)
            .WithMany()
            .HasForeignKey(usf => usf.SubcategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure unique constraints for follows (one follow per user per subcategory)
        modelBuilder.Entity<UserSubcategoryFollow>()
            .HasIndex(usf => new { usf.UserId, usf.SubcategoryId })
            .IsUnique();

        // Configure Bookmark relationships
        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.Post)
            .WithMany(p => p.Bookmarks)
            .HasForeignKey(b => b.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure unique constraint for Bookmarks (one bookmark per user per post)
        modelBuilder.Entity<Bookmark>()
            .HasIndex(b => new { b.UserId, b.PostId })
            .IsUnique();
    }
}