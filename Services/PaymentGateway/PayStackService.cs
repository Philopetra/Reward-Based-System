using PayStack.Net;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;
using RYT.Utilities;
using Bank = RYT.Models.ViewModels.Bank;

namespace RYT.Services.PaymentGateway
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

        public async Task<string> PaystackAuthorizationUrl(SendRewardVM model)
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
            if (!response.Status) return response.Message;
            return response.Data.AuthorizationUrl;
        }

        public async Task<bool> VerifyFunding(SendRewardVM model)
        {
            var response = await PaystackAuthorizationUrl(model);
            //Verify transaction
            var verifyResponse = _payStack.Transactions.Verify(response);

            if (verifyResponse.ToString() != "success")
                return false;
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

        public async Task<IEnumerable<Bank>> GetListOfBanks()
        {
            var result = _payStack.Get<ApiResponse<dynamic>>("bank?currency=NGN");

            if (!result.Status)
                throw new Exception("Unable to fetch banks");

            var banks = (result.Data as IEnumerable<dynamic>)?
                .Select(bank => new Bank(bank.name, bank.code));

            return banks;
        }


    }
}
