namespace Deepotex_V2.Utilities;

public static class ImageHelper
{
    public static string GetRandomBackgroundImage(IWebHostEnvironment env)
    {
        // Get the path to the wwwroot/images folder
        string imagesFolder = Path.Combine(env.WebRootPath, "images");

        // List all .jpg files in the images folder (you can adjust for .png, etc.)
        string[] imageFiles = Directory.GetFiles(imagesFolder, "*.jpg");

        if (imageFiles.Length == 0)
        {
            return "/images/homeimg.jpg"; // Fallback if no images are found
        }

        // Use a random number to pick an image
        Random random = new Random();
        int randomIndex = random.Next(0, imageFiles.Length);

        // Convert the file path to a URL (relative to wwwroot)
        string filePath = imageFiles[randomIndex];
        string fileName = Path.GetFileName(filePath);
        return $"/images/{fileName}";
    }
}
