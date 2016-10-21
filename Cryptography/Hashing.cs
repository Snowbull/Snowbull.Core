/**
 * Cryptogtaphic Digest Functions for Snowbull's Plugin API ("Snowbull").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull".
 * 
 * "Snowbull" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */
 
using System.Security.Cryptography;

namespace Snowbull.Core.Cryptography {
	public static class Hashing {
		/// <summary>
		/// The hashing salt.
		/// </summary>
		private const string salt = "Y(02.>'H}t\":E1";

		/// <summary>
		/// Hashes a password in MD5 and swaps the two halves.
		/// </summary>
		/// <returns>The hashed and swapped password.</returns>
		/// <param name="password">The password to hash.</param>
        /// <param name="hash">Whether to hash in md5 or not (if using a password from the database, md5 is already applied.)</param>
        private static string SwapMD5(string password, bool hash) {
			string md5 = hash ? Hash<MD5CryptoServiceProvider>(password) : password;
			return md5.Substring(16, 16) + md5.Substring(0, 16);
		}

		/// <summary>
		/// Hashes password with the random key provided by the server.
		/// </summary>
		/// <returns>The hashed password.</returns>
		/// <param name="password">The plaintext password.</param>
		/// <param name="rndk">The random key.</param>
		public static string HashPassword(string password, string rndk) {
			string key = SwapMD5(password, false).ToUpper();
			key += rndk;
			key += salt;
			key = SwapMD5(key, true);
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

