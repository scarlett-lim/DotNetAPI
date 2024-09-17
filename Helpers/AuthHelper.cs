using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration config)
        {
            _config = config;
        }

        // Hashing will always return the same value
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            #region Concat the generated passwordSalt with the PasswordKey to make the password stored more secure
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);
            #endregion

            #region Hash the password
            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            );
            #endregion

            return passwordHash;
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]{
                    new Claim("userId",userId.ToString())
                };

            string? tokenKeyString = _config.GetSection("Appsettings:TokenKey").Value;
            //Token key must at least 512 bits (64 bytes) = 64 characters if wanna use sha512
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials(
                    tokenKey,
                    SecurityAlgorithms.HmacSha512Signature
                );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);


            return tokenHandler.WriteToken(token);
        }
    }
}