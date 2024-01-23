using RYT.Models.ViewModels;

namespace RYT.Services.Payment
{
    public interface IPaymentService
    {
        public Task<bool> InitializePayment(SendRewardVM model);

        public Task<bool> Withdraw(WithdrawVM model);

        public Task<IEnumerable<Bank>> GetListOfBanks();
        //public void VerifyPayment(string reference);
        //public Task<string> Withdraw(WithdrawVM model);
    }
}
