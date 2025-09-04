# SmartMenu

SmartMenu is an open-source, multi-tenant platform for building beautiful, multilingual, and interactive online menus. Tenants (organizations) can create multiple menus, design their look and feel, define categories and items, add informational labels, wire command buttons to staff, and communicate with customers through integrated messaging. It ships with a focused Tenant Admin experience, a flexible theming system, and a clean, extensible architecture on ASP.NET Core (.NET 8).

- .NET 8, ASP.NET Core MVC with Razor views
- Entity Framework Core with ASP.NET Identity (multi-tenant)
- SQL Server compatible
- Modern UI (Bootstrap 5, jQuery, DataTables, SweetAlert2, Select2)
- Extensible services (Theme, QR, WhatsApp)

## Highlights

- Multi-tenant by design
  - Each user belongs to a Tenant
  - Menus, languages, staff, and content are scoped per tenant
- Unlimited menus per tenant
- Multilingual menus
  - Titles and descriptions per language; one default language per tenant
- Theme Designer
  - Switch themes and live-edit theme JSON models per menu
- Category and Item management
  - Images, titles, descriptions, availability, and price
- Labels and Commands
  - Labels: multi-language informational blocks with icons
  - Commands: action buttons with per-language titles, optional customer message, routed to selected staff
- Staff and Schedules
  - Register staff and define per-day time slots; bulk apply schedules
- Messaging and Automation
  - Pluggable WhatsApp service to notify or converse with staff/customers
- QR Codes
  - Generate per-menu QR PNGs that deep-link to the public menu view
- Admin Areas
  - Global Admin: manage Tenants and assign users
  - Tenant Admin: everything required to build and operate menus

## Architecture

- Web
  - Controllers/Views for Admin and TenantAdmin flows
  - Public Menu view endpoint for customers
- Data
  - EF Core DbContext with Identity (users/roles)
  - Soft multi-tenancy via TenantId scoping and authorization
- Domain
  - Menus, Categories, Items, Languages, Labels, Commands, Staff, Schedules
- Services (interfaces first, replaceable)
  - IThemeService: returns theme models and defaults
  - IQrCodeService: generates QR PNGs for public menu URLs
  - IWhatsappService: abstracted integration for outbound messaging

Key security boundary: TenantAdmin endpoints always resolve tenant context from the authenticated user’s TenantId claim and filter queries accordingly.

## Screens and Flows (Tenant Admin)

- Menus
  - List, Create (image + per-language titles), Edit, Delete
  - Open Theme Designer; Preview Public Menu
  - Generate QR codes
- Categories
  - Per menu: Create/Edit (image + per-language title/description), Delete
- Items
  - Per category: Create/Edit (image, price, availability + per-language title/description), Delete
- Labels
  - Per menu: icon + per-language text
- Commands
  - Per menu: icon + per-language title, optional customer message, route to selected staff; test-send
- Staff
  - Create/Edit/Delete; Register (send onboarding); Edit schedules; Bulk schedule
- Languages
  - Per tenant: Create/Edit/Delete, mark one as default

## Tech Stack

- Backend: ASP.NET Core (.NET 8), EF Core, ASP.NET Identity
- Views: Razor, Bootstrap 5, jQuery, DataTables, Select2, SweetAlert2
- AuthZ: Role-based (TenantAdmin, plus Global Admin area)
- Storage: SQL Server (LocalDB/Express/Full)
- Optional Integrations: WhatsApp via IWhatsappService

## Getting Started

Prerequisites:
- Visual Studio 2022 (17.8+) or .NET 8 SDK
- SQL Server (LocalDB/Express/Developer)
- Node is not required (client libraries via CDN)

1) Clone
- git clone https://github.com/ahmad-arab/SmartMenu.git

2) Configure settings
- Copy appsettings.Template.json to appsettings.Development.json
- Set ConnectionStrings:DefaultConnection to your SQL Server
- Provide any keys required by your QR/WhatsApp implementations (if applicable)

3) Create database and apply migrations
- Visual Studio
  - Open the solution and select the SmartMenu startup project
  - Open __Tools > NuGet Package Manager > Package Manager Console__
  - Run: Update-Database
- CLI
  - dotnet ef database update

4) Run
- Press F5 or use __Debug > Start Debugging__ (IIS Express or Kestrel profile)
- The app will start and apply tenant-scoped behavior on authenticated areas

5) First admin and tenant
- Register/sign-in a user
- Make the user a Global Admin and/or TenantAdmin via your preferred method:
  - Global Admin UI (Tenants): create a tenant, assign users
  - Or update the role/user-roles in the database (ASP.NET Identity tables)
- As a TenantAdmin, open the Tenant Admin area and start creating menus

Note: The solution uses claims to scope tenant features. The TenantId claim should be present on authenticated users to access TenantAdmin endpoints.

## Public Menu and QR

- Customers access a menu via: /Menu/View?menuId={id}
- QR PNGs can be generated from Tenant Admin > Menu Page > Generate QR
- The QR endpoint encodes absolute URLs, e.g., /Menu/View?menuId={id}&identifier={optional}

## Theming

- Theme Designer allows live selection of:
  - Menu Theme (MenuThemeKey + JSON model)
  - Category Card Theme
  - Item Card Theme
  - Label Theme
- Implement your own IThemeService to add themes or defaults

## Messaging

- IWhatsappService is an abstraction to plug a concrete provider (Cloud API, Twilio, custom gateway)
- Commands can send test messages to the selected staff
- Staff registration sends onboarding messages and awaits reply to complete the flow

## Extensibility

- Replaceable services (QR, WhatsApp, Themes)
- Strong, explicit EF mappings for multilingual text entities (composite keys)
- Clear separation of Tenant Admin vs Global Admin boundaries
- MVC/Razor views with unobtrusive JavaScript, ready for componentization

## Repository Structure (high-level)

- Data/
  - ApplicationDbContext.cs, Entities (Tenant, Menu, Category, Item, Language, Label, Command, Staff, Schedules, Identity)
- Controllers/
  - TenantAdminController (tenant-scoped operations)
  - Admin (global tenant management)
  - Menu (public menu viewing and actions)
- Views/
  - TenantAdmin/* (Menu, Category, Item, Label, Command, Staff, TimeTable, Language, Theme)
  - Admin/* (Tenants)
  - Shared/* (components and partials)
- Services/
  - Qr, WhatsApp, Theme

## Development Tips

- Use __Build > Clean Solution__ and __Build > Rebuild Solution__ if you change EF models and migrations
- Migrations: add with dotnet ef migrations add <Name> then dotnet ef database update
- Frontend CDN libraries are version-pinned in views; adjust if you upgrade Bootstrap/DataTables/SweetAlert2/Select2

## Roadmap

- Rich analytics per menu/category/item
- Public API for headless consumption
- More themes and a theme marketplace model
- Advanced access control for tenant staff
- Multi-channel messaging providers (email/SMS/push)
- Media storage adapters (Azure Blob, S3)

## Contributing

Contributions are welcome!
- Open an issue for bugs and proposals
- Fork, create a feature branch, and submit a PR
- Please include tests or steps to validate your changes

## License

This project is open source. See the LICENSE file in the repository root for details.

---
Made with ASP.NET Core, Razor, and a focus on multi-tenant simplicity.
