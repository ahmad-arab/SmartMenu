using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Data.Entities;

namespace SmartMenu.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>,
        ApplicationUserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUserRole composite key
            builder.Entity<ApplicationUserRole>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            builder.Entity<MenuLanguage>(b =>
            {
                b.HasKey(ur => new { ur.MenuId, ur.LanguageId });

                b.HasOne(ur => ur.Menu)
                    .WithMany(u => u.MenuLanguages)
                    .HasForeignKey(ur => ur.MenuId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.MenuLanguages)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<MenuTitle>(b =>
            {
                b.HasKey(ur => new { ur.MenuId, ur.LanguageId });

                b.HasOne(ur => ur.Menu)
                    .WithMany(u => u.MenuTitles)
                    .HasForeignKey(ur => ur.MenuId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.MenuTitles)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<CategoryTitle>(b =>
            {
                b.HasKey(ur => new { ur.CategoryId, ur.LanguageId });

                b.HasOne(ur => ur.Category)
                    .WithMany(u => u.CategoryTitles)
                    .HasForeignKey(ur => ur.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.CategoryTitles)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<CategoryDescription>(b =>
            {
                b.HasKey(ur => new { ur.CategoryId, ur.LanguageId });

                b.HasOne(ur => ur.Category)
                    .WithMany(u => u.CategoryDescriptions)
                    .HasForeignKey(ur => ur.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.CategoryDescriptions)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<ItemTitle>(b =>
            {
                b.HasKey(ur => new { ur.ItemId, ur.LanguageId });

                b.HasOne(ur => ur.Item)
                    .WithMany(u => u.ItemTitles)
                    .HasForeignKey(ur => ur.ItemId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.ItemTitles)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<ItemDescription>(b =>
            {
                b.HasKey(ur => new { ur.ItemId, ur.LanguageId });

                b.HasOne(ur => ur.Item)
                    .WithMany(u => u.ItemDescriptions)
                    .HasForeignKey(ur => ur.ItemId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.ItemDescriptions)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<MenuLableText>(b =>
            {
                b.HasKey(ur => new { ur.MenuLableId, ur.LanguageId });

                b.HasOne(ur => ur.MenuLable)
                    .WithMany(u => u.MenuLableTexts)
                    .HasForeignKey(ur => ur.MenuLableId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.MenuLableTexts)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            builder.Entity<MenuCommandText>(b =>
            {
                b.HasKey(ur => new { ur.MenuCommandId, ur.LanguageId });

                b.HasOne(ur => ur.MenuCommand)
                    .WithMany(u => u.MenuCommandTexts)
                    .HasForeignKey(ur => ur.MenuCommandId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(ur => ur.Language)
                    .WithMany(r => r.MenuCommandTexts)
                    .HasForeignKey(ur => ur.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            // MenuCommandStaff mapping
            builder.Entity<MenuCommandStaff>(b =>
            {
                b.HasOne(x => x.MenuCommand)
                    .WithMany(x => x.MenuCommandStaffs)
                    .HasForeignKey(x => x.MenuCommandId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne(x => x.MenuStaff)
                    .WithMany(x => x.MenuCommandStaffs)
                    .HasForeignKey(x => x.MenuStaffId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageText> LanguageTexts { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuLanguage> MenuLanguages { get; set; }
        public DbSet<MenuTitle> MenuTitles { get; set; }
        public DbSet<CategoryTitle> CategoryTitles { get; set; }
        public DbSet<CategoryDescription> CategoryDescriptions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuStaff> MenuStaffs { get; set; }
        public DbSet<MenuStaffTimeSlot> MenuStaffTimeSlots { get; set; }

        public DbSet<ItemTitle> ItemTitles { get; set; }
        public DbSet<ItemDescription> ItemDescriptions { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<MenuLable> MenuLables { get; set; }
        public DbSet<MenuLableText> MenuLableTexts { get; set; }

        public DbSet<MenuCommand> MenuCommands { get; set; }
        public DbSet<MenuCommandText> MenuCommandTexts { get; set; }
        public DbSet<MenuCommandStaff> MenuCommandStaffs { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

    }
}