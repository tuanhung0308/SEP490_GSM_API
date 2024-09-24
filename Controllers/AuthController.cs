using Alpha_API.Models;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _configuration;
	private readonly FirebaseClient _firebaseClient;

	public AuthController(IConfiguration configuration)
	{
		_configuration = configuration;

		// Initialize Firebase client (read/write permissions now open)
		_firebaseClient = new FirebaseClient("https://gym-management-e03f8-default-rtdb.asia-southeast1.firebasedatabase.app/");
	}

	[HttpPost("login")]
	public async Task<ActionResult> Login([FromBody] User logUser)
	{
		// Query Firebase Realtime Database for user information
		var userQuery = await _firebaseClient
			.Child("Users")
			.OrderBy("UserName")
			.EqualTo(logUser.UserName)
			.OnceAsync<User>();

		// If user exists and password matches
		if (userQuery.Any())
		{
			var existingUser = userQuery.First().Object;
			if (logUser.UserPassHashed == existingUser.UserPassHashed)
			{
				// Generate JWT token with the user's role
				var token = GenerateJwtToken(existingUser.UserName, existingUser.RoleId == 1 ? "admin" : (existingUser.RoleId == 2 ? "staff" : "customer"));

				var handler = new JwtSecurityTokenHandler();
				var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
				var role = jsonToken?.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
				Console.WriteLine($"Role: {role}");

				return Ok(new { token });
			}
		}

		// If no matching user or password is incorrect
		return Unauthorized();
	}

	private string GenerateJwtToken(string username, string role)
	{
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.Role, role)  // Include the user's role in the token
        };

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	[HttpPost("logout")]
	public IActionResult Logout()
	{
		return Ok(new { message = "Logged out successfully" });
	}
}
