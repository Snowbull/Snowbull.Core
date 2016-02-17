using System;
using System.Security.Cryptography;

namespace Snowbull.API.Cryptography {
	public static class Hashing {
		/// <summary>
		/// The hashing salt.
		/// </summary>
		private static string salt = "Y(02.>'H}t\":E1";

		/// <summary>
		/// Hashes a password in MD5 and swaps the two halves.
		/// </summary>
		/// <returns>The hashed and swapped password.</returns>
		/// <param name="password">The password to hash.</param>
		private static string SwapMD5(string password) {
			string hash = Hash<MD5CryptoServiceProvider>(password);
			return hash.Substring(16, 16) + hash.Substring(0, 16);
		}

		/// <summary>
		/// Hashes password with the random key provided by the server.
		/// </summary>
		/// <returns>The hashed password.</returns>
		/// <param name="password">The plaintext password.</param>
		/// <param name="rndk">The random key.</param>
		public static string HashPassword(string password, string rndk) {
			string key = SwapMD5(password).ToUpper();
			key += rndk;
			key += salt;
			key = SwapMD5(key);
			return key;
		}

		/// <summary>
		/// Hashes a string with a given hash algorithm (using generic parameters)
		/// </summary>
		/// <param name="text">The string to hash.</param>
		public static string Hash<AlgorithmType>(string text) where AlgorithmType : HashAlgorithm, new() {
			string hash = "";
			AlgorithmType algorithm = new AlgorithmType();
			byte[] hashedBytes = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));
			foreach(byte hashedByte in hashedBytes) hash += hashedByte.ToString("x2");
			return hash;
		}
	}
}

