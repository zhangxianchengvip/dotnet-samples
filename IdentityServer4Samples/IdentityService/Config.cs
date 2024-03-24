using IdentityServer4.Models;

namespace IdentityService
{
    public static class Config
    {
        //定义scope
        public static IEnumerable<ApiScope> ApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("api1", "api1")
            };

        }
        //定义 client

        public static IEnumerable<Client> Clients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // 没有交互式用户，使用 clientid/secret 进行身份验证
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // 用于身份验证的密钥
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // 客户端有权访问的范围
                    AllowedScopes = { "api1" }
                }
            };
        }
    }
}
