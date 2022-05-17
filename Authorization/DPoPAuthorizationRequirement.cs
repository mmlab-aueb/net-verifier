using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
//using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;

namespace net_verifier.Authorization
{
    public class DPoPAuthorizationRequirement : IAuthorizationRequirement, IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.Resource is HttpContext httpContext)
            {

                StringValues dpop_header; ;
                httpContext.Request.Headers.TryGetValue("dpop", out dpop_header);
                if (dpop_header.Count > 0)
                {
                    Debug.WriteLine("DPoP: " + dpop_header);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var dpop = tokenHandler.ReadJwtToken(dpop_header);
                    string clietKeyJSON = dpop.Header["jwk"].ToString();
                    try
                    {
                        var clietJWK = new JsonWebKey(clietKeyJSON);
                        var validationParameters = new TokenValidationParameters()
                        {
                            IssuerSigningKey = clietJWK,
                            ValidateLifetime = false,
                            ValidateAudience = false,
                            ValidateIssuer = false
                        };
                        tokenHandler.ValidateToken(dpop_header, validationParameters, out SecurityToken validatedToken);
                        context.Succeed(this);
                        Debug.WriteLine("SecurityToken: " + validatedToken.ToString());
                    }
                    catch(Exception e) {
                        Debug.WriteLine("Exception: " + e.ToString());
                    }
                }  
             }
            Debug.WriteLine("Taks completed");
            return Task.CompletedTask;
        }

    }
}
