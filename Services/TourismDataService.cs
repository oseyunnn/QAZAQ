namespace QAZAQ.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QAZAQ.Data;
using QAZAQ.Models;
using static Supabase.Postgrest.Constants;

/// <summary>
/// Enterprise-grade data service managing Supabase integration.
/// Implements Habit 1 (indexed queries), Habit 2 (filter in DB before Get), and Habit 3 (single round-trip),
/// while gracefully falling back to local domain memory when Supabase credentials are pending.
/// </summary>
public class TourismDataService : ITourismDataService
{
    private readonly Supabase.Client _db;
    private readonly IConfiguration _config;
    private readonly ILogger<TourismDataService> _logger;
    private readonly bool _isConfigured;

    // Local fallback caches when running offline or pending live Supabase keys
    private readonly List<TouristSpot> _fallbackSpots = new(SampleData.Spots);
    private readonly List<TouristReview> _fallbackReviews = new(SampleData.Reviews);
    private readonly List<TourBooking> _fallbackBookings = new();

    public bool IsConnectedToSupabase { get; private set; }
    public string ConnectionStatusMessage { get; private set; } = string.Empty;

    public TourismDataService(Supabase.Client db, IConfiguration config, ILogger<TourismDataService> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;

        var url = _config["Supabase:Url"]
               ?? _config["SUPABASE_URL"]
               ?? Environment.GetEnvironmentVariable("SUPABASE_URL")
               ?? string.Empty;

        var key = _config["Supabase:Key"]
               ?? _config["Supabase:PublishableKey"]
               ?? _config["SUPABASE_PUBLISHABLE_KEY"]
               ?? _config["SUPABASE_KEY"]
               ?? Environment.GetEnvironmentVariable("SUPABASE_PUBLISHABLE_KEY")
               ?? Environment.GetEnvironmentVariable("SUPABASE_KEY")
               ?? string.Empty;

        _isConfigured = !string.IsNullOrWhiteSpace(url)
                     && !url.Contains("YOUR-REF", StringComparison.OrdinalIgnoreCase)
                     && !string.IsNullOrWhiteSpace(key)
                     && !key.Contains("placeholder", StringComparison.OrdinalIgnoreCase);

        if (_isConfigured)
        {
            IsConnectedToSupabase = true;
            ConnectionStatusMessage = $"Connected to Supabase PostgreSQL at {new Uri(url).Host}";
            _logger.LogInformation("Supabase client initialized with endpoint: {Url}", url);
        }
        else
        {
            IsConnectedToSupabase = false;
            ConnectionStatusMessage = "Supabase configuration pending. Enter your SUPABASE_URL and SUPABASE_KEY in .env or appsettings.json to connect live cloud database. Displaying local seed data.";
            _logger.LogWarning("Supabase credentials not yet configured in .env. Running in offline fallback mode.");
        }
    }

    // =========================================================================
    // DESTINATIONS (TOURIST SPOTS) - Slide 49, 50, 51
    // =========================================================================

    public async Task<List<TouristSpot>> GetSpotsAsync(bool includeSoftDeleted = false)
    {
        if (_isConfigured)
        {
            try
            {
                // Habit 2 (Slide 19, 49): Filter in database before fetch (.Where before .Get), limit first 50 (.Range)
                Supabase.Postgrest.Interfaces.IPostgrestTable<TouristSpot> query = _db.From<TouristSpot>();

                if (!includeSoftDeleted)
                {
                    query = query.Where(p => p.DeletedAt == null);
                }

                var res = await query
                    .Order(p => p.Id, Ordering.Ascending)
                    .Range(0, 49)
                    .Get();

                if (res.Models != null && res.Models.Count > 0)
                {
                    IsConnectedToSupabase = true;
                    return res.Models;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch tourist spots from Supabase. Falling back to local data.");
                IsConnectedToSupabase = false;
                ConnectionStatusMessage = $"Supabase query failed: {ex.Message}. Using fallback data.";
            }
        }

        return includeSoftDeleted 
            ? _fallbackSpots 
            : _fallbackSpots.Where(s => s.DeletedAt == null).ToList();
    }

    public async Task<TouristSpot?> GetSpotByIdAsync(long id)
    {
        if (_isConfigured)
        {
            try
            {
                var res = await _db.From<TouristSpot>()
                    .Where(p => p.Id == id)
                    .Single();
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get spot by ID {Id} from Supabase.", id);
            }
        }

        return _fallbackSpots.FirstOrDefault(s => s.Id == id);
    }

    public async Task<TouristSpot> AddSpotAsync(TouristSpot spot)
    {
        // Habit 1 & Step 7 (Slide 50): sends POST
        if (_isConfigured)
        {
            try
            {
                var res = await _db.From<TouristSpot>().Insert(spot);
                var created = res.Models.FirstOrDefault();
                if (created != null) return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert new tourist spot into Supabase.");
                throw;
            }
        }

        // Fallback local memory
        spot.Id = _fallbackSpots.Count > 0 ? _fallbackSpots.Max(s => s.Id) + 1 : 1;
        _fallbackSpots.Add(spot);
        return spot;
    }

    public async Task<bool> UpdateSpotPriceAsync(long id, decimal newPrice)
    {
        // Step 8 (Slide 51): sends PATCH
        if (_isConfigured)
        {
            try
            {
                await _db.From<TouristSpot>()
                    .Where(x => x.Id == id)
                    .Set(x => x.BasePriceUsd, newPrice)
                    .Update();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update spot price on Supabase.");
                throw;
            }
        }

        var local = _fallbackSpots.FirstOrDefault(s => s.Id == id);
        if (local != null)
        {
            local.BasePriceUsd = newPrice;
            return true;
        }
        return false;
    }

    public async Task<bool> SoftDeleteSpotAsync(long id)
    {
        // Step 8 & Rubric 5 (Slide 51): Soft delete is just an update setting deleted_at
        if (_isConfigured)
        {
            try
            {
                await _db.From<TouristSpot>()
                    .Where(x => x.Id == id)
                    .Set(x => x.DeletedAt, DateTime.UtcNow)
                    .Update();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to soft delete spot on Supabase.");
                throw;
            }
        }

        var local = _fallbackSpots.FirstOrDefault(s => s.Id == id);
        if (local != null)
        {
            local.DeletedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public async Task<bool> RestoreSpotAsync(long id)
    {
        if (_isConfigured)
        {
            try
            {
                await _db.From<TouristSpot>()
                    .Where(x => x.Id == id)
                    .Set(x => x.DeletedAt, null)
                    .Update();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore spot on Supabase.");
                throw;
            }
        }

        var local = _fallbackSpots.FirstOrDefault(s => s.Id == id);
        if (local != null)
        {
            local.DeletedAt = null;
            return true;
        }
        return false;
    }

    // =========================================================================
    // BOOKINGS (RESERVATIONS)
    // =========================================================================

    public async Task<List<TourBooking>> GetBookingsAsync()
    {
        if (_isConfigured)
        {
            try
            {
                var res = await _db.From<TourBooking>()
                    .Where(b => b.DeletedAt == null)
                    .Order(b => b.CreatedAt, Ordering.Descending)
                    .Range(0, 49)
                    .Get();
                return res.Models;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch bookings from Supabase.");
            }
        }

        return _fallbackBookings.Where(b => b.DeletedAt == null).ToList();
    }

    public async Task<TourBooking> CreateBookingAsync(TourBooking booking)
    {
        if (_isConfigured)
        {
            try
            {
                var res = await _db.From<TourBooking>().Insert(booking);
                var created = res.Models.FirstOrDefault();
                if (created != null) return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert booking into Supabase.");
                throw;
            }
        }

        booking.Id = _fallbackBookings.Count > 0 ? _fallbackBookings.Max(b => b.Id) + 1 : 1;
        _fallbackBookings.Insert(0, booking);
        return booking;
    }

    public async Task<bool> CancelBookingAsync(long id)
    {
        if (_isConfigured)
        {
            try
            {
                await _db.From<TourBooking>()
                    .Where(b => b.Id == id)
                    .Set(b => b.DeletedAt, DateTime.UtcNow)
                    .Set(b => b.Status, "Cancelled")
                    .Update();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel booking on Supabase.");
                throw;
            }
        }

        var local = _fallbackBookings.FirstOrDefault(b => b.Id == id);
        if (local != null)
        {
            local.DeletedAt = DateTime.UtcNow;
            local.Status = "Cancelled";
            return true;
        }
        return false;
    }

    // =========================================================================
    // REVIEWS (TESTIMONIALS)
    // =========================================================================

    public async Task<List<TouristReview>> GetReviewsAsync()
    {
        if (_isConfigured)
        {
            try
            {
                var res = await _db.From<TouristReview>()
                    .Where(r => r.DeletedAt == null)
                    .Order(r => r.Id, Ordering.Descending)
                    .Range(0, 49)
                    .Get();

                if (res.Models != null && res.Models.Count > 0)
                {
                    return res.Models;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch reviews from Supabase.");
            }
        }

        return _fallbackReviews.Where(r => r.DeletedAt == null).ToList();
    }

    public async Task<TouristReview> AddReviewAsync(TouristReview review)
    {
        if (_isConfigured)
        {
            try
            {
                var res = await _db.From<TouristReview>().Insert(review);
                var created = res.Models.FirstOrDefault();
                if (created != null) return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert review into Supabase.");
                throw;
            }
        }

        review.Id = _fallbackReviews.Count > 0 ? _fallbackReviews.Max(r => r.Id) + 1 : 1;
        _fallbackReviews.Insert(0, review);
        return review;
    }

    public async Task<bool> VoteReviewHelpfulAsync(long id)
    {
        if (_isConfigured)
        {
            try
            {
                var review = await _db.From<TouristReview>().Where(r => r.Id == id).Single();
                if (review != null)
                {
                    await _db.From<TouristReview>()
                        .Where(r => r.Id == id)
                        .Set(r => r.HelpfulVotes, review.HelpfulVotes + 1)
                        .Update();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increment review helpful votes on Supabase.");
            }
        }

        var local = _fallbackReviews.FirstOrDefault(r => r.Id == id);
        if (local != null && !local.HasVotedHelpful)
        {
            local.HelpfulVotes++;
            local.HasVotedHelpful = true;
            return true;
        }
        return false;
    }

    public async Task<bool> SoftDeleteReviewAsync(long id)
    {
        if (_isConfigured)
        {
            try
            {
                await _db.From<TouristReview>()
                    .Where(r => r.Id == id)
                    .Set(r => r.DeletedAt, DateTime.UtcNow)
                    .Update();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to soft delete review on Supabase.");
                throw;
            }
        }

        var local = _fallbackReviews.FirstOrDefault(r => r.Id == id);
        if (local != null)
        {
            local.DeletedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }
}
