using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly Cloudinary _cloudinary;

    public ImagesController(IConfiguration config)
    {
        // Initialize Cloudinary with your appsettings credentials
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]);

        _cloudinary = new Cloudinary(account);
    }

    [HttpPost()]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No image provided.");

        // Read the incoming file stream
        using var stream = file.OpenReadStream();

        // Prepare the upload parameters for Cloudinary
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            // Optional: Automatically resize massive tablet photos to save space!
            Transformation = new Transformation()
            .Width(800)          // Set your perfect square size
            .Height(800)
            .Crop("fill")        // Fills the square, chopping off the long edges
            .Gravity("auto")     // AI keeps the main subject centered!
            .Quality("auto")     // Automatically compresses the file size
            .FetchFormat("auto") // Converts to WebP/modern formats for fast loading
        };

        // Send it to Cloudinary
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            return StatusCode(500, uploadResult.Error.Message);

        // Return the permanent internet URL!
        return Ok(uploadResult.SecureUrl.ToString() );
    }
}