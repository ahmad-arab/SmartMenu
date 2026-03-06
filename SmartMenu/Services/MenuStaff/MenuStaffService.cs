using Microsoft.AspNetCore.Mvc.Rendering;
using SmartMenu.Data.Entities;
using SmartMenu.Models.MenuStaff;
using SmartMenu.Models.TimeTable;
using SmartMenu.Repositories.Language;
using SmartMenu.Repositories.MenuStaff;
using SmartMenu.Services.Whatsapp;

namespace SmartMenu.Services.MenuStaff
{
    public class MenuStaffService : IMenuStaffService
    {
        private readonly IMenuStaffRepository _menuStaffRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IWhatsappService _whatsappService;

        public MenuStaffService(
            IMenuStaffRepository menuStaffRepository,
            ILanguageRepository languageRepository,
            IWhatsappService whatsappService)
        {
            _menuStaffRepository = menuStaffRepository;
            _languageRepository = languageRepository;
            _whatsappService = whatsappService;
        }

        public async Task<IEnumerable<MenuStaffListItemViewModel>> GetMenuStaffsAsync(int menuId)
        {
            var staffList = await _menuStaffRepository.GetByMenuIdAsync(menuId);
            return staffList.Select(s => new MenuStaffListItemViewModel
            {
                Id = s.Id,
                Name = s.Name,
                PhoneNumber = s.PhoneNumber
            });
        }

        public async Task<CreateMenuStaffViewModel> GetCreateMenuStaffModelAsync(int menuId)
        {
            return new CreateMenuStaffViewModel { MenuId = menuId };
        }

        public async Task CreateMenuStaffAsync(CreateMenuStaffViewModel model)
        {
            var menuStaff = new Data.Entities.MenuStaff
            {
                MenuId = model.MenuId,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };
            await _menuStaffRepository.AddAsync(menuStaff);
        }

        public async Task<EditMenuStaffViewModel?> GetEditMenuStaffModelAsync(int id)
        {
            var menuStaff = await _menuStaffRepository.GetByIdAsync(id);
            if (menuStaff == null)
                return null;

            return new EditMenuStaffViewModel
            {
                MenuStaffId = menuStaff.Id,
                Name = menuStaff.Name,
                PhoneNumber = menuStaff.PhoneNumber,
                IsAvailable = menuStaff.IsAvailable
            };
        }

        public async Task<bool> EditMenuStaffAsync(int id, EditMenuStaffViewModel model)
        {
            var menuStaff = await _menuStaffRepository.GetByIdAsync(id);
            if (menuStaff == null)
                return false;

            menuStaff.Name = model.Name;
            menuStaff.PhoneNumber = model.PhoneNumber;
            menuStaff.IsAvailable = model.IsAvailable;

            await _menuStaffRepository.UpdateAsync(menuStaff);
            return true;
        }

        public async Task<bool> DeleteMenuStaffAsync(int id)
        {
            var menuStaff = await _menuStaffRepository.GetByIdWithAllIncludesAsync(id);
            if (menuStaff == null)
                return false;

            if (menuStaff.TimeSlots?.Any() == true)
                await _menuStaffRepository.RemoveTimeSlotsRangeAsync(menuStaff.TimeSlots);

            if (menuStaff.MenuCommandStaffs?.Any() == true)
                await _menuStaffRepository.RemoveCommandStaffMappingsRangeAsync(menuStaff.MenuCommandStaffs);

            await _menuStaffRepository.DeleteAsync(menuStaff);
            return true;
        }

        public async Task<(bool Success, string Message)> RegisterMenuStaffAsync(int id, int tenantId)
        {
            var menuStaff = await _menuStaffRepository.GetByIdWithMenuTitlesAsync(id);
            if (menuStaff == null)
                return (false, "Menu staff not found.");

            if (string.IsNullOrWhiteSpace(menuStaff.PhoneNumber))
                return (false, "Recipient phone number is required.");

            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            var defaultLangId = languages.FirstOrDefault(l => l.IsDefault)?.Id;

            var defaultMenuTitle =
                menuStaff.Menu.MenuTitles.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                ?? menuStaff.Menu.MenuTitles.FirstOrDefault()?.Text
                ?? FallbackText.NoTitle;

            try
            {
                await _whatsappService.SendTemplateMessageAsync(menuStaff.PhoneNumber, defaultMenuTitle);
                return (true, "Registration message sent successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send registration message: {ex.Message}");
            }
        }

        public async Task<StaffTimeTableViewModel?> GetStaffTimeTableModelAsync(int id)
        {
            var menuStaff = await _menuStaffRepository.GetByIdWithTimeSlotsAsync(id);
            if (menuStaff == null)
                return null;

            return new StaffTimeTableViewModel
            {
                MenuStaffId = menuStaff.Id,
                TimeSlots = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .OrderBy(d => (int)d)
                    .Select(day => new StaffTimeTableViewModel.DayTimeSlot
                    {
                        Day = day,
                        Ranges = menuStaff.TimeSlots
                            .Where(ts => ts.DayOfWeek == day)
                            .OrderBy(ts => ts.StartTime)
                            .Select(ts => new StaffTimeTableViewModel.TimeRange
                            {
                                Start = ts.StartTime,
                                End = ts.EndTime
                            }).ToList()
                    }).ToList()
            };
        }

        public async Task<bool> EditStaffTimeTableAsync(StaffTimeTableViewModel model)
        {
            var menuStaff = await _menuStaffRepository.GetByIdWithTimeSlotsAsync(model.MenuStaffId);
            if (menuStaff == null)
                return false;

            await _menuStaffRepository.RemoveTimeSlotsRangeAsync(menuStaff.TimeSlots);

            foreach (var daySlot in model.TimeSlots)
            {
                foreach (var range in daySlot.Ranges)
                {
                    if (range.Start < range.End)
                    {
                        await _menuStaffRepository.AddTimeSlotAsync(new MenuStaffTimeSlot
                        {
                            MenuStaffId = menuStaff.Id,
                            DayOfWeek = daySlot.Day,
                            StartTime = range.Start,
                            EndTime = range.End
                        });
                    }
                }
            }

            return true;
        }

        public async Task<BulkStaffTimeTableViewModel> GetBulkEditModelAsync(int menuId)
        {
            var staff = (await _menuStaffRepository.GetByMenuIdAsync(menuId))
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            return new BulkStaffTimeTableViewModel
            {
                MenuId = menuId,
                AvailableStaff = staff,
                TimeSlots = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .OrderBy(d => (int)d)
                    .Select(d => new StaffTimeTableViewModel.DayTimeSlot { Day = d, Ranges = new List<StaffTimeTableViewModel.TimeRange>() })
                    .ToList()
            };
        }

        public async Task<bool> BulkEditStaffTimeTableAsync(BulkStaffTimeTableViewModel model)
        {
            if (model.StaffIds == null || !model.StaffIds.Any())
                return false;

            var staffList = await _menuStaffRepository.GetByMenuIdWithTimeSlotsAsync(model.MenuId, model.StaffIds);
            if (!staffList.Any())
                return false;

            foreach (var staff in staffList)
            {
                await _menuStaffRepository.RemoveTimeSlotsRangeAsync(staff.TimeSlots);

                foreach (var daySlot in model.TimeSlots)
                {
                    if (daySlot?.Ranges == null) continue;
                    foreach (var range in daySlot.Ranges)
                    {
                        if (range.Start < range.End)
                        {
                            await _menuStaffRepository.AddTimeSlotAsync(new MenuStaffTimeSlot
                            {
                                MenuStaffId = staff.Id,
                                DayOfWeek = daySlot.Day,
                                StartTime = range.Start,
                                EndTime = range.End
                            });
                        }
                    }
                }
            }

            return true;
        }
    }
}
