namespace QAZAQ.Models;

using Supabase.Postgrest.Attributes;

/// <summary>
/// Domain model for Kazakhstan tourist spot destinations inheriting from DestinationBase
/// and mapped to Supabase 'tourist_spots' table (Milestone Step 5 / Slide 48).
/// </summary>
[Table("tourist_spots")]
public class TouristSpot : DestinationBase
{
    [Column("highlights")]
    public List<string> Highlights { get; set; } = new();

    [Column("difficulty")]
    public string DifficultyString { get; set; } = "Easy";

    [System.Text.Json.Serialization.JsonIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("_difficulty_enum")]
    public DifficultyLevel Difficulty
    {
        get => Enum.TryParse<DifficultyLevel>(DifficultyString, true, out var d) ? d : DifficultyLevel.Easy;
        set => DifficultyString = value.ToString();
    }

    [Column("duration_days")]
    public int DurationDays { get; set; } = 1;

    [Column("altitude_meters")]
    public int AltitudeMeters { get; set; }

    [Column("is_unesco_heritage")]
    public bool IsUnescoHeritage { get; set; }

    [Column("is_popular")]
    public bool IsPopular { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsSaved { get; set; }

    /// <summary>
    /// Overridden polymorphic pricing logic with group discounting and optional guide surcharge.
    /// </summary>
    public override decimal CalculatePackageCost(int travelers, bool includeNomadicGuide)
    {
        int validTravelers = Math.Max(1, travelers);
        decimal baseCost = BasePriceUsd * validTravelers;

        // Group incentive: 12% discount for groups of 4 or more
        if (validTravelers >= 4)
        {
            baseCost *= 0.88m;
        }

        // Daily guide fee
        if (includeNomadicGuide)
        {
            baseCost += 65m * Math.Max(1, DurationDays);
        }

        return Math.Round(baseCost, 2);
    }
}
