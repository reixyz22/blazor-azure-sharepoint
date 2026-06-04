using BlazorApp1.Components;

var builder = WebApplication.CreateBuilder(args);

// Allow Blazor Server to work inside cross-origin iframes (SharePoint)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Allow SharePoint to embed this app in an iframe
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "frame-ancestors 'self' https://*.sharepoint.com";
    // Remove X-Frame-Options — it overrides CSP frame-ancestors in some browsers
    context.Response.Headers.Remove("X-Frame-Options");
    await next();
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();