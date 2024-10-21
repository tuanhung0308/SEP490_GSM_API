using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
namespace WebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();

			var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


			builder.Services.AddCors(opts =>
			{
				opts.AddPolicy("CORSPolicy", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true));
			});


			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});



			//builder.Services.AddAuthorization(options =>
			//{
			//	options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));
			//	options.AddPolicy("StaffOnly", policy => policy.RequireClaim(ClaimTypes.Role, "staff"));
			//	options.AddPolicy("PTOnly", policy => policy.RequireClaim(ClaimTypes.Role, "pt"));
			//	options.AddPolicy("CustomerOnly", policy => policy.RequireClaim(ClaimTypes.Role, "customer"));
			//});

			FirebaseApp.Create(new AppOptions()
			{
				Credential = GoogleCredential.FromFile("D:\\Downloads\\Fall24\\SEP490\\Secret\\sgm-management-c98cd-firebase-adminsdk-kc3zt-383493a9bd.json")
			});
			builder.WebHost.UseUrls("http://0.0.0.0:5000");
			builder.Services.AddHttpClient<AuthController>();

			// Add session services
			builder.Services.AddDistributedMemoryCache(); // You can also use other types of caches
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
				options.Cookie.HttpOnly = true; // Make the session cookie accessible only to the server
				options.Cookie.IsEssential = true; // Ensure session cookie is always available
			});

			var app = builder.Build();

			app.UseHttpsRedirection();
			app.UseRouting();	
			app.UseCors("CORSPolicy");
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseSession();
			app.MapControllers();
			app.UseODataBatching();



			app.Run();
		}
	}
}