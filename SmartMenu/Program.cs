using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;
using SmartMenu.Middlewares;
using SmartMenu.Repositories.Category;
using SmartMenu.Repositories.Item;
using SmartMenu.Repositories.Language;
using SmartMenu.Repositories.Menu;
using SmartMenu.Repositories.MenuCommand;
using SmartMenu.Repositories.MenuLable;
using SmartMenu.Repositories.MenuStaff;
using SmartMenu.Repositories.Tenant;
using SmartMenu.Repositories.User;
using SmartMenu.Services.Category;
using SmartMenu.Services.FileUpload;
using SmartMenu.Services.Item;
using SmartMenu.Services.Language;
using SmartMenu.Services.Menu;
using SmartMenu.Services.MenuCommand;
using SmartMenu.Services.MenuLable;
using SmartMenu.Services.MenuStaff;
using SmartMenu.Services.PublicMenu;
using SmartMenu.Services.Qr;
using SmartMenu.Services.Tenant;
using SmartMenu.Services.Theme;
using SmartMenu.Services.User;
using SmartMenu.Services.Whatsapp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register file upload service
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Register TwilioWhatsappService for IWhatsappService
builder.Services.AddTransient<IWhatsappService, TwilioWhatsappService>();

// Register QR Code service
builder.Services.AddSingleton<IQrCodeService, QrCodeService>();

// Register Theme Service
builder.Services.AddSingleton<IThemeService, ThemeService>();

// Register ApplicationDbContext with your database provider (e.g., SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services and configure to use your custom entities and DbContext
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Optional: configure password, lockout, etc.
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    // Optional: configure cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// Register Repositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IMenuLableRepository, MenuLableRepository>();
builder.Services.AddScoped<IMenuCommandRepository, MenuCommandRepository>();
builder.Services.AddScoped<IMenuStaffRepository, MenuStaffRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();

// Register Domain Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IMenuLableService, MenuLableService>();
builder.Services.AddScoped<IMenuCommandService, MenuCommandService>();
builder.Services.AddScoped<IMenuStaffService, MenuStaffService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IPublicMenuService, PublicMenuService>();

builder.Services.AddTransient<DbSeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    seeder.ApplyMigrationsAndSeed();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
