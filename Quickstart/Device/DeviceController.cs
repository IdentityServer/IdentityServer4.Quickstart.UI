using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Quickstart.UI
{
    [Authorize]
    [SecurityHeaders]
    public class DeviceController : Controller
    {
        private readonly IDeviceFlowInteractionService _interactionService;

        public DeviceController(IDeviceFlowInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [HttpGet]
        public IActionResult Index([FromQuery(Name = "user_code")] string userCode)
        {
            if (string.IsNullOrWhiteSpace(userCode)) return View("UserCodeCapture");
            return View("UserCodeConfirmation", userCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Callback(string userCode)
        {
            if (string.IsNullOrWhiteSpace(userCode)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(userCode));

            var result = await _interactionService.HandleRequestAsync(userCode);

            if (result.IsError) return View("Error");
            if (result.IsAccessDenied) return View("Error");

            return View("Success");
        }
    }
}