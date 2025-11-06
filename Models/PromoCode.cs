using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models
{
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }

    public class PromoCode
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        public int? MaxUses { get; set; }
        public int TimesUsed { get; set; } = 0;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinimumPurchase { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Referral
    {
        public int Id { get; set; }

        public string ReferrerId { get; set; } = string.Empty;
        public ApplicationUser Referrer { get; set; } = null!;

        public string ReferredId { get; set; } = string.Empty;
        public ApplicationUser Referred { get; set; } = null!;

        [StringLength(50)]
        public string ReferralCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal RewardAmount { get; set; }

        public bool RewardClaimed { get; set; } = false;

        public DateTime ReferredDate { get; set; }
        public DateTime? RewardClaimedDate { get; set; }
    }
}
