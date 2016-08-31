// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4.Quickstart.UI.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly InMemoryUserLoginService _loginService;
        private readonly IUserInteractionService _interaction;

        public AccountController(
            InMemoryUserLoginService loginService,
            IUserInteractionService interaction)
        {
            _loginService = loginService;
            _interaction = interaction;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = new LoginViewModel(HttpContext);

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                vm.Username = context.LoginHint;
                vm.ReturnUrl = returnUrl;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                if (_loginService.ValidateCredentials(model.Username, model.Password))
                {
                    var user = _loginService.FindByUsername(model.Username);
                    //await IssueCookie(user, "idsvr", "password");
                    await HttpContext.Authentication.SignInAsync(user.Subject, user.Username);
                    
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            var vm = new LoginViewModel(HttpContext, model);
            return View(vm);
        }

        [HttpGet]
        public IActionResult External(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }
            returnUrl = "/account/externalcallback?returnUrl=" + returnUrl;

            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> ExternalCallback(string returnUrl)
        {
            var tempUser = await HttpContext.Authentication.AuthenticateAsync("Temp");
            if (tempUser == null)
            {
                throw new Exception();
            }

            var claims = tempUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            claims.Remove(userIdClaim);

            var provider = userIdClaim.Issuer;
            var userId = userIdClaim.Value;

            var user = _loginService.FindByExternalProvider(provider, userId);
            if (user == null)
            {
                user = _loginService.AutoProvisionUser(provider, userId, claims);
            }

            var sid = claims.FirstOrDefault(x => x.Type == OidcConstants.EndSessionRequest.Sid)?.Value;
            await IssueCookie(user, provider, "external", sid);
            await HttpContext.Authentication.SignOutAsync("Temp");

            if (_interaction.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");

        }

        [HttpGet]
        public IActionResult Logout(string logoutId)
        {
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            return View("LoggedOut", vm);
        }

        private async Task IssueCookie(
            InMemoryUser user,
            string idp,
            string amr,
            string sid = null)
        {
            var name = user.Claims.Where(x => x.Type == JwtClaimTypes.Name).Select(x => x.Value).FirstOrDefault() ?? user.Username;

            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Subject, user.Subject),
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.IdentityProvider, idp),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
                new Claim("role", "some_role")
            };
            if (sid != null)
            {
                claims.Add(new Claim(OidcConstants.EndSessionRequest.Sid, sid));
            }

            var ci = new ClaimsIdentity(claims, amr, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, cp);
        }
    }
}
