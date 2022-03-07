using DIONYSOS.API.Context;
using DIONYSOS.API.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DIONYSOS.API.Authentification
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {

        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public JwtAuthenticationService(DionysosContext context)
        {
            _context = context;
        }

        public APIUser Authenticate(string email)
        {
            return _context.APIUser.Where(u => u.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }

        public string GenerateToken(string secret, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature)

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
