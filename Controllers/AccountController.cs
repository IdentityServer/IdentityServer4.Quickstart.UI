// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Quickstart.UI.Models;
using IdentityServer4.Services;
using IdentityServer4.Services.InMemory;
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
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    public class AccountController : Controller
    {
        private readonly InMemoryUserLoginService _loginService;
        private readonly IIdentityServerInteractionService _interaction;

        public AccountController(
            InMemoryUserLoginService loginService,
            IIdentityServerInteractionService interaction)
        {
            _loginService = loginService;
            _interaction = interaction;
        }

        /// <summary>
        /// Show login page
        /// </summary>
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

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (_loginService.ValidateCredentials(model.Username, model.Password))
                {
                    // issue authentication cookie with subject ID and username
                    var user = _loginService.FindByUsername(model.Username);
                    await HttpContext.Authentication.SignInAsync(user.Subject, user.Username);
                    
                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // something went wrong, show form with error
            var vm = new LoginViewModel(HttpContext, model);
            return View(vm);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult External(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }
            returnUrl = "/account/externalcallback?returnUrl=" + returnUrl;

            // start challenge and roundtrip the return URL
            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalCallback(string returnUrl)
        {
            // read external identity from the temporary cookie
            var tempUser = await HttpContext.Authentication.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var claims = tempUser.Claims.ToList();

            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            // remove the user id claim from the claims collection and move to the userId property
            // also set the name of the external authentication provider
            claims.Remove(userIdClaim);
            var provider = userIdClaim.Issuer;
            var userId = userIdClaim.Value;

            // check if the external user is already provisioned
            var user = _loginService.FindByExternalProvider(provider, userId);
            if (user == null)
            {
                // this sample simply auto-provisions new external user
                // another common approach is to start a registrations workflow first
                user = _loginService.AutoProvisionUser(provider, userId, claims);
            }

            var additionalClaims = new List<Claim>();

            // if the external system sent a session id claim, copy it over
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // issue authentication cookie for user
            await HttpContext.Authentication.SignInAsync(user.Subject, user.Username, provider, additionalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // validate return URL and redirect back to authorization endpoint
            if (_interaction.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");

        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public IActionResult Logout(string logoutId)
        {
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            return View("LoggedOut", vm);
        }
    }
}