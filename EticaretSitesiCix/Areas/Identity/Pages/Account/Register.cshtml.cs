using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EticaretSitesiCix.Data;
using EticaretSitesiCix.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;


namespace EticaretSitesiCix.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Ad { get; set; }
            [Required]
            public string Soyad { get; set; }
            public string Adres { get; set; }
            public string Sehir { get; set; }
            public string Semt { get; set; }
            public string PostaKodu { get; set; }
            public string TelefonNo { get; set; }

            public string Rol { get; set; }
            public IEnumerable<SelectListItem> RolListesi { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel()
            {
                RolListesi = _roleManager.Roles.Where(i => i.Name != Diger.Rol_Birey)
                .Select(x => x.Name)
                .Select(u => new SelectListItem
                {
                    Text = u,
                    Value = u
                })
            };
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var kullanici = new Kullanici
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Adres = Input.Adres,
                    Sehir = Input.Sehir,
                    Semt = Input.Semt,
                    Ad = Input.Ad,
                    Soyad = Input.Soyad,
                    PhoneNumber = Input.TelefonNo,
                    PostaKodu = Input.PostaKodu,
                    Rol = Input.Rol

                };
                var result = await _userManager.CreateAsync(kullanici, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if(!await _roleManager.RoleExistsAsync(Diger.Rol_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Rol_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Rol_Birey))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Rol_Birey));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Rol_Kullanici))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Rol_Kullanici));
                    }
                   if(kullanici.Rol==null)
                    {
                        await _userManager.AddToRoleAsync(kullanici, Diger.Rol_Kullanici);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(kullanici, kullanici.Rol);
                    }

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(kullanici);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = kullanici.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if(kullanici.Rol==null)
                        {
                            await _signInManager.SignInAsync(kullanici, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index","User",new { Area="Admin"});
                        }
                        
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
