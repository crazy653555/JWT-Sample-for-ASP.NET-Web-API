using System.Web.Helpers;

namespace WebAPI.Util
{
    public class AccountSecurityService
    {
        public string GenerateSalt(int byteLength = 16)
        {
            string salt = Crypto.GenerateSalt(byteLength);

            return salt;
        }

        public string CryptographyPassword(string password, string salt)
        {
            string cryptographyPassword =
            Crypto.SHA1(password + salt);

            return cryptographyPassword;
        }
    }
}