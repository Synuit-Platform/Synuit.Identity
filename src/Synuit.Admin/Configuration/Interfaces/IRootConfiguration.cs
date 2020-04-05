namespace Synuit.Idp.Admin.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        AdminConfiguration AdminConfiguration { get; }
        IdentityDataConfiguration IdentityDataConfiguration { get; }
        IdentityServerDataConfiguration IdentityServerDataConfiguration { get; }
    }
}





