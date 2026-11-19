using QAZAQ.Components;
using QAZAQ.Services;

// Load .env file if present in the workspace root
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;
        var parts = trimmed.Split('=', 2);
        if (parts.Length == 2)
        {
            var k = parts[0].Trim();
            var v = parts[1].Trim().Trim('"', '\'');
            Environment.SetEnvironmentVariable(k, v);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Supabase registration (Milestone Step 4)
var url = builder.Configuration["Supabase:Url"]
       ?? builder.Configuration["SUPABASE_URL"]
       ?? Environment.GetEnvironmentVariable("SUPABASE_URL")
       ?? "https://bgrsqjcsjdukhiskycvj.supabase.co";

var key = builder.Configuration["Supabase:Key"]
       ?? builder.Configuration["Supabase:PublishableKey"]
       ?? builder.Configuration["SUPABASE_PUBLISHABLE_KEY"]
       ?? builder.Configuration["SUPABASE_KEY"]
       ?? Environment.GetEnvironmentVariable("SUPABASE_PUBLISHABLE_KEY")
       ?? Environment.GetEnvironmentVariable("SUPABASE_KEY")
       ?? "sb_publishable_CvqI1EjDA46mdWc5hvYxGg_TK2FHKnS";

var supabase = new Supabase.Client(url, key,
    new Supabase.SupabaseOptions { AutoConnectRealtime = false });
await supabase.InitializeAsync();
builder.Services.AddSingleton(supabase);
builder.Services.AddSingleton<ITourismDataService, TourismDataService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

