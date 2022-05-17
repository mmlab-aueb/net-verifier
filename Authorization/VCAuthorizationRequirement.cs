using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;

namespace net_verifier.Authorization
{
    public class VCAuthorizationRequirement : IAuthorizationRequirement, IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.Resource is HttpContext httpContext)
            {

                var url = UriHelper.GetEncodedUrl(httpContext.Request);
                Debug.WriteLine("Requested URL: " + url);
                var vc = context.User.FindFirst("vc");
                if (vc != null)
                    {
                        Debug.WriteLine("Token: " + vc.Value);
                        JsonNode capabilitiesCredential = JsonNode.Parse(vc.Value)!;
                        var capabilities = capabilitiesCredential["credentialSubject"]?["capabilities"]?[url]?.AsArray();
                        if (capabilities != null)
                        {
                            context.Succeed(this);
                        }

                    }
                
            }
            return Task.CompletedTask;
        }
    }
}
