using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthSample
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            #region ServerOAuth

            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions
                ()
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    AuthenticationType = "Emad",
                    Realm = "EHM" //anything
                });

            var options = new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions();
            options.TokenEndpointPath = new PathString("/account/token");
            options.AuthorizeEndpointPath = new PathString("/account/auth");
            options.AllowInsecureHttp = true; //Don't do that!! always be on secure scheme, this is set to "true" for demo purposes
            var provider = new OAuthAuthorizationServerProvider();

            provider.OnValidateClientRedirectUri = (context) =>
            {
                return Task.Run(() =>
                {
                    //Caution: this is not to validate that the uri is valid syntax wise, this is to validate it business wise. If this uri is not
                    //valid syntax wise this entry will not be hit in the first place, and your authentication process will not work!
                    context.Validated();

                });
            };

            provider.OnValidateAuthorizeRequest = (context) =>
                {
                    return Task.Run(() =>
                    {
                        //Authorization validation here
                        //Somewhere in the request you should create the identity and sign in with it, I put it here, it could be a page on your app?
                        context.OwinContext.Authentication.SignIn(new System.Security.Claims.ClaimsIdentity("Bearer"));
                        context.Validated();
                    });
                };

            provider.OnAuthorizeEndpoint = (context) =>
            {
                return Task.Run(() =>
                {
                    //This is the last chance to alter the request, you can either end it here using RequestCompleted and start resonding, 
                    //or you can let it go through to the subsequent middleware, 
                    //except that you have to make sure the response returns a 200, otherwise the whole thing will not work
                    context.RequestCompleted();
                    var str = context.Options.AccessTokenFormat;

                });
            };


            provider.OnValidateClientAuthentication = (context) =>
            {
                return Task.Run(() =>
                {
                    //Client validation here
                    context.Validated();
                });
            };

            options.Provider = provider;

            AuthenticationTokenProvider authTokenProvider = new AuthenticationTokenProvider();
            authTokenProvider.OnCreate = (context) =>
            {
                //create a dummy token 
                context.SetToken("MyTokenblablabla");
            };

            //This is called when a client is requesting with Authorization header and passing the token, like this "Authorization: Bearer jdksjkld"
            authTokenProvider.OnReceive = (context) =>
            {
                //create dummy identity regardless of the validty of the token :)
                var claimsIdentity = new System.Security.Claims.ClaimsIdentity("Bearer");
                claimsIdentity.AddClaim(new Claim("something", "Ahmad")); //This claim type "something" is used for protection from anti-forgery...
                //Check the Global.asax for "AntiForgeryConfig.UniqueClaimTypeIdentifier = "something";"
                //you can avoid setting this, but you have to use the default claims type. check http://bartwullems.blogspot.com.au/2013/09/aspnet-mvc-4-error-when-using-anti.html
                
                context.SetTicket(new Microsoft.Owin.Security.AuthenticationTicket(claimsIdentity,
                    new Microsoft.Owin.Security.AuthenticationProperties
                    {
                        ExpiresUtc = new System.DateTimeOffset(2015, 3, 1, 1, 1, 1, new System.TimeSpan()),
                    }
                    ));
            };

            options.AuthorizationCodeProvider = authTokenProvider;
            options.RefreshTokenProvider = authTokenProvider;
            options.AccessTokenProvider = authTokenProvider;

            app.UseOAuthBearerTokens(options);

            #endregion

            //app.UseGoogleAuthentication();
        }
    }
}