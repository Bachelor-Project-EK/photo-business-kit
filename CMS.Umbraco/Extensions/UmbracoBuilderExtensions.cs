using CMS.Umbraco.Providers;
using System.Net;
using System.Security.Claims;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace CMS.Umbraco.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddOpenIdExternalMemberLogin(this IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<OpenIdExternalMemberLoginProviderOptions>();

            builder.AddMemberExternalLogins(logins =>
            {
                logins.AddMemberLogin(
                    memberAuthenticationBuilder =>
                    {
                        memberAuthenticationBuilder.AddOpenIdConnect(
                            memberAuthenticationBuilder.SchemeForMembers(OpenIdExternalMemberLoginProviderOptions.Scheme),
                            options =>
                            {
                                var config = builder.Config;
                                options.ResponseType = "code";
                                options.Scope.Add("openid");
                                options.Scope.Add("profile");
                                options.Scope.Add("email");
                                options.Scope.Add("phone");
                                options.Scope.Add("address");
                                options.RequireHttpsMetadata = true;
                                options.MetadataAddress = config["OpenIdConnect:MetadataAddress"];
                                options.ClientId = config["OpenIdConnect:ClientId"];
                                options.ClientSecret = config["OpenIdConnect:ClientSecret"];
                                options.SaveTokens = true;
                                options.TokenValidationParameters.SaveSigninToken = true;
                                options.Events.OnTokenValidated = async context =>
                                {
                                    var claims = context?.Principal?.Claims.ToList();
                                    var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                                    if (email != null)
                                    {
                                        claims?.Add(new Claim(ClaimTypes.Email, email.Value));
                                    }

                                    var name = claims?.FirstOrDefault(x => x.Type == "user_displayname");
                                    if (name != null)
                                    {
                                        claims?.Add(new Claim(ClaimTypes.Name, name.Value));
                                    }
                                    else
                                    {
                                        name = claims?.FirstOrDefault(x => x.Type == "nickname");
                                        if (name != null)
                                        {
                                            claims?.Add(new Claim(ClaimTypes.Name, name.Value));
                                        }
                                    }

                                    if (context != null)
                                    {
                                        var authenticationType = context.Principal?.Identity?.AuthenticationType;
                                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
                                    }

                                    await Task.FromResult(0);
                                };
                                options.Events.OnRedirectToIdentityProviderForSignOut = async notification =>
                                {
                                    var protocolMessage = notification.ProtocolMessage;

                                    var logoutUrl = config["OpenIdConnect:LogoutUrl"];
                                    var returnAfterLogout = config["OpenIdConnect:ReturnAfterLogout"];
                                    if (!string.IsNullOrEmpty(logoutUrl) && !string.IsNullOrEmpty(returnAfterLogout))
                                    {
                                        protocolMessage.IssuerAddress =
                                            $"{config["OpenIdConnect:LogoutUrl"]}" +
                                            $"?client_id={config["OpenIdConnect:ClientId"]}" +
                                            $"&returnTo={WebUtility.UrlEncode(config["OpenIdConnect:ReturnAfterLogout"])}";
                                    }

                                    var memberManager = notification.HttpContext.RequestServices.GetRequiredService<IMemberManager>();
                                    if (memberManager != null)
                                    {
                                        var currentMember = await memberManager.GetCurrentMemberAsync();

                                        var idToken = currentMember?.LoginTokens.FirstOrDefault(x => x.Name == "id_token");
                                        if (idToken != null && !string.IsNullOrEmpty(idToken.Value))
                                        {
                                            protocolMessage.IdTokenHint = idToken.Value;
                                        }
                                    }

                                    await Task.FromResult(0);
                                };
                            });
                    });
            });
            return builder;
        }
    }
}
