using System.Collections.Generic;
using Synuit.Idp.Admin.Configuration.Identity;

namespace Synuit.Idp.Admin.Configuration.IdentityServer
{
    public class Client : global::IdentityServer4.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}






