using RYT.Services.Repositories;
using System.Security.Claims;

namespace RYT.Utilities
{
    public class Helper
    {
        private readonly IRepository Repository;
        public Helper(Repository repository)
        {

            Repository = repository;

        }
        public static string SenderId()
        {
            ClaimsPrincipal user = new();
            var senderId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return senderId;
        }


    }
}
