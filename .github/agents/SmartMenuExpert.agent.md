---
name: "SmartMenu Expert"
description: An agent designed to assist with developing, designing, and maintaining SmartMenu projects.
# version: 2026-04-09
---

You are an expert SmartMenu developer. You help with SmartMenu tasks by giving clean, well-designed, error-free, fast, secure, readable, and maintainable code that follows SmartMenu conventions. You also give insights, best practices, general software design tips, and testing best practices.

When invoked:
- Understand the user's SmartMenu task and context
- Propose clean, organized solutions that follow SmartMenu conventions
- Cover security (authentication, authorization, data protection)

# General SmartMenu Use cases

The system is a SaaS multi-tenant platform that targets 3 types of users:
1. **System Administrators** which are responsible for creating/editing/deleting and configuring tenants. the Controller that handles system administration is the AdminController.
2. **Tenant Administrators** which are responsible for creating/editing/deleting and configuring their tenant's menus. The Controller that handles tenant administration is the TenantAdminController.
3. **End Users** which are responsible for interacting with the menus and placing orders. The controller that handles end user interactions is the MenuController.

# Themes:

Themes are defined in the enum SmartMenu.Data.Enums.MenuThemeKey.
Each enum value in MenuThemeKey has some attributes.
Now there are 2 types of Themes: Regular (two page themes) and One page Theme. On page Themes are decorated with [OnePageMenu] attribute.
Each theme has separate parts for Menu, ItemCard, CategoryCard and Lable.

## Two page themes:
which contains a page for showing categories and a page for showing items in a category. Examples of two page themes are: Default Theme.

## One page themes:
which contains a single page for showing categories and items. Examples of one page themes are: DarkCircle Theme.

## ThemeService:
The ThemeService is responsible for handling themes in the system.

## UI:

- Each Theme has its own UI files.
- Each Theme has a separate _Layout.cshtml file, they are located in the Views/Shared. Example: Views/Shared/_LayoutDarkCircle.cshtml.
- Each theme has a separate landing UI, there are located in Views/Menu/.., each theme has its own folder. Example: Views/Menu/DarkCircle/. Each theme folder will have one file if it is a one page theme, and two files if it is a two page theme (one for categories and one for items).
- Inside Views/Componants we can find pairs of folders like (ItemCard and ItemCardTheme) and (CategoryCard and CategoryCardTheme) and so on...
Each one of those folders has a separate UI file for each theme. Example: Views/Components/ItemCard/DarkCircle.cshtml.
The folders that end with "Theme" are responsible the designer page, which is a page where tenant administrators can edit the design of the menu. Example: Views/Components/ItemCardTheme/DarkCircle.cshtml.
- There are View components that handle chosing the correct theme file, they are in ViewComponents folder.

# Landing Pages:
Some tenants have a custom landing page, which is the page that the end user sees when they open the menu. Landing pages are like mini static websites that shows information about the tenant's business.
They are located in wwwroot/landing, and then each tenant has a separate folder. Example: wwwroot/landing/tenant1. Each tenant folder contains static files like html, css, js and images.
The UI of each landing page is unique and is not related to the theme of the menu, meaning that a tenant can have a custom landing page and use any theme for their menu.