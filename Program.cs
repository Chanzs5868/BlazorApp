using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // 生产环境(Railway):用 PostgreSQL
    // Railway 给的 DATABASE_URL 格式是 postgres://user:pass@host:port/dbname
    // Npgsql 需要转换成标准连接字符串格式
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var npgsqlConnectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(npgsqlConnectionString));
}
else
{
    // 本地开发环境:继续用 SQLite
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=app.db"));
}

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://my-app-chanzs5869.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

const string secretKey = "this-is-a-demo-secret-key-please-change-in-production-12345";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // 生产环境(PostgreSQL):用正式迁移
        db.Database.Migrate();
    }
    else
    {
        // 本地开发(SQLite):直接根据模型建表,不走迁移历史
        db.Database.EnsureCreated();
    }
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var targetUser = db.Users.FirstOrDefault(u => u.Username == "ADMIN123");
    if (targetUser is not null && !targetUser.IsAdmin)
    {
        targetUser.IsAdmin = true;
        db.SaveChanges();
        Console.WriteLine($"✅ Set '{targetUser.Username}' as admin.");
    }
    else if (targetUser is null)
    {
        Console.WriteLine("⚠️ User not found. Check the username.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors("AllowNextJs");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();