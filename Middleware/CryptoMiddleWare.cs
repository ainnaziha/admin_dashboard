using System.Security.Cryptography;
using System.Text;

namespace spl.Middleware
{
    public class CryptoMiddleWare
    {
        public static string ComputeSHA256Hash(string str)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            byte[] inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }
    }
}
