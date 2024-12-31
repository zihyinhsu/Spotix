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



// �`�JDBContext�A���w�E���{�Ƕ�
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
builder.Services.AddIdentity<User, IdentityRole>(options =>
	{
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 8;
		options.Password.RequireUppercase = false;
		options.Password.RequireLowercase = false;
		options.User.RequireUniqueEmail = true; // ��ܥΤ���U�ɬO�_�ݭn�ϥΰߤ@���q�l�l��a�}�C
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

// �ϥΨ������ҪA��(�����b UseAuthorization ���e)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
