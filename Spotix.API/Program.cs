using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Spotix.API.Middlewares;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Repositories;
using System.Text;
using Spotix.API.Mappings;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.NewebPay;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{

	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Spotix API",
		Version = "v1"
	});

	// 在 Swagger 中添加 JWT 身份驗證
	options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
	{
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme{
			Reference = new OpenApiReference{
				Type = ReferenceType.SecurityScheme,
				Id = JwtBearerDefaults.AuthenticationScheme
			},
			Scheme = "Oauth2",
			Name = JwtBearerDefaults.AuthenticationScheme,
			In = ParameterLocation.Header
		},
			new List<string>()
		}
	});
});



// 設定 CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigins",
		builder =>
		{
			builder.WithOrigins("http://localhost:3000", "https://spo-tix.vercel.app")
				   .AllowAnyHeader()
				   .AllowAnyMethod();
		});
});

// 注入DBContext，指定遷移程序集
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"),
		b => b.MigrationsAssembly("Spotix.API")));


// DI
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AreaService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<LineBotService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	options.TokenValidationParameters = new TokenValidationParameters
	{
		AuthenticationType = "Jwt",
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
builder.Services.AddIdentityCore<User>(options =>
	{
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 8;
		options.Password.RequireUppercase = false;
		options.Password.RequireLowercase = false;
		options.User.RequireUniqueEmail = true; // 表示用戶註冊時是否需要使用唯一的電子郵件地址。
		options.SignIn.RequireConfirmedAccount = false; 
		options.SignIn.RequireConfirmedEmail = false;
		options.SignIn.RequireConfirmedPhoneNumber = false;
		options.User.AllowedUserNameCharacters = null;

	})
	.AddRoles<IdentityRole>()
	.AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();


// 設定 SQL Server 快取
builder.Services.AddDistributedSqlServerCache(options =>
{
	options.ConnectionString = builder.Configuration.GetConnectionString("ConnectionString"); // SQL Server 連線字串
	options.SchemaName = "dbo"; // 資料表的 Schema
	options.TableName = "OrderCache"; // 資料表名稱
});
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();


// 使用 CORS 中間件
app.UseCors("AllowSpecificOrigins");

// 使用身分驗證服務(必須在 UseAuthorization 之前)
app.UseAuthentication();
app.UseAuthorization();

// 註冊中間件
app.UseMiddleware<ResponseHandleMiddleware>();

app.MapControllers();

app.Run();

