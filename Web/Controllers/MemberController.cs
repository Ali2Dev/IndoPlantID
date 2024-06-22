using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NuGet.Common;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Web.Enums;
using Web.Extensions;
using Web.Identity;
using Web.Identity.Services.Abstract;
using Web.Identity.ViewModels;
using static OpenAI.ObjectModels.SharedModels.IOpenAiModels;

namespace Web.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IFileProvider _fileProvider;

        private readonly ITwoFactorService _twoFactorService;

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IFileProvider fileProvider, ITwoFactorService twoFactorService) : base(userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _fileProvider = fileProvider;
            _twoFactorService = twoFactorService;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);


            var userViewModel = new UserViewModel
            {
                FirstName = currentUser!.FirstName,
                LastName = currentUser!.LastName,
                UserName = currentUser!.UserName,
                Email = currentUser!.Email,
                PhoneNumber = currentUser!.PhoneNumber,
                EmailConfirmed = currentUser!.EmailConfirmed,
                TwoFactorEnabled = currentUser!.TwoFactorEnabled
            };
            ViewData["PictureUrl"] = currentUser.Picture;

            return View(userViewModel);
        }

        public async Task<IActionResult> ChangeEmail()
        {
            TempData["ShowWarningMsg"] = true;
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);


            var changeEmailViewModel = new ChangeEmailViewModel()
            {
                FirstName = currentUser!.FirstName,
                LastName = currentUser!.LastName,
                UserName = currentUser!.UserName,
                OldEmail = currentUser!.Email,
            };
            await GetUserPicture();
            return View(changeEmailViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel request)
        {

            //ViewBag.ShowNavbar = false;
            TempData["ShowWelcomeMessage"] = false;
            TempData["ShowWarningMsg"] = false;



            //bool userFound = false;

            var hasUser = await _userManager.FindByEmailAsync(request.OldEmail);


            if (hasUser == null)
            {
                //TempData["UserFound"] = userFound;
                ModelState.AddModelError(string.Empty, "Geçerli e-posta adresi bulunamadı.");
                return View(request);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(hasUser, request.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Şifreniz yanlış.");
                return View(request);
            }

            //Check for New Email
            var IsExist = await _userManager.FindByEmailAsync(request.NewEmail);

            if (IsExist == null)
            {
                string emailResetToken = await _userManager.GenerateChangeEmailTokenAsync(hasUser, request.NewEmailConfirmed);
                var callbackUrl = Url.Action("ConfirmEmailChanging", "Member", new { userId = hasUser.Id, newEmail = request.NewEmail, token = emailResetToken }, protocol: Request.Scheme);

                await _emailService.SendChangeEmailAsync(request.NewEmailConfirmed, callbackUrl);

                ViewBag.Message = "Yeni e-posta adresinize bir onaylama bağlantısı gönderildi.";
                return View(request);
            }
            TempData["AlreadyExist"] = "Bu e-mail ile kayıtlı mevcut bir kullanıcı var. Yeni bir e-mail girin.";
            return View(request);


        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailChanging(string userId, string newEmail, string token)
        {
            if (userId == null || newEmail == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }


            await _userManager.UpdateSecurityStampAsync(user);

            TempData["ChangedEmail"] = "E-posta adresiniz başarıyla değiştirildi.";
            return RedirectToAction("Index", "Member");
        }

        public async Task<IActionResult> ChangePassword()
        {
            await GetUserPicture();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel request)
        {


            if (!ModelState.IsValid)
            {

                return View();
            }

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");

                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.OldPassword, request.NewPassword);

            if (!resultChangePassword.Succeeded)
            {

                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, request.NewPassword, true, false);

            TempData["SuccessMessage"] = true;
            await GetUserPicture();

            await _emailService.SendResetPasswordIsSuccessfulAsync(currentUser.UserName!, currentUser.Email!);

            return View();
        }

        public async Task<IActionResult> EditUser()
        {
            var currentUser = await _userManager.FindByNameAsync(User?.Identity?.Name!);

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View();
            }

            var editUserViewModel = new EditUserViewModel()
            {

                UserName = currentUser!.UserName!,
                Email = currentUser!.Email!,
                Phone = currentUser!.PhoneNumber!,
                FirstName = currentUser!.FirstName,
                LastName = currentUser!.LastName,
                IsEmailConfirmed = currentUser.EmailConfirmed,
                TwoFactorEnabled = currentUser.TwoFactorEnabled
            };

            ViewData["PictureUrl"] = currentUser.Picture;

            return View(editUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View(request);
            }
            ViewData["PictureUrl"] = currentUser.Picture;
            request.IsEmailConfirmed = currentUser.EmailConfirmed;

            if (!ModelState.IsValid)
            {
                return View(request);
            }


            if (request.UserName != currentUser.UserName)
            {
                var isExist = await _userManager.FindByNameAsync(request.UserName);

                if (isExist != null)
                {
                    ModelState.AddModelError(string.Empty, "Bu kullanıcı adı başkası tarafından kullanılıyor.");
                    return View(request);
                }

            }




            currentUser!.UserName = request.UserName;
            //currentUser!.Email = request.Email;
            currentUser!.PhoneNumber = request.Phone;
            currentUser.FirstName = request.FirstName;
            currentUser.LastName = request.LastName;


            if (request.File != null && request.File.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot//uploads");

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.File.FileName)}";
                //jpg,png vs
                var newPicturePath =
                    Path.Combine(wwwrootFolder.First(x => x.Name == "userPictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.File.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updatedUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updatedUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updatedUserResult.Errors);

                ViewData["PictureUrl"] = currentUser.Picture;

                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);

            await _signInManager.SignOutAsync();

            await _signInManager.SignInAsync(currentUser, true);

            TempData["SuccessMessage"] = "Kullanıcı bilgileri güncellendi";

            return View(request);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserPicture(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            user.Picture = null;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("EditUser", "Member");
        }


        public async Task<IActionResult> TwoFactorAuth()
        {



            var authenticatorViewModel = new AuthenticatorViewModel
            {
                TwoFactorType = (TwoFactor)CurrentUser.TwoFactorType
            };

            await GetUserPicture();
            return View(authenticatorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuth(AuthenticatorViewModel authenticatorVM)
        {
            switch (authenticatorVM.TwoFactorType)
            {
                case TwoFactor.None:
                    CurrentUser.TwoFactorEnabled = false;
                    CurrentUser.TwoFactorType = (sbyte)TwoFactor.None;

                    TempData["TwoFactorMsg"] = "2-Adımlı doğrulama devre dışı bırakıldı.";
                    TempData["TwoFactorDisabled"] = true;
                    break;

                case TwoFactor.MicrosoftGoogle:
                    return RedirectToAction("TwoFactorByAuthenticator");

                case TwoFactor.SMS:
                    CurrentUser.TwoFactorEnabled = true;
                    CurrentUser.TwoFactorType = (sbyte)TwoFactor.SMS;

                    TempData["TwoFactorMsg"] = "2-Adımlı doğrulama aktif edildi. Güvenlik tipi: SMS";

                    break;
            }

            await _userManager.UpdateAsync(CurrentUser);
            await GetUserPicture();
            return View(authenticatorVM);
        }

        public async Task<IActionResult> TwoFactorByAuthenticator()
        {
            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(CurrentUser);

            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(CurrentUser);

                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(CurrentUser);

            }

            var authenticatorVM = new AuthenticatorViewModel
            {
                SharedKey = unformattedKey,
                AuthenticatorUri = _twoFactorService.GenerateQrCodeUri(CurrentUser.Email, unformattedKey)
            };
            await GetUserPicture();
            return View(authenticatorVM);

        }


        [HttpPost]
        public async Task<IActionResult> TwoFactorByAuthenticator(AuthenticatorViewModel request)
        {
            var verificationCode = CleanVerificationCode(request.VerificationCode);

            var is2FATokenValid = await _userManager.VerifyTwoFactorTokenAsync(CurrentUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);


            if (is2FATokenValid)
            {
                CurrentUser.TwoFactorEnabled = true;
                CurrentUser.TwoFactorType = (sbyte)TwoFactor.MicrosoftGoogle;

                TempData["TwoFactorMsg"] = "2-Adımlı doğrulama aktif edildi. Güvenlik tipi: Microsoft/Google";
                TempData["TwoFactorDisabled"] = false;


                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(CurrentUser, 5);
                TempData["RecoveryCodes"] = recoveryCodes;

                return RedirectToAction("TwoFactorAuth");
            }
            ModelState.AddModelError("", "Girilen doğrulama kodu geçersiz.");
            return View(request);
        }


        public static string CleanVerificationCode(string verificationCode)
        {
            // Boşlukları ve özel karakterleri kaldır
            string cleanedCode = Regex.Replace(verificationCode, @"[\s\-\+]", "");
            return cleanedCode;
        }

    }
}
