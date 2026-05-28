using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Security;

namespace CMS.Umbraco.Providers
{
    public class OpenIdExternalMemberLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
    {
        public const string Scheme = "OpenIdConnect";

        public void Configure(string? name, MemberExternalLoginProviderOptions options)
        {
            if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + Scheme) 
                return;

            Configure(options);
        }

        public void Configure(MemberExternalLoginProviderOptions options)
        {
            options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(
                autoLinkExternalAccount: true,
                defaultCulture: null,
                defaultIsApproved: true,
                defaultMemberTypeAlias: "Member",
                defaultMemberGroups: null
                )
            {
                OnAutoLinking = (autoLinkUser, loginInfo) =>
                {
                    // Here you can implement any custom logic before the auto-linking happens.
                },
                OnExternalLogin = (user, loginInfo) =>
                {
                    // Here you can implement any custom logic after the external login has been processed.
                    return true;
                },
            };
        }
    }
}
