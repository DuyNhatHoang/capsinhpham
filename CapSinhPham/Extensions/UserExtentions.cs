using System.Security.Claims;

namespace CapSinhPham.Extensions
{
    public static class UserExtentions
    {
        public static string GetUserName(this IEnumerable<Claim> claims)
        {
            var userName = claims.Where(claim => claim.Type == "Username").FirstOrDefault();
            if(userName == null)
            {
                return "";
            }
            return userName.Value;
        }
    }
}
