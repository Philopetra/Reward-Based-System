using RYT.Models.Enums;

namespace RYT.Models.ViewModels
{
    public class SendRewardVM
    {
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string TransactionType { get; set; } = TransactionTypes.Funding.ToString();
        public string Description { get; set; }
        public bool Status { get; set; }
        public string Reference { get; set; }

    }
}
