// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Quickstart.UI.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserInteractionService _interaction;

        public HomeController(IUserInteractionService interaction)
        {
            _interaction = interaction;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}