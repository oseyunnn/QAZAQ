namespace QAZAQ.Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


[Table("tourist_reviews")]
public class TouristReview : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("created_at", ignoreOnInsert: true)]
    public DateTime CreatedAt { get; set; }

    [Column("spot_id")]
    public long? SpotId { get; set; }

    [Column("destination_name")]
    public string DestinationName { get; set; } = string.Empty;

    [Column("author_name")]
    public string AuthorName { get; set; } = string.Empty;

    [Column("country")]
    public string Country { get; set; } = string.Empty;

    [Column("rating")]
    public int Rating { get; set; } = 5;

    [Column("travel_month")]
    public string TravelMonth { get; set; } = string.Empty;

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("comment")]
    public string Comment { get; set; } = string.Empty;

    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("helpful_votes")]
    public int HelpfulVotes { get; set; }

    [Column("is_verified_guest")]
    public bool IsVerifiedGuest { get; set; } = true;

    [Column("deleted_at", ignoreOnInsert: true)]
    public DateTime? DeletedAt { get; set; }


    [System.Text.Json.Serialization.JsonIgnore]
    public bool HasVotedHelpful { get; set; }
}
