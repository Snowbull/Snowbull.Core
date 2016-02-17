using System;
using System.Security.Cryptography;
namespace Snowbull.API.Cryptography {
    public static class Random {
        public static string GenerateRandomKey(int length) {
            char[] characters = { 'a', 'b' ,'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
                's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J',
                'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0',
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '$', '(', ')', '+', ',', '-', '.', ':',
                '_' };
            byte[] generated = new byte[length];
            string random = "";
            using(RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider()) generator.GetBytes(generated);
            foreach(byte rand in generated) random += characters[rand % characters.Length];
            return random;
        }
    }
}

