using ContosoUniversity.Data;
using ContosoUniversity.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
var app = builder.Build();
using(var scope = app.Services.CreateScope()){
    var serviceProvider = scope.ServiceProvider;
    try{
        DbInitializer.Initialize(serviceProvider);
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");

app.MapRazorPages();


app.Run();
