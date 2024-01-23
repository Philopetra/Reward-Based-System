using PayStack.Net;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;
using System.Security.Claims;
using RYT.Utilities;

namespace RYT.Services.Payment
{
    public class PayStackService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository _repository;
        private readonly string _secretKey;
        private readonly PayStackApi _payStack;
        public string Url { get; set; }

        public PayStackService(IConfiguration configuration, RYTDbContext context, IRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
            _secretKey = _configuration["Payment:PayStackSecretKey"];
            _payStack = new PayStackApi(_secretKey);
        }

        public async Task<bool> InitializePayment(SendRewardVM model)
        {
            var senderEmail = (await _repository.GetAsync<AppUser>())
                .Where(s => s.Id == model.SenderId)
                .Select(s => s.Email)
                .First();

            var request = new TransactionInitializeRequest
            {
                AmountInKobo = (int)model.Amount * 100,
                Email = senderEmail,
                Currency = "NGN",
                CallbackUrl = _configuration["Payment:PayStackCallbackUrl"],
                Reference = TransactionHelper.GenerateTransRef(),
            };

            var response = _payStack.Transactions.Initialize(request);

            if (!response.Status) return false;

            var transaction = new Transaction
            {
                Amount = model.Amount,
                SenderId = model.SenderId,
                ReceiverId = model.ReceiverId,
                WalletId = model.WalletId,
                Reference = response.Data.Reference,
                Status = false,
                Description = model.Description,
                TransactionType = model.TransactionType,
            };
            await _repository.AddAsync(transaction);
            Url = response.Data.AuthorizationUrl;

            //Verify transaction
            var verifyResponse = _payStack.Transactions.Verify(response.Data.Reference);

            if (verifyResponse.ToString() != "success")
                return false;

            transaction.Status = true;

            await _repository.UpdateAsync(transaction);

            return true;
        }

        public async Task<bool> Withdraw(WithdrawVM model)
        {
            var result = _payStack.Post<ApiResponse<dynamic>, dynamic>("transferrecipient", new
            {
                type = "nuban",
                name = model.AccountName,
                account_number = model.AccountNumber,
                bank_code = model.BankCode,
                currency = "NGN",
            });

            return result.Status;
        }
    }
}