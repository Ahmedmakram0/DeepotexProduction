using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deepotex.EF.Repos
{
    public class PhotoServices
    {
        private readonly Cloudinary _cloudinary;

        public PhotoServices(string? cloudName, string? apiKey, string? apiSecret)
        {
            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
                throw new ArgumentNullException("Cloudinary configuration is missing.");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // Upload a single image (for backward compatibility)
        public async Task<string> UploadProductImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

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

        // Upload multiple images
        public async Task<List<string>> UploadProductImagesAsync(List<IFormFile> files)
        {
            if (files == null || !files.Any(f => f != null && f.Length > 0))
                return new List<string>();

            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                string url = await UploadProductImageAsync(file);
                if (!string.IsNullOrEmpty(url))
                    imageUrls.Add(url);
            }

            return imageUrls;
        }

        // Delete multiple images
        public async Task<bool> DeleteProductImagesAsync(List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any())
                return true;

            bool allDeleted = true;

            foreach (var imageUrl in imageUrls)
            {
                if (string.IsNullOrEmpty(imageUrl))
                    continue;

                string publicId = ExtractPublicIdFromUrl(imageUrl);

                if (string.IsNullOrEmpty(publicId))
                    continue;

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result != "ok")
                    allDeleted = false;
            }

            return allDeleted;
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            try
            {
                var uri = new Uri(url);
                var pathSegments = uri.AbsolutePath.Split('/');
                var fileName = pathSegments.LastOrDefault();
                if (string.IsNullOrEmpty(fileName))
                    return string.Empty;

                // Include folder in public ID
                return $"Deepotex/{Path.GetFileNameWithoutExtension(fileName)}";
            }
            catch (UriFormatException)
            {
                return string.Empty;
            }
        }
    }
}