using RYT.Models.ViewModels;

namespace RYT.Services.Payment
{
    public interface IPayments
    {
        public Task<bool> Payment(SendRewardVM model);
        public void UpdatePayment(SendRewardVM model);
    }
}
