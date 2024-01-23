﻿using RYT.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RYT.Models.Entities
{
    public class Transaction : BaseEntity
    {
        public string WalletId { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string TransactionType { get; set; } = TransactionTypes.Funding.ToString();
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; } //ADDED
        public string Reference { get; set; } //ADDED

        // Navigation Props
        public AppUser? Sender { get; set; }
        public Wallet? Wallet { get; set; }
    }
}
