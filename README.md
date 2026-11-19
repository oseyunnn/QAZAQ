# QAZAQ — Kazakhstan Tourism Paradise

QAZAQ is a modern, premium Kazakhstan tourism web application built with **ASP.NET Core (Blazor Interactive Server .NET 8)**, **Tailwind CSS**, and backed by a **Supabase PostgreSQL Cloud Database**.

The platform combines destination discovery, cultural heritage highlights, verified traveler journals, dynamic pricing estimation, and interactive reservation management.

---

## 🏛️ Database Milestone Implementation (CIT-U Lecture Milestone)

This project satisfies all requirements from **Part VI: Project Milestone (Pages 43–52)** in *APIs and Databases: Where Your App's Data Lives* (Instructor: John Quinnvic G. Taboada, CIT-U Computer Studies Department).

### 📋 Milestone Rubric Compliance Tracker (100 / 100 Points)

| Rubric Milestone | Points | Implementation Details | Files Involved |
| :--- | :---: | :--- | :--- |
| **Table Design** | 20 pts | PostgreSQL tables with `bigint generated always as identity primary key`, relational foreign keys, `check` constraints, indexes, `GRANT` permissions (Lock 1), and Row Level Security (RLS) open policies (Lock 2). | `database/schema.sql` |
| **Read** | 15 pts | Destinations catalog loads rows directly from Supabase via `Db.From<TouristSpot>().Where(p => p.DeletedAt == null).Order(p => p.Id, Ordering.Ascending).Range(0, 49).Get()`. | `Components/Pages/Spots.razor` |
| **Create, Update, Delete** | 25 pts | **Create**: Add Destination modal with double-click guard (`disabled="@saving"`) sending `POST`.<br>**Update**: Quick price bump (`+$10`) sending `PATCH`.<br>**Delete**: Soft delete button setting `deleted_at = DateTime.UtcNow`. | `Components/Pages/Spots.razor`<br>`Components/Pages/Booking.razor` |
| **Fast Habits** | 15 pts | **Habit 1**: B-Tree indexes on `(name)`, `(region)`, `(category)`, and `(deleted_at)`.<br>**Habit 2**: Database-level filtering (`.Where()` before `.Get()`) and paging (`.Range(0, 49)`).<br>**Habit 3**: Single-roundtrip data retrieval with zero queries inside loops (avoiding N+1). | `database/schema.sql`<br>`Services/TourismDataService.cs` |
| **Storage Pattern & Reason** | 15 pts | Implements **Soft Delete** pattern (`deleted_at timestamptz`). Full 3–5 sentence rationale documented below. | `README.md`<br>`Models/Entities/DestinationBase.cs` |
| **Clean Code & UI** | 10 pts | Elegant luxury editorial styling, error handling, double-POST prevention, and graceful fallback when credentials are pending. | Entire project |

---

## 💾 Storage Pattern & Rationale (Rubric Milestone 5)

> **Why Soft Delete was chosen for the QAZAQ Tourism System:**  
> In the QAZAQ application, the `tourist_spots`, `bookings`, and `tourist_reviews` tables strictly utilize the **Soft Delete** pattern (`deleted_at timestamptz`) rather than permanent hard deletes. Tourist destinations are fundamentally linked to past travel reservations, financial receipts, and guest expedition testimonials; hard-deleting a destination row would cascade and destroy historical booking records or cause foreign key violations. By stamping `deleted_at = now()`, the destination is instantaneously hidden from public tourist discovery while retaining complete ledger integrity for accounting and customer support. Furthermore, this approach allows administrators to easily audit changes, review seasonal trends, or restore an archived destination with a single click without any data reconstruction.

---

## 🗄️ Database Architecture & Models

The database schema is organized in `database/schema.sql` and mirrored in C# model classes inheriting from `Supabase.Postgrest.Models.BaseModel`:

### 1. `tourist_spots` (`Models/Entities/TouristSpot.cs`)
- **Primary Key**: `id bigint generated always as identity primary key`
- **Columns**: `name`, `kazakh_name`, `region`, `category`, `short_description`, `full_description`, `image_url`, `base_price_usd`, `rating`, `review_count`, `best_season`, `highlights`, `difficulty`, `duration_days`, `altitude_meters`, `is_unesco_heritage`, `is_popular`
- **Audit / Pattern**: `deleted_at timestamptz`, `created_at timestamptz not null default now()`
- **Indexes**: `name`, `region`, `category`, `deleted_at`

### 2. `bookings` (`Models/Entities/TourBooking.cs`)
- **Primary Key**: `id bigint generated always as identity primary key`
- **Foreign Key**: `spot_id bigint references public.tourist_spots(id) on delete restrict`
- **Columns**: `full_name`, `trip_style`, `email`, `phone`, `travel_date`, `travelers`, `include_guide`, `include_yurt_experience`, `special_requests`, `total_price_usd`, `voucher_code`, `status`
- **Audit / Pattern**: `deleted_at timestamptz`, `created_at timestamptz not null default now()`

### 3. `tourist_reviews` (`Models/Entities/TouristReview.cs`)
- **Primary Key**: `id bigint generated always as identity primary key`
- **Foreign Key**: `spot_id bigint references public.tourist_spots(id) on delete set null`
- **Columns**: `destination_name`, `author_name`, `country`, `rating`, `travel_month`, `title`, `comment`, `image_url`, `helpful_votes`, `is_verified_guest`
- **Audit / Pattern**: `deleted_at timestamptz`, `created_at timestamptz not null default now()`

---

## 🚀 Supabase Setup Instructions (Milestone Steps 1–4)

### Step 1: Create Your Supabase Project (Slide 44)
1. Go to [supabase.com](https://supabase.com) and click **Start your project** (Sign in with GitHub).
2. Create an organization and choose the **Free** tier.
3. Click **New project** and name it `QAZAQ`.
4. Region: Choose **Southeast Asia (Singapore)** (closest server for lowest latency).
5. Generate a secure database password and save it in a safe place.

### Step 2: Create Tables & Seed Data (Slide 45)
1. In your Supabase Dashboard, open **SQL Editor** → **New query**.
2. Open the file [`database/schema.sql`](file:///c:/Users/L23Y16W19/Downloads/QAZAQ-main/database/schema.sql) in this repository.
3. Copy the entire contents, paste it into the SQL Editor, and click **Run**.
4. Check **Table Editor**: you will immediately see all 9 curated Kazakhstan destinations and initial reviews pre-populated!

### Step 3: Copy Your API Keys & Configure the App (Slide 46 & 47)
1. In Supabase, go to **Project Settings** → **API Keys**.
2. Copy your **Project URL** and your **Publishable key** (`sb_publishable_...` or `anon` public key).
   > **Note**: Never use or commit your secret `service_role` key!
3. Paste them into either your `.env` file or `appsettings.json`:

**Option A: In `.env`**
```env
SUPABASE_URL=https://your-project-id.supabase.co
SUPABASE_KEY=sb_publishable_your_key_here
```

**Option B: In `appsettings.json` and `wwwroot/appsettings.json`**
```json
{
  "Supabase": {
    "Url": "https://your-project-id.supabase.co",
    "Key": "sb_publishable_your_key_here"
  }
}
```

### Step 4: Quick REST Test (Slide 46)
You can test your Supabase API directly in VS Code / IDE using `test.http`:
```http
GET https://YOUR-REF.supabase.co/rest/v1/tourist_spots?select=*
apikey: YOUR_PUBLISHABLE_KEY
```

---

## 💻 Running the Application Locally

```bash
# 1. Restore .NET packages (includes Supabase 8.12.0)
dotnet restore

# 2. Build the CSS bundle (Tailwind CSS v4)
npm run build:css

# 3. Run the Blazor application
dotnet run
```

Navigate to `http://localhost:5029` in your browser.

- Visit `/spots` to view the live database catalog, add a new destination (Create), increase rates by $10 (Update), or remove a spot (Soft Delete).
- Check the **"Show Soft-Deleted / Archived"** checkbox to observe soft-deleted records and click **Restore**!
- Visit `/booking` to reserve a destination, generating a real row in `public.bookings`.
- Visit `/reviews` to read testimonials, submit new reviews, and vote helpful.

---

## 📸 Submission Checklist (Slide 52)

- [x] Repository includes `database/schema.sql` with full DDL, indexes, RLS, and seed data
- [x] README includes Storage Pattern explanation (Soft Delete) in 3–5 sentences
- [x] List page (`/spots`) loads rows from Supabase with Habit 2 filtering
- [x] Create, Update, and Delete all functional in the existing UI
- [x] No secret keys or database passwords committed (only publishable `anon` key)
- [ ] Two screenshots attached for submission:
  1. Front-end web page with active destinations (`/spots`)
  2. Supabase Table Editor showing the `tourist_spots` rows with `deleted_at` column
