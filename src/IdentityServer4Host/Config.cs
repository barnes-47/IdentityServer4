using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4Host
{
    public class Config
    {
        #region Private Constants
        private const string _Client_ClientId_Console = "console";
        private const string _Client_ClientName_Console = "Console App";
        private const string _Client_ClientSecret_Console = "secret";
        private const string _ApiResource_Name_Api1 = "api1";
        private const string _ApiResource_Name_ApiHost = "api_host";
        private const string _ApiResource_DisplayName_Api1 = "My API One";
        private const string _ApiResource_DisplayName_ApiHost = "My API Host";
        private const string _Scope_ApiHost_FullAccess = "api_host.full_access";
        private const string _Scope_ApiHost_ReadAccess = "api_host.read_access";
        private const string _Scope_ApiHost_WriteAccess = "api_host.write_access";
        private const string _Scope_ApiHost_DeleteAccess = "api_host.delete_access";
        private const string _Scope_DisplayName_FullAccess = "Full access to API Host";
        private const string _Scope_DisplayName_ReadAccess = "Read access to API Host";
        private const string _Scope_DisplayName_WriteAccess = "Write access to API Host";
        private const string _Scope_DisplayName_DeleteAccess = "Delete access to API Host";
        #endregion

        public static IEnumerable<ApiResource> ApiResources
            => new List<ApiResource>
            {
                //Creating ApiResource in this manner will also set the "Scope" => _ApiResource_Name_Api1
                new ApiResource(_ApiResource_Name_Api1, _ApiResource_DisplayName_Api1)

                , new ApiResource
                {
                    Name = _ApiResource_Name_ApiHost
                    , DisplayName = _ApiResource_DisplayName_ApiHost
                    , Scopes =
                    {
                        new Scope
                        {
                            Name = _Scope_ApiHost_FullAccess
                            , DisplayName = _Scope_DisplayName_FullAccess

                            //This UserClaim(other than Name & Email) will be added when a token with this scope is requested.
                            , UserClaims = { JwtClaimTypes.EmailVerified }
                        }
                        , new Scope
                        {
                            Name = _Scope_ApiHost_ReadAccess
                            , DisplayName = _Scope_DisplayName_ReadAccess
                        }
                        , new Scope
                        {
                            Name = _Scope_ApiHost_WriteAccess
                            , DisplayName = _Scope_DisplayName_WriteAccess
                            , UserClaims = { JwtClaimTypes.EmailVerified }
                        }
                        , new Scope
                        {
                            Name = _Scope_ApiHost_DeleteAccess
                            , DisplayName = _Scope_DisplayName_DeleteAccess
                            , UserClaims = { JwtClaimTypes.EmailVerified }
                        }
                    }

                    //These UserClaims will be added when a token for "api_host" is requested.
                    , UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email }
                }
            };

        public static IEnumerable<Client> Clients
            => new List<Client>
            {
                //Registering a console client in our "IdentityServer4Host" application.
                //The console client will call the "ApiHost" on user's behalf.
                new Client
                {
                    ClientId = _Client_ClientId_Console
                    , ClientName = _Client_ClientName_Console
                    , ClientSecrets = new List<Secret>
                    {
                        new Secret(_Client_ClientSecret_Console.Sha256())
                    }

                    , AllowOfflineAccess = false
                    , AllowedGrantTypes = GrantTypes.ClientCredentials
                    , AllowedScopes =
                    {
                        _ApiResource_Name_Api1
                        , _Scope_ApiHost_DeleteAccess
                        , _Scope_ApiHost_FullAccess
                        , _Scope_ApiHost_ReadAccess
                        , _Scope_ApiHost_WriteAccess
                    }
                }
            };
    }
}
