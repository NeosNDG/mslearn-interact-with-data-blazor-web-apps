using BlazingPizza.Data;
using BlazingPizza.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Added by me: HTTP Client so that we can make HTTP requests (to the "specials" Controller API)
builder.Services.AddHttpClient();
// Added by me: Sqlite with the PizzaStoreContext vis Microsoft.EntityFrameworkCore so that we can use a database (create if not exists)
builder.Services.AddSqlite<PizzaStoreContext>("Data Source=pizza.db");

// My Services Class added to the container as a service so that we can use it in our Blazor components (e.g. Index.razor)
builder.Services.AddScoped<OrderState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

// Initialize the database via Data/SeedData.cs
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
    if (db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
}

app.Run();
