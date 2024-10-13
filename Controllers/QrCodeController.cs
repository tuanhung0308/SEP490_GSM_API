using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_API.Controllers
{
	[ApiController]
	[Route("api/qr")]
	public class QrCodeController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public QrCodeController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpPost("generate")]
		public async Task<IActionResult> GenerateQrCode([FromBody] QrCodeRequest request)
		{
			if (request == null)
			{
				return BadRequest("Request cannot be null.");
			}

			try
			{
				var client = _httpClientFactory.CreateClient();
				var apiUrl = "https://api.vietqr.io/v2/generate";

				// Prepare the request data


				var jsonData = new
				{
					accountNo = request.AccountNo,
					accountName = request.AccountName,
					acqId = request.AcqId,
					addInfo = request.AddInfo,
					amount = request.Amount,
					template = request.Template
				};


				//var jsonData = new
				//{
				//	accountNo = "0978788128",
				//	accountName = "DINH DAI DUONG",
				//	acqId = "970422",
				//	addInfo = "DK030303",
				//	amount = "500000",
				//	template = "compact"
				//};

				// Serialize request data to JSON
				var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
				var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

				// Add headers
				client.DefaultRequestHeaders.Add("x-client-id", "e27b68d6-aadc-44b8-bee3-9b77f39e9e0e");
				client.DefaultRequestHeaders.Add("x-api-key", "3163d46b-a727-4cc8-a841-fd8e0910dd57");

				// Send POST request to VietQR API
				HttpResponseMessage response = await client.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					var responseData = await response.Content.ReadAsStringAsync();
					var jsonResponse = JObject.Parse(responseData);

					// Extract qrDataURL from the response
					var qrDataUrl = jsonResponse["data"]?["qrDataURL"]?.ToString();

					if (!string.IsNullOrEmpty(qrDataUrl))
					{
						// Optionally, you can convert this Data URI to an image file or return it directly
						return Ok(new { qrCode = qrDataUrl }); // Return the Data URI
					}
					else
					{
						return BadRequest("QR data URL not found in response.");
					}
				}
				else
				{
					return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}

	public class QrCodeRequest
	{
		public string AccountNo { get; set; } = "0978788128";
		public string AccountName { get; set; } = "DINH DAI DUONG";
		public string AcqId { get; set; } = "970422";
		public string AddInfo { get; set; } = "DK030303";
		public string Amount { get; set; } = "500000";
		public string Template { get; set; } = "compact";
	}
}
