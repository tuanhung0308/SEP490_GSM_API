using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Alpha_API.Models;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class RoleController : ControllerBase
{
	private readonly FirebaseClient _firebaseClient;

	public RoleController()
	{
		_firebaseClient = new FirebaseClient("https://sgm-management-c98cd-default-rtdb.firebaseio.com/");
	}

	// GET: api/roles
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
	{
		var roles = await _firebaseClient
			.Child("Roles")
			.OnceAsync<Role>();

		var roleList = roles.Select(r => r.Object).ToList(); // Flatten the roles

		return Ok(roleList);
	}

	// POST: api/roles
	[HttpPost]
	public async Task<ActionResult<Role>> PostRole([FromBody] Role role)
	{
		if (role == null)
		{
			return BadRequest();
		}

		// Retrieve existing roles and determine the next RoleId
		var roles = await _firebaseClient
			.Child("Roles")
			.OnceAsync<Role>();

		//int nextRoleId = roles.Count > 0 ? roles.Max(r => r.Object.RoleId) + 1 : 1;

		//// Assign the next available RoleId
		//role.RoleId = nextRoleId;

		// Save the role in the database without assigning to a variable
		var result = await _firebaseClient
			.Child("Roles")
			//.Child(nextRoleId.ToString()) // Using RoleId as the key
			.PostAsync(new { 
				role.RoleName
             });

		role.RoleId = result.Key;

		return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId }, role);
	}


	// GET: api/roles/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<Role>> GetRoleById(string id)
	{
		var role = await _firebaseClient
			.Child("Roles")
			.Child(id.ToString())
			.OnceSingleAsync<Role>();

		if (role == null)
		{
			return NotFound();
		}

		return Ok(role);
	}

	// PUT: api/roles/{id}
	[HttpPut("{id}")]
	public async Task<ActionResult> PutRole(string id, [FromBody] Role role)
	{
		var existingRole = await _firebaseClient
			.Child("Roles")
			.Child(id.ToString())
			.OnceSingleAsync<Role>();

		if (existingRole == null)
		{
			return NotFound();
		}

		// Update the role
		await _firebaseClient
			.Child("Roles")
			.Child(id.ToString())
			.PutAsync(role);

		return NoContent();
	}

	// DELETE: api/roles/{id}
	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteRole(string id)
	{
		var existingRole = await _firebaseClient
			.Child("Roles")
			.Child(id)
			.OnceSingleAsync<Role>();

		if (existingRole == null)
		{
			return NotFound();
		}

		await _firebaseClient
			.Child("Roles")
			.Child(id)
			.DeleteAsync();

		return NoContent();
	}
}
