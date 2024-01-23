using RYT.Models.ViewModels;
using System.Security.Claims;

namespace RYT.Services.Payment
{
    public interface IPaymentService
    {
        public Task<bool> InitializePayment(SendRewardVM model);

        public Task<bool> Withdraw(WithdrawVM model);
        //public void VerifyPayment(string reference);
        //public Task<string> Withdraw(WithdrawVM model);
    }
}
