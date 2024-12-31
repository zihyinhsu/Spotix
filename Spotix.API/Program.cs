using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spotix.Utilities.Interfaces;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// 注入DBContext，指定遷移程序集
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"),
		b => b.MigrationsAssembly("Spotix.API")));


// DI
builder.Services.AddScoped<ITokenRepository, TokenRepository>();


// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true, // 指示是否應驗證令牌的發行者
		ValidateAudience = true,// 指示是否應驗證令牌的接收方
		ValidateLifetime = true, // 指示是否應驗證令牌的有效期
		ValidateIssuerSigningKey = true, // 指示是否應驗證令牌的簽名密鑰
		ValidIssuer = builder.Configuration["Jwt:Issuer"],// 指定受信任的發行者
		ValidAudience = builder.Configuration["Jwt:Audience"],// 指定受信任的接收方
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // 指定用於驗證令牌簽名的密鑰。
	});

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
	{
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 8;
		options.Password.RequireUppercase = false;
		options.Password.RequireLowercase = false;
		options.User.RequireUniqueEmail = true; // 表示用戶註冊時是否需要使用唯一的電子郵件地址。
		options.SignIn.RequireConfirmedAccount = false; 
		options.SignIn.RequireConfirmedEmail = false;
		options.SignIn.RequireConfirmedPhoneNumber = false;
	})
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 使用身分驗證服務(必須在 UseAuthorization 之前)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
