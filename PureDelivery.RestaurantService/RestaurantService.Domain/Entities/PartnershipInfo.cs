namespace RestaurantService.Domain.Entities
{
    public class PartnershipInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; }

        public bool IsActive { get; set; }

        // Финансовые условия
        public decimal CommissionRate { get; set; } // 0.15 = 15%
        public bool ParticipatesInLoyalty { get; set; } = false;
        public decimal LoyaltyPointsRate { get; set; } = 0.05m; // 5% від суми замовлення → points


        // Контракт
        public DateTime ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string ContractNumber { get; set; } = string.Empty;

        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
