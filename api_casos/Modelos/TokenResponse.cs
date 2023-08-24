using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_casos.Modelos
{
    public class TokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
