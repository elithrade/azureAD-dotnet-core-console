using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace azureAD_dotnet_core_console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task RunAsync()
        {
            AuthConfig authConfig = AuthConfig.ReadFromJson("appsettings.json");

            // Initialise public client

            var app = PublicClientApplicationBuilder.Create(authConfig.ClientId)
                                                    .WithRedirectUri(authConfig.RedirectUri)
                                                    .WithAuthority(new Uri(authConfig.Authority))
                                                    .Build();

            var authResult = await AcquireATokenFromCacheOrInteractive(app, authConfig.Scopes);

            Console.WriteLine(authResult.AccessToken);
        }

        private async static Task<AuthenticationResult> AcquireATokenFromCacheOrInteractive(IPublicClientApplication app, string[] scopes)
        {
            AuthenticationResult result = null;
            var accounts = await app.GetAccountsAsync();

            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                       .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent.
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                Console.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    result = await app.AcquireTokenInteractive(scopes)
                          .ExecuteAsync();
                }
                catch (MsalException msalEx)
                {
                    Console.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{msalEx}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            }

            return result;
        }
    }
}
