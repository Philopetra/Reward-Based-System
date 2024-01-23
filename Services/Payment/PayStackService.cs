using PayStack.Net;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;
using RYT.Utilities;
using System.Security.Claims;

namespace RYT.Services.Payment
{
    public class PayStackService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly RYTDbContext _context;
        private readonly IRepository Repository;
        private readonly string token;
        private PayStackApi PayStack { get; set; }
        public string Url { get; set; }
        public PayStackService(IConfiguration configuration, RYTDbContext context, IRepository repository)
        {
            _configuration = configuration;
            _context = context;
            Repository = repository;
            token = _configuration["Payment:PayStack"];
            PayStack = new PayStackApi(token);

        }
        public async Task<string> InitializePayment(SendRewardVM model, ClaimsPrincipal user)
        {
            var senderId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderEmail = (await Repository.GetAsync<AppUser>())
                .Where(s => s.Id == model.SenderId)
                .Select(s => new { s.Email })
                .First()
                .Email;


            var request = new TransactionInitializeRequest()
            {
                AmountInKobo = (int)model.Amount * 100,
                Email = senderEmail,
                Currency = "NGN",
                CallbackUrl = "https://localhost:7238/dashboard/listofrewards"
            };
            TransactionInitializeResponse response = PayStack.Transactions.Initialize(request);
            if (response.Status)
            {
                var transaction = new Transaction()
                {
                    Amount = model.Amount,
                    SenderId = Helper.SenderId(),
                    ReceiverId = "",
                    WalletId = "",
                    Reference = response.Data.Reference,
                    Status = false,
                    Description = model.Description,
                    TransactionType = model.TransactionType.ToString(),
                };
                await Repository.AddAsync<Transaction>(transaction);
                Url = response.Data.AuthorizationUrl.ToString();


                //Verify transaction
                TransactionVerifyResponse verifyResponse = PayStack.Transactions.Verify(response.Data.Reference);
                if (verifyResponse.ToString() == "success")
                {
                    var transactionRef = _context.Transactions.Where(x => x.Reference == response.Data.Reference).FirstOrDefault();
                    if (transactionRef != null)
                    {
                        transaction.Status = true;
                    }
                }
                await Repository.UpdateAsync<Transaction>(transaction);


            }
            return Url;
        }

        //public Task<string> Withdraw(SendRewardVM model)
        //{

        //}

    }
}
