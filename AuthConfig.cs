using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace azureAD_dotnet_core_console
{
    public class AuthConfig
    {
        /// <summary>
        /// Instance of Azure AD, for example public Azure or a Sovereign cloud (Azure China, Germany, US government, etc ...)
        /// </summary>
        public string Instance { get; } = "https://login.microsoftonline.com/{0}";

        /// <summary>
        /// Tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Guid used by the application to uniquely identify itself to Azure AD
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// URL of the authority
        /// </summary>
        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, TenantId);
            }
        }

        /// <summary>
        /// The redirect URI configured when this application is registered to Azure AD
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Web Api scopes that the client app needs to use when requesting a access token.
        /// </summary>
        public string[] Scopes { get; set; }

        /// <summary>
        /// Reads the configuration from a json file
        /// </summary>
        /// <param name="path">Path to the configuration json file</param>
        /// <returns>AuthConfig read from the json file</returns>
        public static AuthConfig ReadFromJson(string path)
        {
            IConfigurationRoot Configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            Configuration = builder.Build();

            return Configuration.Get<AuthConfig>();
        }
    }



}

