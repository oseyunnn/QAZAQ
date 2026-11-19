namespace QAZAQ.Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

/// <summary>
/// Domain model for persistent tour bookings and reservations
/// mapped to Supabase 'bookings' table with foreign key to 'tourist_spots' (Milestone Step 5 / Slide 14, 48).
/// </summary>
[Table("bookings")]
public class TourBooking : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("spot_id")]
    public long SpotId { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("trip_style")]
    public string TripStyle { get; set; } = "Luxury Escape";

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    [Column("travel_date")]
    public DateTime TravelDate { get; set; } = DateTime.Today.AddDays(14);

    [Column("travelers")]
    public int Travelers { get; set; } = 2;

    [Column("include_guide")]
    public bool IncludeGuide { get; set; } = true;

    [Column("include_yurt_experience")]
    public bool IncludeYurtExperience { get; set; } = true;

    [Column("special_requests")]
    public string? SpecialRequests { get; set; }

    [Column("total_price_usd")]
    public decimal TotalPriceUsd { get; set; }

    [Column("voucher_code")]
    public string VoucherCode { get; set; } = string.Empty;

    [Column("status")]
    public string Status { get; set; } = "Confirmed";

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime? CreatedAt { get; set; }
}
