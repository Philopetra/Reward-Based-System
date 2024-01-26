using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.PaymentGateway;
using RYT.Services.Repositories;

namespace RYT.Services.Payment
{
    public class Payments : IPayments
    {
        private readonly IPaymentService _paymentService;
        private readonly IRepository _repository;
        public Payments(IPaymentService paymentService, IRepository repository)
        {
            _paymentService = paymentService;
            _repository = repository;
        }
        public async Task<bool> Payment(SendRewardVM model)
        {
            var response = await _paymentService.Funding(model);
            if (response != null)
            {
                var transaction = new Transaction
                {
                    Amount = model.Amount,
                    SenderId = model.SenderId,
                    ReceiverId = model.ReceiverId,
                    WalletId = model.WalletId,
                    Reference = response,
                    Status = false,
                    Description = model.Description,
                    TransactionType = model.TransactionType,
                };
                await _repository.AddAsync(transaction);
                return true;
                //var Url = response.Data.AuthorizationUrl;
            }
            return false;
        }
        public async void UpdatePayment(SendRewardVM model)
        {
            var response = Payment(model);
            if (response != null)
            {
                var transaction = new Transaction
                {
                    Status = true
                };
                await _repository.UpdateAsync(transaction);
            }
        }
    }
}
