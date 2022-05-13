//@BaseCode
//MdStart
#if ACCOUNT_ON
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace QuickTemplate.Logic.Modules.Security
{
    internal partial class JsonWebToken
    {
        static JsonWebToken()
        {
            ClassConstructing();
            Key = Configuration.AppSettings.Get($"JwtSetting:{nameof(Key)}", "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1");
            Issuer = Configuration.AppSettings.Get($"JwtSetting:{nameof(Issuer)}", nameof(QuickTemplate));
            Audience = Configuration.AppSettings.Get($"JwtSetting:{nameof(Audience)}", nameof(Logic));
            var settingValue = Configuration.AppSettings.Get($"JwtSetting:{nameof(TimeOutInSec)}", Authorization.DefaultTimeOutInSeconds.ToString());
            TimeOutInSec = Convert.ToInt32(settingValue);
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();

        internal static string Key { get; set; }
        internal static string Issuer { get; set; }
        internal static string Audience { get; set; }
        internal static int TimeOutInSec { get; set; }

        internal static string GenerateToken(IEnumerable<Claim> claimsParam)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var secToken = new JwtSecurityToken(
                signingCredentials: credentials,
                issuer: Issuer,
                audience: Audience,
                claims: claimsParam,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(TimeOutInSec));

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(secToken);
        }
        public static bool CheckToken(string token)
        {
            return CheckToken(token, out _);
        }
        internal static bool CheckToken(string token, out SecurityToken? validatedToken)
        {
            var result = false;
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                result = true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
            }
            return result;
        }
        internal static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)) // The same key as the one that generate the token
            };
        }
    }
}
#endif
//MdEnd