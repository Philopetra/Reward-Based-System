using RYT.Models.ViewModels;

namespace RYT.Services.PaymentGateway
{
    public interface IPaymentService
    {
        public Task<string> PaystackAuthorizationUrl(SendRewardVM model);

        public Task<bool> Withdraw(WithdrawVM model);
        public Task<bool> VerifyFunding(SendRewardVM model);

        public Task<IEnumerable<Bank>> GetListOfBanks();

    }
}
