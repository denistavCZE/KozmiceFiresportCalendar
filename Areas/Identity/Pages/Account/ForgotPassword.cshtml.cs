// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FiresportCalendar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;

namespace FiresportCalendar.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Person> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ReCaptchaService _reCaptchaService;
        private readonly IConfiguration _configuration;
        public string ReCaptchaSiteKey { get; set; }
        public ForgotPasswordModel(UserManager<Person> userManager, IEmailSender emailSender, ReCaptchaService reCaptchaService, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _reCaptchaService = reCaptchaService;
            _configuration = configuration;
        }
        public void OnGet()
        {
            ReCaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public string RecaptchaToken { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (!await _reCaptchaService.IsCaptchaValid(RecaptchaToken))
            {
                ModelState.AddModelError("", "ReCAPTCHA selhala. Zkuste to prosím znovu");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Obnova hesla",
                $"Prosím obnovte si své heslo <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>kliknutím zde</a>.");

            return RedirectToPage("./ForgotPasswordConfirmation");
        

        }
    }
}
