using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Alpha_API.Models;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly FirebaseClient _firebaseClient;

	public UsersController()
	{
		_firebaseClient = new FirebaseClient("https://gym-management-e03f8-default-rtdb.asia-southeast1.firebasedatabase.app/");
	}

	// GET: api/users
	[Authorize(Policy = "AdminOnly")]
	[HttpGet]
	public async Task<ActionResult<IEnumerable<User>>> GetUsers()
	{
		var users = await _firebaseClient
			.Child("Users")
			.OnceAsync<User>();

		var userList = new List<User>();
		foreach (var user in users)
		{
			userList.Add(user.Object);
		}

		return Ok(userList);
	}

	// POST: api/users
	[HttpPost]
	public async Task<ActionResult<User>> PostUser([FromBody] User user)
	{
		if (user == null)
		{
			return BadRequest();
		}

		var result = await _firebaseClient
			.Child("Users")
		.PostAsync(new
		{
			user.UserAvatar,
			user.UserImage,
			user.UserFirstLogin,
			user.UserIsEnabled,
			user.UserPhone, 
			user.UserName,
			user.UserPassHashed,
			user.UserEmail,
			user.RoleId,
			user.Address,
		});

		user.UserId = result.Key; // Firebase generates a unique key
		return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
	}

	// GET: api/users/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<User>> GetUserById(string id)
	{
		var user = await _firebaseClient
			.Child("Users")
			.Child(id)
			.OnceSingleAsync<User>();

		if (user == null)
		{
			return NotFound();
		}

		return Ok(user);
	}

	// PUT: api/users/{id}
	[HttpPut("{id}")]
	public async Task<ActionResult> PutUser(string id, [FromBody] User user)
	{
		if (id != user.UserId)
		{
			return BadRequest();
		}

		await _firebaseClient
			.Child("Users")
			.Child(id)
			.PutAsync(user);

		return NoContent(); // 204 No Content
	}

	// PATCH: api/users/{id}
	[HttpPatch("{id}")]
	public async Task<ActionResult> PatchUser(string id, [FromBody] User user)
	{
		var existingUser = await _firebaseClient
			.Child("Users")
			.Child(id)
			.OnceSingleAsync<User>();

		if (existingUser == null)
		{
			return NotFound();
		}

		// Update fields selectively
		if (!string.IsNullOrEmpty(user.UserName))
		{
			existingUser.UserName = user.UserName;
		}
		if (!string.IsNullOrEmpty(user.UserEmail))
		{
			existingUser.UserEmail = user.UserEmail;
		}

		await _firebaseClient
			.Child("Users")
			.Child(id)
			.PutAsync(existingUser);

		return NoContent(); // 204 No Content
	}

	// DELETE: api/users/{id}
	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteUser(string id)
	{
		var existingUser = await _firebaseClient
			.Child("Users")
			.Child(id)
			.OnceSingleAsync<User>();

		if (existingUser == null)
		{
			return NotFound();
		}

		await _firebaseClient
			.Child("Users")
			.Child(id)
			.DeleteAsync();

		return NoContent(); // 204 No Content
	}
}
