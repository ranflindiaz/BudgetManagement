using BudgetManagement.Interface;
using System.Security.Claims;

namespace BudgetManagement.Services
{
    public class UsersServices : IUsersServices
    {
        private readonly HttpContext httpContext;

        public UsersServices(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        public int GetUserId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User
                    .Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("User is not authenticated.");
            }

        }
    }
}
