using SmartMenu.Data.Entities;
using SmartMenu.Models.Category;
using SmartMenu.Models.Item;
using SmartMenu.Models.Language;
using SmartMenu.Repositories.Category;
using SmartMenu.Repositories.Item;
using SmartMenu.Repositories.Language;
using SmartMenu.Services.FileUpload;

namespace SmartMenu.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IFileUploadService _fileUploadService;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IItemRepository itemRepository,
            ILanguageRepository languageRepository,
            IFileUploadService fileUploadService)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
            _languageRepository = languageRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<CreateCategoryViewModel> GetCreateCategoryModelAsync(int tenantId, int menuId)
        {
            var languages = await GetLanguagesForTenantAsync(tenantId);
            return new CreateCategoryViewModel
            {
                MenuId = menuId,
                AvailableLanguages = languages.ToList(),
                TitlesAndDescriptions = languages.Select(l => new CategoryTitleAndDescriptionViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };
        }

        public async Task CreateCategoryAsync(CreateCategoryViewModel model)
        {
            var imageUrl = await _fileUploadService.UploadImageAsync(model.Image, "category-images");

            var category = new Data.Entities.Category
            {
                ImageUrl = imageUrl,
                MenuId = model.MenuId
            };
            await _categoryRepository.AddAsync(category);

            foreach (var td in model.TitlesAndDescriptions)
            {
                if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    await _categoryRepository.AddTitleAsync(new CategoryTitle
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }
                if (!string.IsNullOrWhiteSpace(td.Description))
                {
                    await _categoryRepository.AddDescriptionAsync(new CategoryDescription
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
                }
            }
        }

        public async Task<EditCategoryViewModel?> GetEditCategoryModelAsync(int tenantId, int id)
        {
            var category = await _categoryRepository.GetByIdWithTitlesAndDescriptionsAsync(id);
            if (category == null)
                return null;

            var languages = await GetLanguagesForTenantAsync(tenantId);

            var titlesAndDescriptions = languages.Select(lang => new CategoryTitleAndDescriptionViewModel
            {
                LanguageId = lang.Id,
                Text = category.CategoryTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "",
                Description = category.CategoryDescriptions.FirstOrDefault(d => d.LanguageId == lang.Id)?.Text ?? ""
            }).ToList();

            return new EditCategoryViewModel
            {
                CategoryId = category.Id,
                ImageUrl = category.ImageUrl,
                AvailableLanguages = languages.ToList(),
                TitlesAndDescriptions = titlesAndDescriptions
            };
        }

        public async Task<bool> EditCategoryAsync(EditCategoryViewModel model)
        {
            var category = await _categoryRepository.GetByIdWithTitlesAndDescriptionsAsync(model.CategoryId);
            if (category == null)
                return false;

            if (model.Image?.Length > 0)
                category.ImageUrl = await _fileUploadService.UploadImageAsync(model.Image, "category-images");

            foreach (var td in model.TitlesAndDescriptions)
            {
                var title = category.CategoryTitles.FirstOrDefault(t => t.LanguageId == td.LanguageId);
                if (title != null)
                    title.Text = td.Text;
                else if (!string.IsNullOrWhiteSpace(td.Text))
                    await _categoryRepository.AddTitleAsync(new CategoryTitle
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });

                var desc = category.CategoryDescriptions.FirstOrDefault(d => d.LanguageId == td.LanguageId);
                if (desc != null)
                    desc.Text = td.Description;
                else if (!string.IsNullOrWhiteSpace(td.Description))
                    await _categoryRepository.AddDescriptionAsync(new CategoryDescription
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
            }

            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        public async Task<CategoryViewModel?> GetCategoryPageModelAsync(int tenantId, int id)
        {
            var category = await _categoryRepository.GetByIdWithTitlesAndDescriptionsAsync(id);
            if (category == null)
                return null;

            var languages = (await _languageRepository.GetByTenantIdAsync(tenantId)).ToList();
            var items = await _itemRepository.GetByCategoryIdWithTitlesAsync(id);

            var defaultLangId = languages.FirstOrDefault(l => l.IsDefault)?.Id;

            var itemViewModels = items.Select(item => new ItemListItemViewModel
            {
                Id = item.Id,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                DefaultTitle =
                    item.ItemTitles.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                    ?? item.ItemTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoTitle,
                TitlesByLanguage = languages.ToDictionary(
                    lang => lang.Name,
                    lang => item.ItemTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoTitle)
            }).ToList();

            return new CategoryViewModel
            {
                Id = category.Id,
                MenuId = category.MenuId,
                DefaultTitle =
                    category.CategoryTitles.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                    ?? category.CategoryTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoTitle,
                Title = string.Join(" / ", languages.ToDictionary(
                    lang => lang.Name,
                    lang => category.CategoryTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoTitle
                ).Select(p => $"{p.Key}: {p.Value}")),
                ImageUrl = category.ImageUrl,
                Items = itemViewModels
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithTitlesAndDescriptionsAsync(id);
            if (category == null)
                return false;

            var items = await _itemRepository.GetByCategoryIdWithTitlesAsync(id);
            foreach (var item in items)
            {
                var fullItem = await _itemRepository.GetByIdWithTitlesAndDescriptionsAsync(item.Id);
                if (fullItem != null)
                {
                    await _itemRepository.RemoveTitlesRangeAsync(fullItem.ItemTitles);
                    await _itemRepository.RemoveDescriptionsRangeAsync(fullItem.ItemDescriptions);
                    _fileUploadService.DeleteImage(fullItem.ImageUrl);
                    await _itemRepository.DeleteAsync(fullItem);
                }
            }

            await _categoryRepository.RemoveTitlesRangeAsync(category.CategoryTitles);
            await _categoryRepository.RemoveDescriptionsRangeAsync(category.CategoryDescriptions);
            _fileUploadService.DeleteImage(category.ImageUrl);
            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<IEnumerable<LanguageListItemViewModel>> GetLanguagesForTenantAsync(int tenantId)
        {
            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            return languages.Select(l => new LanguageListItemViewModel
            {
                Id = l.Id,
                Name = l.Name,
                IsRtl = l.IsRtl
            });
        }
    }
}
