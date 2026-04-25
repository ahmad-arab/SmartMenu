using SmartMenu.Data.Entities;
using SmartMenu.Models.Item;
using SmartMenu.Models.Language;
using SmartMenu.Repositories.Item;
using SmartMenu.Repositories.Language;
using SmartMenu.Services.FileUpload;

namespace SmartMenu.Services.Item
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IFileUploadService _fileUploadService;

        public ItemService(
            IItemRepository itemRepository,
            ILanguageRepository languageRepository,
            IFileUploadService fileUploadService)
        {
            _itemRepository = itemRepository;
            _languageRepository = languageRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<CreateItemViewModel> GetCreateItemModelAsync(int tenantId, int categoryId)
        {
            var languages = await GetLanguagesForTenantAsync(tenantId);
            return new CreateItemViewModel
            {
                CategoryId = categoryId,
                AvailableLanguages = languages.ToList(),
                TitlesAndDescriptions = languages.Select(l => new ItemTitleAndDescriptionViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };
        }

        public async Task CreateItemAsync(CreateItemViewModel model)
        {
            var imageUrl = await _fileUploadService.UploadImageAsync(model.Image, "item-images");

            var item = new Data.Entities.Item
            {
                ImageUrl = imageUrl,
                Price = model.Price,
                Order = model.Order,
                IsAvailable = model.IsAvailable,
                CategoryId = model.CategoryId
            };
            await _itemRepository.AddAsync(item);

            foreach (var td in model.TitlesAndDescriptions)
            {
                if (!string.IsNullOrWhiteSpace(td.Text))
                    await _itemRepository.AddTitleAsync(new ItemTitle
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });

                if (!string.IsNullOrWhiteSpace(td.Description))
                    await _itemRepository.AddDescriptionAsync(new ItemDescription
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
            }
        }

        public async Task<EditItemViewModel?> GetEditItemModelAsync(int tenantId, int id)
        {
            var item = await _itemRepository.GetByIdWithTitlesAndDescriptionsAsync(id);
            if (item == null)
                return null;

            var languages = await GetLanguagesForTenantAsync(tenantId);

            var titlesAndDescriptions = languages.Select(lang => new ItemTitleAndDescriptionViewModel
            {
                LanguageId = lang.Id,
                Text = item.ItemTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "",
                Description = item.ItemDescriptions.FirstOrDefault(d => d.LanguageId == lang.Id)?.Text ?? ""
            }).ToList();

            return new EditItemViewModel
            {
                ItemId = item.Id,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                Order = item.Order,
                IsAvailable = item.IsAvailable,
                AvailableLanguages = languages.ToList(),
                TitlesAndDescriptions = titlesAndDescriptions
            };
        }

        public async Task<bool> EditItemAsync(EditItemViewModel model)
        {
            var item = await _itemRepository.GetByIdWithTitlesAndDescriptionsAsync(model.ItemId);
            if (item == null)
                return false;

            if (model.Image?.Length > 0)
                item.ImageUrl = await _fileUploadService.UploadImageAsync(model.Image, "item-images");

            item.Price = model.Price;
            item.Order = model.Order;
            item.IsAvailable = model.IsAvailable;

            foreach (var td in model.TitlesAndDescriptions)
            {
                var title = item.ItemTitles.FirstOrDefault(t => t.LanguageId == td.LanguageId);
                if (title != null)
                    title.Text = td.Text;
                else if (!string.IsNullOrWhiteSpace(td.Text))
                    await _itemRepository.AddTitleAsync(new ItemTitle
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });

                var desc = item.ItemDescriptions.FirstOrDefault(d => d.LanguageId == td.LanguageId);
                if (desc != null)
                    desc.Text = td.Description;
                else if (!string.IsNullOrWhiteSpace(td.Description))
                    await _itemRepository.AddDescriptionAsync(new ItemDescription
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
            }

            await _itemRepository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _itemRepository.GetByIdWithTitlesAndDescriptionsAsync(id);
            if (item == null)
                return false;

            await _itemRepository.RemoveTitlesRangeAsync(item.ItemTitles);
            await _itemRepository.RemoveDescriptionsRangeAsync(item.ItemDescriptions);
            _fileUploadService.DeleteImage(item.ImageUrl);
            await _itemRepository.DeleteAsync(item);
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
