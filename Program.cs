using ContosoUniversity.Authorization;
using ContosoUniversity.Data;
using ContosoUniversity.Hubs;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

var supportedCultures = new[] { "en-US", "vi-VN" }; 
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"), 
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToArray(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToArray()
};

builder.Services.AddSingleton(localizationOptions);
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});



builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});




builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddSignalR();

builder.Services.AddIdentity<ContosoUser, IdentityRole>()
    .AddEntityFrameworkStores<SchoolContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


// builder.Services.AddIdentity<ContosoUser, IdentityRole>()
//     .AddDefaultTokenProviders()
//     .AddUserStore<JsonUserStore>() 
//     .AddRoleStore<JsonRoleStore>()
//     .AddDefaultUI(); 

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    }).AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
        facebookOptions.AccessDeniedPath = "/Identity/Account/Login";
    }).AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
    });

// Authorization handlers.
builder.Services.AddScoped<IAuthorizationHandler,
                        IsOwerAuthorizationHandler<AuthorizationPropertyProvider>>();

builder.Services.AddSingleton<IAuthorizationHandler,
                        AdministratorAuthorizationHandler<Object>>();


builder.Services.AddSingleton<IAuthorizationHandler,
                        ManagerAuthorizationHandler<AuthorizationPropertyProvider>>();
var app = builder.Build();

app.UseRequestLocalization(localizationOptions);
using (var scope = app.Services.CreateScope()){
    var serviceProvider = scope.ServiceProvider;
    try{
        var context = serviceProvider.GetRequiredService<SchoolContext>();
        context.Database.Migrate();
        var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");
        await DbInitializer.Initialize(serviceProvider,testUserPw!);
    }catch(Exception ex){
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
        
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");

app.MapRazorPages();


app.Run();
