using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.UpdateWallpage.Service.Services
{
    public static class MD5Encryptor
    {
        public static string GetMD5(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                return GetMD5(br.ReadBytes((int)stream.Length));
            }
        }

        public static string GetMD5(byte[] bytes)
        {
            return GetMD5(Convert.ToBase64String(bytes));
        }

        public static string GetMD5(string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
