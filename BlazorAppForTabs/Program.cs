using BlazorAppForTabs.Components;
using BlazorAppForTabs.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5041/"); // ???? ??? ?? ????? ?-API ???
});

// Add cookie-based authentication
//builder.Services.AddAuthentication("Cookies")
//    .AddCookie("Cookies", options =>
//    {
//        options.LoginPath = "/signin";              // Page shown to users who aren't logged in
//        options.AccessDeniedPath = "/unauthorized"; // Optional: for users with wrong role
//    });


//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
//    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
//    options.AddPolicy("GuestOnly", policy => policy.RequireRole("Guest"));
//});

// Default HttpClient injection
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication(); // authenticate user via cookies
//app.UseAuthorization();  // enforce [Authorize] checks

app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


