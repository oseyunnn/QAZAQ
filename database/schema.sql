-- ============================================================================
-- QAZAQ TOURISM APPLICATION - SUPABASE DATABASE SCHEMA
-- Course: Application Development (CIT-U Computer Studies Department)
-- Instructor: John Quinnvic G. Taboada
-- Milestone: Connect Your Front End to Supabase (Pages 43-52)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 1. TABLE: tourist_spots (Main Destinations Model)
-- ----------------------------------------------------------------------------
create table if not exists public.tourist_spots (
    id bigint generated always as identity primary key,
    name text not null,
    kazakh_name text not null,
    region text not null,
    category text not null,
    short_description text not null,
    full_description text not null default '',
    image_url text not null,
    base_price_usd numeric(10,2) not null default 0 check (base_price_usd >= 0),
    rating numeric(3,2) not null default 5.0 check (rating >= 0 and rating <= 5),
    review_count integer not null default 0 check (review_count >= 0),
    best_season text not null default 'May to October',
    highlights text[] not null default '{}',
    difficulty text not null default 'Easy',
    duration_days integer not null default 1 check (duration_days >= 1),
    altitude_meters integer not null default 0,
    is_unesco_heritage boolean not null default false,
    is_popular boolean not null default false,
    deleted_at timestamptz,                              -- Step 5 & 8: soft delete column
    created_at timestamptz not null default now()
);

-- Fast Habit 1: Find rows quickly with an index (Slide 18, 45)
create index if not exists idx_tourist_spots_name on public.tourist_spots (name);
create index if not exists idx_tourist_spots_region on public.tourist_spots (region);
create index if not exists idx_tourist_spots_category on public.tourist_spots (category);
create index if not exists idx_tourist_spots_deleted_at on public.tourist_spots (deleted_at);

-- Lock 1: GRANT permissions to anon & authenticated roles (Slide 39, 45)
grant select, insert, update, delete on public.tourist_spots to anon, authenticated;

-- Lock 2: Row Level Security (RLS) with open policies for class project (Slide 39, 45)
alter table public.tourist_spots enable row level security;

drop policy if exists "read_all_spots" on public.tourist_spots;
drop policy if exists "insert_all_spots" on public.tourist_spots;
drop policy if exists "update_all_spots" on public.tourist_spots;
drop policy if exists "delete_all_spots" on public.tourist_spots;

create policy "read_all_spots" on public.tourist_spots for select to anon, authenticated using (true);
create policy "insert_all_spots" on public.tourist_spots for insert to anon, authenticated with check (true);
create policy "update_all_spots" on public.tourist_spots for update to anon, authenticated using (true) with check (true);
create policy "delete_all_spots" on public.tourist_spots for delete to anon, authenticated using (true);


-- ----------------------------------------------------------------------------
-- 2. TABLE: bookings (Tour Expedition Reservations Model)
-- Normalized with foreign key referencing tourist_spots(id) (Slide 14, 15)
-- ----------------------------------------------------------------------------
create table if not exists public.bookings (
    id bigint generated always as identity primary key,
    spot_id bigint not null references public.tourist_spots(id) on delete restrict,
    full_name text not null,
    trip_style text not null,
    email text not null,
    phone text not null,
    travel_date timestamptz not null,
    travelers integer not null default 2 check (travelers >= 1 and travelers <= 20),
    include_guide boolean not null default true,
    include_yurt_experience boolean not null default true,
    special_requests text,
    total_price_usd numeric(10,2) not null default 0 check (total_price_usd >= 0),
    voucher_code text not null unique,
    status text not null default 'Confirmed',
    deleted_at timestamptz,                              -- Soft delete pattern for booking cancellations
    created_at timestamptz not null default now()
);

-- Fast Habit 1: Indexes for foreign keys and frequent query lookups
create index if not exists idx_bookings_spot_id on public.bookings (spot_id);
create index if not exists idx_bookings_email on public.bookings (email);
create index if not exists idx_bookings_voucher_code on public.bookings (voucher_code);
create index if not exists idx_bookings_deleted_at on public.bookings (deleted_at);

-- Lock 1 & 2: GRANT + RLS
grant select, insert, update, delete on public.bookings to anon, authenticated;
alter table public.bookings enable row level security;

drop policy if exists "read_all_bookings" on public.bookings;
drop policy if exists "insert_all_bookings" on public.bookings;
drop policy if exists "update_all_bookings" on public.bookings;
drop policy if exists "delete_all_bookings" on public.bookings;

create policy "read_all_bookings" on public.bookings for select to anon, authenticated using (true);
create policy "insert_all_bookings" on public.bookings for insert to anon, authenticated with check (true);
create policy "update_all_bookings" on public.bookings for update to anon, authenticated using (true) with check (true);
create policy "delete_all_bookings" on public.bookings for delete to anon, authenticated using (true);


-- ----------------------------------------------------------------------------
-- 3. TABLE: tourist_reviews (Expedition Testimonials Model)
-- ----------------------------------------------------------------------------
create table if not exists public.tourist_reviews (
    id bigint generated always as identity primary key,
    spot_id bigint references public.tourist_spots(id) on delete set null,
    destination_name text not null,
    author_name text not null,
    country text not null default 'International',
    rating integer not null default 5 check (rating >= 1 and rating <= 5),
    travel_month text not null,
    title text not null,
    comment text not null,
    image_url text not null default '',
    helpful_votes integer not null default 0 check (helpful_votes >= 0),
    is_verified_guest boolean not null default true,
    deleted_at timestamptz,                              -- Soft delete pattern
    created_at timestamptz not null default now()
);

-- Fast Habit 1: Indexes
create index if not exists idx_tourist_reviews_spot_id on public.tourist_reviews (spot_id);
create index if not exists idx_tourist_reviews_rating on public.tourist_reviews (rating);
create index if not exists idx_tourist_reviews_deleted_at on public.tourist_reviews (deleted_at);

-- Lock 1 & 2: GRANT + RLS
grant select, insert, update, delete on public.tourist_reviews to anon, authenticated;
alter table public.tourist_reviews enable row level security;

drop policy if exists "read_all_reviews" on public.tourist_reviews;
drop policy if exists "insert_all_reviews" on public.tourist_reviews;
drop policy if exists "update_all_reviews" on public.tourist_reviews;
drop policy if exists "delete_all_reviews" on public.tourist_reviews;

create policy "read_all_reviews" on public.tourist_reviews for select to anon, authenticated using (true);
create policy "insert_all_reviews" on public.tourist_reviews for insert to anon, authenticated with check (true);
create policy "update_all_reviews" on public.tourist_reviews for update to anon, authenticated using (true) with check (true);
create policy "delete_all_reviews" on public.tourist_reviews for delete to anon, authenticated using (true);


