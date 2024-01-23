using RYT.Models.ViewModels;
using System.Security.Claims;

namespace RYT.Services.Payment
{
    public interface IPaymentService
    {
        public Task<string> InitializePayment(SendRewardVM model, ClaimsPrincipal user);
        //public void VerifyPayment(string reference);
        public Task Withdraw(WithdrawVM model);
    }
}
