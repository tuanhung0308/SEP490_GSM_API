using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpha_API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class FeedbackController : ControllerBase
{
	private readonly FirebaseClient _firebaseClient;

	public FeedbackController()
	{
		_firebaseClient = new FirebaseClient("https://sgm-management-c98cd-default-rtdb.firebaseio.com/");
	}

	// GET: api/feedback
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedback()
	{
		var feedbacks = await _firebaseClient
			.Child("Feedback")
			.OnceAsync<Feedback>();

		var feedbackList = feedbacks.Select(f => f.Object).ToList();
		return Ok(feedbackList);
	}


	[Authorize(Policy = "StaffOnly")]
	// PUT: api/feedback/respond/{id}
	[HttpPut("respond/{id}")]
	public async Task<IActionResult> RespondToFeedback(string id, [FromBody] Feedback feedbackResponse)
	{
		var respondedBy = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
		// Retrieve the existing feedback
		var existingFeedback = await _firebaseClient
			.Child("Feedback")
			.Child(id)
			.OnceSingleAsync<Feedback>();

		// If feedback does not exist, return 404
		if (existingFeedback == null)
		{
			return NotFound(new { message = "Feedback not found" });
		}

		// Update the response fields
		existingFeedback.Response = feedbackResponse.Response;
		existingFeedback.RespondedAt = DateTime.UtcNow;
		existingFeedback.RespondedBy = respondedBy;

		// Save the updated feedback back to Firebase
		await _firebaseClient
			.Child("Feedback")
			.Child(id)
			.PutAsync(existingFeedback);

		return Ok(new { message = "Feedback responded successfully" });
	}

	[Authorize(Policy = "CustomerOnly")]
	// POST: api/feedback
	[HttpPost]
	public async Task<ActionResult<Feedback>> PostFeedback([FromBody] Feedback feedback)
	{
		var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
		feedback.SubmittedAt = DateTime.UtcNow;
		feedback.UserEmail = email;
		var result = await _firebaseClient
			.Child("Feedback")
			.PostAsync(feedback);
		feedback.FeedbackId = result.Key;
		return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.FeedbackId }, feedback);
	}

	// GET: api/feedback/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<Feedback>> GetFeedbackById(string id)
	{
		var feedback = await _firebaseClient
			.Child("Feedback")
			.Child(id)
			.OnceSingleAsync<Feedback>();

		if (feedback == null)
		{
			return NotFound();
		}

		return Ok(feedback);
	}

	// DELETE: api/feedback/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteFeedback(string id)
	{
		var existingFeedback = await _firebaseClient
			.Child("Feedback")
			.Child(id)
			.OnceSingleAsync<Feedback>();

		if (existingFeedback == null)
		{
			return NotFound();
		}

		await _firebaseClient
			.Child("Feedback")
			.Child(id)
			.DeleteAsync();

		return NoContent(); // 204 No Content
	}
}
