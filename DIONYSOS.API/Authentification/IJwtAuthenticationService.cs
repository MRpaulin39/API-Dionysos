using DIONYSOS.API.Data.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace DIONYSOS.API.Authentification
{
    public interface IJwtAuthenticationService
    {
        APIUser Authenticate(string email);
        string GenerateToken(string secret, List<Claim> claims);
    }
}
