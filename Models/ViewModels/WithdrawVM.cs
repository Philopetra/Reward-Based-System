namespace RYT.Models.ViewModels
{
    public class WithdrawVM
    {
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
