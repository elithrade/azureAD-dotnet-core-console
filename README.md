# Authenticating a .NETCore Console Application using Azure AD

This repository contains sample code, written in C# using .NETCore, that interact with Azure AD to acquire an `accessToken` that needed to talk an authenticated Web API service. In this repository, we have configured the console app to talk to a authenticated [`cascade entity store`](https://github.com/fugro/cascade.entity_store) AWS Fargate service.

For more information on how to setup and deploy an authenticated Fargate service, please refer to [`fugro.cascade.app.cdk.template
`](https://github.com/fugro/fugro.cascade.app.cdk.template) repository, as setting it up is beyond the scope of this document. We will assume the authenticated service is deployed and running, we will configure our console app to acquire an `accessToken` which is required to interact with the service.

## App registration

In order to interact with Azure AD, we need to register an app in the Azure portal so the Microsoft identity platform can provide authentication and authorization services for our application and its users. The Microsoft identity platform performs identity and access management (IAM) only for registered applications. Whether it's a client application like a web or mobile app, or it's a web API that backs a client app, registering it establishes a trust relationship between your application and the identity provider, the Microsoft identity platform.

Please follow these steps to create the app registration for our console app:
1. Sign in to the [Azure portal](https://portal.azure.com/#home).
2. Search for and select **Azure Active Directory**.
3. Under **Manage**, select **App registrations > New registration**.
4. Enter a Name for your application. Users of your app might see this name. You can change it later.
5. Specify who can use the application, sometimes called its sign-in audience. We select **Accounts in this organizational directory only (Fugro only - Single tenant)** because we only only allow Fugro users to sign in our app.

![](/README/account-type-selection.png)

6. Under **Redirect URI**, select **Public client/native (mobile & desktop)** and enter `http://localhost` as the Redirect URI. Please take a note of this URL as we will use it to configure our console app later on.

![](/README/redirect-uri.png)

7. Select **Register** to complete the initial app registration.

When registration finishes, the Azure portal displays the app registration's Overview pane. You see the Application (client) ID. Also called the client ID, this value uniquely identifies your application in the Microsoft identity platform.

Client ID is used by Microsoft Azure AD to validate against the app registration.

![](/README/overview.png)

We now have registered our console app, the next section will configure our app so it can interact with Azure AD. Please note that in order to talk to an authenticated web API, the web API has to be registered and expose itself in Azure AD. We have registered the authenticated `cascade entity store` app for this sample console app, but for more information on how to register and expose a web API please [refer to the documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-expose-web-apis).

## App configuration

Open `appsettings.json` in this repository, we can see the mandatory configurations needed for our app.
```
{
	"ClientId": "a01cb876-1474-4cba-ab38-cd75aedadc20",
	"TenantId": "e3b48527-4cbe-42a2-b4d2-11b3cc7a86fc",
	"RedirectUri": "http://localhost",
	"Scopes": [ "d8f639a1-2eae-43ab-ba40-3757d25b9880/All.All" ]
}
```
* `ClientId`: The client/app ID we got after we registered our app in Azure AD
* `TenantId`: The tenant in which our app is hosted, in our case it will be the Fugro tenant ID as shown in the previous section under **Directory (tenant) ID***. Fugro only has one tenant for all the apps register, so this is not really needed but we keep it in our configuration file for more flexibilities.
* `RedirectUri`: This should be `http://localhost` as we specified when we register our app. Please note that we are using a interactive authentication process, a web page will be opened as a user to sign in and consent when we acquire an `accessToken`. Please note that the user must consent access to the app in order to use it. Microsoft's authentication library `MSAL` provides many other sign-in options for different workflows and environments for more information please [refer to the documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-authentication-flows).
* `Scopes`: The scopes defined in the authenticated API our app wants to talk to. Note the format is `WebApiClientId/scope`. By specifying a web API's scopes in your client app's registration, the client app can obtain an access token containing those scopes from the Microsoft identity platform. The web API app can then provide permission-based access to its resources based on the scopes found in the access token. In our case the `guid` part refers to the client/app ID of the authenticated `cascade entity store` web API. `All.All` is a scope within the app that represents all permissions. We can define other scopes such as `ReadOnly` but it is up to the `cascade entity store` to interpret the scopes and react accordingly. 

## Running the sample app

Simply do a `dotnet build && dotnet run`, the app will print the `accessToken` acquired. The user will be asked to sign in and grant permission if it is the first time that user runs the app. After user signed in the web page can be closed.

![](/README/signin.png)
```
eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiJkOGY2MzlhMS0yZWFlLTQzYWItYmE0MC0zNzU3ZDI1Yjk4ODAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9lM2I0ODUyNy00Y2JlLTQyYTItYjRkMi0xMWIzY2M3YTg2ZmMvIiwiaWF0IjoxNjEzNjE3NTcyLCJuYmYiOjE2MTM2MTc1NzIsImV4cCI6MTYxMzYyMTQ3MiwiYWNyIjoiMSIsImFpbyI6IkFUUUF5LzhUQUFBQVRSRFVzbjIzL3VBTTQzSWZxVWhXNG1ZTGt1SnpjL21UZmVvQU4zVmdTSGM0WSs2Y0ZuWmREK3lXY1ZoMjFSejYiLCJhbXIiOlsicHdkIl0sImFwcGlkIjoiYTAxY2I4NzYtMTQ3NC00Y2JhLWFiMzgtY2Q3NWFlZGFkYzIwIiwiYXBwaWRhY3IiOiIwIiwiZmFtaWx5X25hbWUiOiJIdSIsImdpdmVuX25hbWUiOiJCaW4iLCJpcGFkZHIiOiIxMjIuMTQ4LjI0Ny4yNDgiLCJuYW1lIjoiSHUsIEJpbiIsIm9pZCI6ImQ1MGQzZThmLTQwYjktNDAyYy1hOGNmLWU2MmZmMGVkYWMxNiIsIm9ucHJlbV9zaWQiOiJTLTEtNS0yMS0xNTQyMjE4MjY3LTM0NDA3MjM3MDktMzY0MjY0NzkwOC03MDAzIiwicmgiOiIwLkFBQUFKNFcwNDc1TW9rSzAwaEd6ekhxR19IYTRIS0IwRkxwTXF6ak5kYTdhM0NBTkFMSS4iLCJzY3AiOiJBbGwuQWxsIiwic3ViIjoiWVlPdDlCV1MzRW5YbHIzbnV4YWozdXE3VHdVQkVFS0lqS1I2a3RFRGxwSSIsInRpZCI6ImUzYjQ4NTI3LTRjYmUtNDJhMi1iNGQyLTExYjNjYzdhODZmYyIsInVuaXF1ZV9uYW1lIjoiYi5odUBmdWdyby5jb20iLCJ1cG4iOiJiLmh1QGZ1Z3JvLmNvbSIsInV0aSI6Ildia2w2QTkycWtXaWZxeVhTMDBCQUEiLCJ2ZXIiOiIxLjAifQ.E7lJC97ZX5-tLIebdYOgzrvyE8ZB-bNH6naVnvRtBRImrGHP1HvMbYgvm2Uy4Yw4t4LtQ5K4PVwHCi9lfMsEmajAw7J1KRoMxTseZzFSpFHwf8dRZx5foqyNjMK8fdE0C4sAnIk3jT8IXLaEO-erhmcgpDxRIq4OIdYWjfLdM6_BkPZYUbFnvZuf8ktd4qbUfScNQJky1Qgsc0_x5IqvpJZsGfQXZ-RdYvJahKB16a7nisW2pl0WvR2uIHLeZNipxU5MKlKBt_wYYro5c6ozH9yvr2gRose5423DklCFjGFhvX8P3yWlSWuRskiwcQoB_YQtaBTVKfjSz3ats5DHdg

```

Once we received a `accessToken`, we can inspect it more closely in [jwt.io](jhttps://jwt.io), and it should show the correct `scp` (scopes) and `aud` (audience).

![](/README/jwt.png)

Once we received a valid `accessToken` we can use it in our `http` request header by specifying `Authorization Bearer {accessToken}`. We can also use tools such as Postman to test the token.

An authenticated `cascade entity store` instance has been deployed to [https://dev.cascade.apac-au-all-rdgdm-dev.fugro-dev.com](https://dev.cascade.apac-au-all-rdgdm-dev.fugro-dev.com) in the Datalogistics DEV AWS account. We will have different URLs deployed for different environments later.

Open Postman and create a `GET` request for endpoint `https://dev.cascade.apac-au-all-rdgdm-dev.fugro-dev.com/entities`, click **Authorization** tab and choose **Bearer Token**, copy the `accessToken` and paste it in the text box and click **Send** button, a status code 200 should be returned from the `cascade entity store` instance.

![](/README/postman.png)


# Learn more
Microsoft's documentation on how to interact with Azure AD can be overwhelming, you need to figure out what your workflow is and which authentication method fits the workflow the best. The same applies when figuring out which API to call to acquire a `accessToken`.

A good start can be found [here](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-create-new-tenant).
A collection of samples for different workflows can be found in [GitHub](https://github.com/AzureAD/microsoft-authentication-library-for-js/wiki/Samples) as well.
