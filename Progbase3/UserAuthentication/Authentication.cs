using System;
using System.Text;
using System.Security.Cryptography;

namespace ConsoleApp
{
    public class Authentication
    {
        private UserRepository userRepo;
        public Authentication(UserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        public long RegisterUser(User user)
        {
            if (this.userRepo.GetByUsername(user.username) != null)
            {
                return -1;
            }
            
            SHA256 sha256Hash = SHA256.Create();
            user.password = GetHash(sha256Hash, user.password);

            sha256Hash.Dispose();

            long userId = this.userRepo.Insert(user);
            return userId;
        }

        public User LoginUser(string username, string password)
        {
            User user = this.userRepo.GetByUsername(username);
            SHA256 sha256Hash = SHA256.Create();
            string userPassword = GetHash(sha256Hash, password);
            
            if (user == null || user.password != userPassword)
            {
                return null;
            }

            return user;
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
        
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();
        
            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
        
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
 
        // Verify a hash against a string.
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);
 
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
 
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
