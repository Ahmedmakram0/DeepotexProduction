using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Deepotex.core.Repositories;
using Microsoft.AspNetCore.Http;


namespace Deepotex.EF.Repos;
public class PhotoServices 
{
    private readonly Cloudinary _cloudinary;

    public PhotoServices(string? cloudName, string? apiKey, string? apiSecret)
    {
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadProductImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return "No Image Uploaded";

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "Deepotex", 
            UseFilename = true,
            UniqueFilename = true,
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        return uploadResult.SecureUrl.ToString();
    }


    public async Task<bool> DeleteProductImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return false;

        // Extract public ID from URL
        string publicId = ExtractPublicIdFromUrl(imageUrl);

        if (string.IsNullOrEmpty(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok";
    }

    private string ExtractPublicIdFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return "Unvalid Url";

        try
        {
            var uri = new Uri(url);
            var pathSegments = uri.AbsolutePath.Split('/');
            return pathSegments.LastOrDefault() ?? "Invalid Url";
        }
        catch (UriFormatException)
        {
            return "Invalid Url Format";
        }
    }

}
