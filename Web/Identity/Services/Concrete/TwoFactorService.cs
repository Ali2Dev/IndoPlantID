using System.Text.Encodings.Web;
using Web.Identity.Services.Abstract;

namespace Web.Identity.Services.Concrete
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UrlEncoder _urlEncoder;

        public TwoFactorService(UrlEncoder urlEncoder)
        {
            _urlEncoder = urlEncoder;
        }

        public string GenerateQrCodeUri(string email, string unformattedKey)
        {
            const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return string.Format(format, _urlEncoder.Encode("www.bitkinet.com"), _urlEncoder.Encode(email), unformattedKey);

        }
    }
}
