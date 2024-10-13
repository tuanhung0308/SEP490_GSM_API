using Alpha_API.Models;
using Alpha_API.ViewModel;
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
		_firebaseClient = new FirebaseClient("https://sgm-management-c98cd-default-rtdb.firebaseio.com/");
	}

	[HttpPost("login")]
	public async Task<ActionResult> Login([FromBody] LogUser logUser)
	{
		// Query Firebase Realtime Database for user information
		var userQuery = await _firebaseClient
			.Child("Users")
			.OrderBy("Email")
			.EqualTo(logUser.Email)
			.OnceAsync<User>();

		// If user exists and password matches
		if (userQuery.Any())
		{
			var existingUser = userQuery.First().Object;
			if (logUser.Password == existingUser.Password)
			{
				// Retrieve role name from Firebase
				var roleName = await GetRoleNameFromFirebase(existingUser.RoleId);

				//// Generate JWT token with the user's role
				//var token = GenerateJwtToken(existingUser.UserName, existingUser.RoleId == 1 ? "admin" : (existingUser.RoleId == 2 ? "staff" : "customer"));

				// Generate the JWT token based on the role retrieved
				var token = GenerateJwtToken(existingUser.Email, roleName);

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

	// GET: api/Users/GetUserIdByEmail?email=user@example.com
	[HttpGet("GetUserIdByEmail")]
	public async Task<IActionResult> GetUserIdByEmail(string email)
	{
		try
		{
			// Use FirebaseAuth to get the user by email
			UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

			// Return the userId (Firebase UID)
			return Ok(new { userId = userRecord.Uid });
		}
		catch (FirebaseAuthException ex)
		{
			// Handle cases where the user is not found
			if (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
			{
				return NotFound("User with the specified email not found.");
			}

			// Handle other Firebase authentication errors
			return BadRequest(ex.Message);
		}
	}

	private string GenerateJwtToken(string email, string role)
	{
		var claims = new[]
		{
			//new Claim(JwtRegisteredClaimNames.Sub, email),

			 new Claim(ClaimTypes.Email, email),  // Use ClaimTypes.Email for email claim
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

	[HttpGet("role/{roleId}")]
	public async Task<string> GetRoleNameFromFirebase(string roleId)
	{
		// Directly access the role using the unique key (roleId)
		var roleQuery = await _firebaseClient
			.Child("Roles")
			.Child(roleId) // Use roleId directly as the key
			.OnceSingleAsync<Role>();

		// If the role is found, return its name; otherwise, default to "customer"
		return roleQuery?.RoleName ?? "customer"; // Default to "customer" if no match found
	}


	[HttpPost("logout")]
	public IActionResult Logout()
	{
		return Ok(new { message = "Logged out successfully" });
	}
}
