using Microsoft.AspNetCore.Mvc;
using UserDetailsBackend.Models;
using UserDetailsBackend.Services; // Ensure this is here
using System.Threading.Tasks;

namespace UserDetailsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly GoogleSheetsService _googleSheetsService;

        // Constructor dependency injection for GoogleSheetsService
        public UserController(GoogleSheetsService googleSheetsService)
        {
            _googleSheetsService = googleSheetsService;
        }

        // POST api/user/add
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserDetails userDetails)
        {
            // Check if the provided user details are null
            if (userDetails == null)
            {
                return BadRequest("User details are required.");
            }

            // Ensure the values are not null before passing them to GoogleSheetsService
            if (string.IsNullOrEmpty(userDetails.Name) || string.IsNullOrEmpty(userDetails.Email) || string.IsNullOrEmpty(userDetails.Phone))
            {
                return BadRequest("All user details (Name, Email, Phone) are required.");
            }

            // Append user details to Google Sheets
            await _googleSheetsService.AppendUserDetailsAsync(userDetails.Name, userDetails.Email, userDetails.Phone);

            return Ok("User details added to Google Sheet.");
        }
    }
}
