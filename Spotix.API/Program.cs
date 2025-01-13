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

	// �b Swagger ���K�[ JWT ��������
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



// �]�w CORS
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

// �`�JDBContext�A���w�E���{�Ƕ�
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
		ValidateIssuer = true, // ���ܬO�_�����ҥO�P���o���
		ValidateAudience = true,// ���ܬO�_�����ҥO�P��������
		ValidateLifetime = true, // ���ܬO�_�����ҥO�P�����Ĵ�
		ValidateIssuerSigningKey = true, // ���ܬO�_�����ҥO�P��ñ�W�K�_
		ValidIssuer = builder.Configuration["Jwt:Issuer"],// ���w���H�����o���
		ValidAudience = builder.Configuration["Jwt:Audience"],// ���w���H����������
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // ���w�Ω����ҥO�Pñ�W���K�_�C
	});


// Add Identity
builder.Services.AddIdentityCore<User>(options =>
	{
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 8;
		options.Password.RequireUppercase = false;
		options.Password.RequireLowercase = false;
		options.User.RequireUniqueEmail = true; // ��ܥΤ���U�ɬO�_�ݭn�ϥΰߤ@���q�l�l��a�}�C
		options.SignIn.RequireConfirmedAccount = false; 
		options.SignIn.RequireConfirmedEmail = false;
		options.SignIn.RequireConfirmedPhoneNumber = false;
		options.User.AllowedUserNameCharacters = null;

	})
	.AddRoles<IdentityRole>()
	.AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();


// �]�w SQL Server �֨�
builder.Services.AddDistributedSqlServerCache(options =>
{
	options.ConnectionString = builder.Configuration.GetConnectionString("ConnectionString"); // SQL Server �s�u�r��
	options.SchemaName = "dbo"; // ��ƪ� Schema
	options.TableName = "OrderCache"; // ��ƪ�W��
});
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();


// �ϥ� CORS ������
app.UseCors("AllowSpecificOrigins");

// �ϥΨ������ҪA��(�����b UseAuthorization ���e)
app.UseAuthentication();
app.UseAuthorization();

// ���U������
app.UseMiddleware<ResponseHandleMiddleware>();

app.MapControllers();

app.Run();

