namespace QAZAQ.Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

/// <summary>
/// Abstract base class illustrating OOP inheritance, encapsulation, and polymorphism,
/// integrated with Supabase Postgrest BaseModel mapping (Milestone Step 5 / Slide 48).
/// </summary>
public abstract class DestinationBase : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("kazakh_name")]
    public string KazakhName { get; set; } = string.Empty;

    [Column("region")]
    public string RegionString { get; set; } = "AlmatyRegion";

    [System.Text.Json.Serialization.JsonIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("_region_enum")]
    public Region Region
    {
        get => Enum.TryParse<Region>(RegionString, true, out var r) ? r : Region.AlmatyRegion;
        set => RegionString = value.ToString();
    }

    [Column("category")]
    public string CategoryString { get; set; } = "NaturalWonder";

    [System.Text.Json.Serialization.JsonIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("_category_enum")]
    public SpotCategory Category
    {
        get => Enum.TryParse<SpotCategory>(CategoryString, true, out var c) ? c : SpotCategory.NaturalWonder;
        set => CategoryString = value.ToString();
    }

    [Column("short_description")]
    public string ShortDescription { get; set; } = string.Empty;

    [Column("full_description")]
    public string FullDescription { get; set; } = string.Empty;

    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("base_price_usd")]
    public decimal BasePriceUsd { get; set; }

    [Column("rating")]
    public double Rating { get; set; } = 5.0;

    [Column("review_count")]
    public int ReviewCount { get; set; }

    [Column("best_season")]
    public string BestSeason { get; set; } = string.Empty;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Polymorphic cost calculation customized by subclasses.
    /// </summary>
    public abstract decimal CalculatePackageCost(int travelers, bool includeNomadicGuide);

    /// <summary>
    /// Formatted location tag for visual cards.
    /// </summary>
    public virtual string GetFormattedRegion()
    {
        return Region switch
        {
            Region.AlmatyRegion => "Almaty Region",
            Region.Mangystau => "Mangystau Peninsula",
            Region.Turkistan => "Turkistan Silk Road",
            Region.AstanaCapital => "Astana Capital",
            Region.EastKazakhstan => "East Kazakhstan / Altai",
            Region.BalkhashRegion => "Balkhash Region",
            Region.WesternKazakhstan => "Western Kazakhstan",
            _ => Region.ToString()
        };
    }
}
