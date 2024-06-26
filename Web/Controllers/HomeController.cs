﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Enums;
using Web.Extensions;
using Web.Identity.ViewModels;
using Web.Identity;
using Web.Identity.Services.Abstract;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        //Identity
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, ILogger<HomeController> logger) : base(userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }


        public async Task<ActionResult> Index()
        {
            await GetUserPicture();
            //ViewBag.ShowNavbar = false;
            TempData["ShowNavbar"] = false;
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            //ViewBag.ShowNavbar = false;
            TempData["ShowNavbar"] = false;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            TempData["ShowNavbar"] = false;
            TempData["ShowWelcomeMessage"] = false;

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var userResult = await _userManager.FindByEmailAsync(request.Email);

            if (userResult == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            bool userCheck = await _userManager.CheckPasswordAsync(userResult, request.Password);
            var signInResult = await _signInManager.PasswordSignInAsync(userResult, request.Password, request.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                var serverDateTime = DateTime.UtcNow;
                var lockoutEndDateTimeOffset = await _userManager.GetLockoutEndDateAsync(userResult);

                var lockoutEndDateUtc = lockoutEndDateTimeOffset.Value.UtcDateTime;
                var result = Math.Abs(Convert.ToInt16((serverDateTime - lockoutEndDateUtc).TotalSeconds));

                ModelState.AddModelErrorList(new List<string>() { $"{result} saniye boyunca giriş yapamazsınız." });
                return View();
            }

            if (userCheck)
            {
                await _userManager.ResetAccessFailedCountAsync(userResult);

                if (signInResult.RequiresTwoFactor)
                {
                    TempData["ShowNavbar"] = false;
                    TempData["ShowWelcomeMessage"] = true;
                    return RedirectToAction("TwoFactorSignIn", "Home");
                }

                if (signInResult.Succeeded)
                {
                    return Redirect(returnUrl);
                }
            }




            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış!" });

            return View();
        }

        public async Task<IActionResult> TwoFactorSignIn()
        {
            TempData["ShowNavbar"] = false;
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            switch ((TwoFactor)user.TwoFactorType)
            {
                case TwoFactor.MicrosoftGoogle:
                    break;

            }

            var twoFactorSignInVM = new TwoFactorSignInViewModel
            {
                TwoFactorType = (TwoFactor)user.TwoFactorType,
                IsRecoveryCodeRequested = false,
                RememberMe = false,
                VerificationCode = string.Empty
            };
            return View(twoFactorSignInVM);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorSignIn(TwoFactorSignInViewModel request, string? returnUrl = null)
        {
            TempData["ShowNavbar"] = false;
            returnUrl = returnUrl ?? Url.Action("Index", "Home");
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();


            ModelState.Clear();


            if ((TwoFactor)user.TwoFactorType == TwoFactor.MicrosoftGoogle)
            {
                Microsoft.AspNetCore.Identity.SignInResult result;

                if (request.IsRecoveryCodeRequested)
                {
                    result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(request.VerificationCode);

                }
                else
                {
                    result = await _signInManager.TwoFactorAuthenticatorSignInAsync(request.VerificationCode, request.RememberMe, false);

                }

                if (result.Succeeded)
                {
                    return Redirect(returnUrl);
                }

            }

            request.TwoFactorType = (TwoFactor)user.TwoFactorType;
            ModelState.AddModelError(string.Empty, "Doğrulama kodu geçersiz.");
            return View(request);
        }



        public IActionResult SignUp()
        {
            TempData["ShowWelcomeMessage"] = true;
            //ViewBag.ShowNavbar = false;
            TempData["ShowNavbar"] = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            //ViewBag.ShowNavbar = false;
            TempData["ShowNavbar"] = false;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new AppUser
            {
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                TwoFactorType = 0
            }, request.PasswordConfirmed);

            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Kayıt işlemi gerçekleştirildi ve e-mail adresinize doğrulama linki gönderildi.";

                var user = await _userManager.FindByEmailAsync(request.Email);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmEmailLink = Url.Action("ConfirmEmail", "Home", new { user.Id, token }, HttpContext.Request.Scheme);

                //Send an email
                await _emailService.SendConfirmEmailLinkAsync(confirmEmailLink!, user.Email!, user.FullName!);
                return RedirectToAction(nameof(HomeController.SignUp));
            }

            //foreach (var item in identityResult.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, item.Description);
            //    ViewBag.ErrorMessage = item.Description;
            //}

            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            return View();


        }


        public IActionResult ForgotMyPassword()
        {
            //ViewBag.ShowNavbar = false;
            TempData["ShowNavbar"] = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotMyPassword(ForgotMyPasswordViewModel request)
        {
            //ViewBag.ShowNavbar = false;
            TempData["ShowNavbar"] = false;
            TempData["ShowWelcomeMessage"] = false;

            bool userFound = false;

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                TempData["UserFound"] = userFound;
                ModelState.AddModelError(string.Empty, "Böyle bir kullanıcı bulunamadı.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink =
                Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, token = passwordResetToken }, HttpContext.Request.Scheme);

            //Send an email
            await _emailService.SendResetPasswordEmailAsync(passwordResetLink!, hasUser.Email!, hasUser.UserName!);

            userFound = true;
            TempData["UserFound"] = userFound;

            return RedirectToAction(nameof(ForgotMyPassword));

        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            bool isPasswordUpdated = false;

            if (TempData["userId"] == null || TempData["token"] == null)
            {
                // userId veya token null ise hata sayfasına yönlendir.
                return RedirectToAction("Error");
            }


            string userId = TempData["userId"].ToString()!;
            string token = TempData["token"].ToString()!;

            var hasUser = await _userManager.FindByIdAsync(userId);

            if (hasUser == null)
            {
                TempData["isPasswordUpdated"] = isPasswordUpdated;
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token, request.Password);

            if (result.Succeeded)
            {
                isPasswordUpdated = true;
                TempData["isPasswordUpdated"] = isPasswordUpdated;

                //Send an email - PasswordUpdatedInfo
                await _emailService.SendResetPasswordIsSuccessfulAsync(hasUser.UserName!, hasUser.Email!);
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            if (id == null || token == null)
            {
                TempData["ConfirmEmailMsg"] = "Bu token artık geçersiz. Yeni bir tane talep edebilirsin.";
                TempData["MessageType"] = "negative";
                return RedirectToAction(nameof(HomeController.Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    TempData["ConfirmEmailMsg"] = "Mail adresiniz başarıyla onaylandı!";
                    TempData["MessageType"] = "positive";
                    return RedirectToAction(nameof(HomeController.Index));
                }
            }

            TempData["ConfirmEmailMsg"] = "Bu e-mail ile bir kullanıcı bulanamadı!";
            TempData["MessageType"] = "warning";
            return RedirectToAction(nameof(HomeController.Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailConfirmLink(SignUpViewModel request)
        {

            var user = await _userManager.FindByEmailAsync(request.Email);


            bool isEmailSent = await SendEmailConfirmLinkAsync(user);

            if (isEmailSent)
            {
                TempData["ConfirmEmailMsg"] = "Hesap doğrulama bağlantısı e-posta adresinize gönderildi!";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(MemberController.EditUser), "Member");

            }

            TempData["ConfirmEmailMsg"] = "Hesap doğrulama bağlantısı gönderilemedi! Lütfen daha sonra tekrar deneyin.";
            TempData["MessageType"] = "negative";
            return RedirectToAction(nameof(MemberController.EditUser), "Member");
        }

        private async Task<bool> SendEmailConfirmLinkAsync(AppUser user)
        {


            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmEmailLink = Url.Action("ConfirmEmail", "Home", new { user.Id, token }, HttpContext.Request.Scheme);

                //Send an email
                await _emailService.SendConfirmEmailLinkAsync(confirmEmailLink!, user.Email!, user.FullName!);
                return true;
            }
            return false;

        }
    }
}