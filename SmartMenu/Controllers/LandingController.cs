using Microsoft.AspNetCore.Mvc;
using SmartMenu.Services.PublicMenu;
using SmartMenu.Services.Tenant;

namespace SmartMenu.Controllers
{
    public class LandingController : Controller
    {
        private readonly ITenantService _tenantService;

        public LandingController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
    }
}
