namespace RYT.Models.ViewModels
{
    public class OverviewViewModel
    {
        public List<SentTransactionViewModel> MySentTransactions { get; set; } = new List<SentTransactionViewModel>();
        public List<ReceivedTransactionsViewModel> MyReceivedTransactions { get; set; } = new List<ReceivedTransactionsViewModel>();
    }
}
