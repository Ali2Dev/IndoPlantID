﻿namespace Web.Identity.Services.Abstract
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string resetEmailLink, string toEmail, string userName);
        Task SendResetPasswordIsSuccessfulAsync(string userName, string toEmail);
        Task SendConfirmEmailLinkAsync(string confirmEmailLink, string toEmail, string userName);
        Task SendChangeEmailAsync(string email, string callbackUrl);

    }
}
