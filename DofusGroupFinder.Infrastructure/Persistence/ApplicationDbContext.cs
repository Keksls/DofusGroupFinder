using Microsoft.EntityFrameworkCore;
using DofusGroupFinder.Domain.Entities;

namespace DofusGroupFinder.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Dungeon> Dungeons => Set<Dungeon>();
        public DbSet<Listing> Listings => Set<Listing>();
        public DbSet<ListingCharacter> ListingCharacters => Set<ListingCharacter>();
        public DbSet<ListingGroupMember> ListingGroupMembers => Set<ListingGroupMember>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Pseudo)
                .IsUnique();

            // Character
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Account)
                .WithMany(a => a.Characters)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dungeon
            modelBuilder.Entity<Dungeon>()
                .HasMany(d => d.Listings)
                .WithOne(l => l.Dungeon)
                .HasForeignKey(l => l.DungeonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Listing
            modelBuilder.Entity<Listing>()
                .HasOne(l => l.Account)
                .WithMany(a => a.Listings)
                .HasForeignKey(l => l.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Listing>()
                .HasMany(l => l.ListingCharacters)
                .WithOne(lc => lc.Listing)
                .HasForeignKey(lc => lc.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Listing>()
                .HasMany(l => l.ListingGroupMembers)
                .WithOne(lgm => lgm.Listing)
                .HasForeignKey(lgm => lgm.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ListingCharacter
            modelBuilder.Entity<ListingCharacter>()
                .HasOne(lc => lc.Listing)
                .WithMany(l => l.ListingCharacters)
                .HasForeignKey(lc => lc.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListingCharacter>()
                .HasOne(lc => lc.Character)
                .WithMany(c => c.ListingCharacters)
                .HasForeignKey(lc => lc.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // ListingGroupMember
            modelBuilder.Entity<ListingGroupMember>()
                .HasOne(lgm => lgm.Listing)
                .WithMany(l => l.ListingGroupMembers)
                .HasForeignKey(lgm => lgm.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}