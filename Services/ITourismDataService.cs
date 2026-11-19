namespace QAZAQ.Services;

using QAZAQ.Models;

/// <summary>
/// High-level data service contract providing simplified, robust access to Supabase data
/// across all pages of the application, with automatic fallback and fast habit optimizations.
/// </summary>
public interface ITourismDataService
{
    bool IsConnectedToSupabase { get; }
    string ConnectionStatusMessage { get; }

    // Destinations (Tourist Spots) - CRUD + Soft Delete
    Task<List<TouristSpot>> GetSpotsAsync(bool includeSoftDeleted = false);
    Task<TouristSpot?> GetSpotByIdAsync(long id);
    Task<TouristSpot> AddSpotAsync(TouristSpot spot);
    Task<bool> UpdateSpotPriceAsync(long id, decimal newPrice);
    Task<bool> SoftDeleteSpotAsync(long id);
    Task<bool> RestoreSpotAsync(long id);

    // Bookings - CRUD
    Task<List<TourBooking>> GetBookingsAsync();
    Task<TourBooking> CreateBookingAsync(TourBooking booking);
    Task<bool> CancelBookingAsync(long id);

    // Reviews - CRUD
    Task<List<TouristReview>> GetReviewsAsync();
    Task<TouristReview> AddReviewAsync(TouristReview review);
    Task<bool> VoteReviewHelpfulAsync(long id);
    Task<bool> SoftDeleteReviewAsync(long id);
}
