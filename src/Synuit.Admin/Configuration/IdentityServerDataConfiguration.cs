using IdentityServer4.Models;
using System.Collections.Generic;
using Client = Synuit.Idp.Admin.Configuration.IdentityServer.Client;

namespace Synuit.Idp.Admin.Configuration
{
    public class IdentityServerDataConfiguration
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<IdentityResource> IdentityResources { get; set; } = new List<IdentityResource>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
    }
}






