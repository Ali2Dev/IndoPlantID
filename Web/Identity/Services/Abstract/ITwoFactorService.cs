namespace Web.Identity.Services.Abstract
{
    public interface ITwoFactorService
    {
        string GenerateQrCodeUri(string email, string unformattedKey);
    }
}
