using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Logman.Business
{
    public static class CryptoHelper
    {
        private const short SaltLentgh = 10;

        public static string GenerateSalt()
        {
            var rngService = new RNGCryptoServiceProvider();
            var buffer = new byte[SaltLentgh];
            rngService.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static string Hash(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            var cryptoService = new SHA256Managed();
            byte[] hashedBytes = cryptoService.ComputeHash(bytes);
            return string.Join("", hashedBytes.Select(x => x.ToString("x2")));
        }
    }
}