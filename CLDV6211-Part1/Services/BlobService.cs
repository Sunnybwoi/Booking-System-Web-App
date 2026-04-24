using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CLDV6211_Part1.Services
    {
        public class BlobService : IBlobService
        {
            private readonly ILogger<BlobService>? _logger;
            private readonly string _connectionString;
            private readonly string _venueImagesContainerName;
            private readonly string _eventImagesContainerName;

            public BlobService(IConfiguration configuration, ILogger<BlobService> logger)
            {
                // Azurite local emulator connection string (can be overridden in user secrets)
                _connectionString = configuration.GetConnectionString("AzuriteStorage") ?? "UseDevelopmentStorage=true";
                _venueImagesContainerName = configuration["BlobSettings:VenueImagesContainer"]
                    ?? throw new InvalidOperationException("BlobSettings:VenueImagesContainer is not configured.");
                _eventImagesContainerName = configuration["BlobSettings:EventImagesContainer"]
                    ?? throw new InvalidOperationException("BlobSettings:EventImagesContainer is not configured.");
                _logger = logger;
            }

            private async Task<BlobContainerClient> GetContainerAsync()
            {
                return await GetContainerAsync(_venueImagesContainerName);
            }

            private async Task<BlobContainerClient> GetContainerAsync(string containerName)
            {
                try
                {
                    var client = new BlobServiceClient(_connectionString);
                    var container = client.GetBlobContainerClient(containerName);
                    await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
                    return container;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Unable to access blob storage container '{ContainerName}'", containerName);
                    throw new InvalidOperationException("Blob storage is unavailable. Ensure Azurite is running and the connection string is correct.", ex);
                }
            }

            public async Task<string> UploadImageAsync(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file provided.");

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                var contentType = file.ContentType ?? string.Empty;
                if (!allowedTypes.Contains(contentType.ToLower()))
                    throw new InvalidOperationException("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

                try
                {
                    var container = await GetContainerAsync();
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var blobClient = container.GetBlobClient(fileName);

                    using var stream = file.OpenReadStream();
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

                    return blobClient.Uri.ToString();
                }
                catch (InvalidOperationException)
                {
                    // rethrow friendly errors from GetContainerAsync
                    throw;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to upload image to blob storage");
                    throw new InvalidOperationException("Failed to upload image. Check blob storage availability and try again.", ex);
                }
            }

            // Upload image to the configured event images container.
            public async Task<string> UploadEventImageAsync(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file provided.");

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                var contentType = file.ContentType ?? string.Empty;
                if (!allowedTypes.Contains(contentType.ToLower()))
                    throw new InvalidOperationException("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

                try
                {
                    var container = await GetContainerAsync(_eventImagesContainerName);
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var blobClient = container.GetBlobClient(fileName);

                    using var stream = file.OpenReadStream();
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

                    return blobClient.Uri.ToString();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to upload event image to blob storage");
                    throw new InvalidOperationException("Failed to upload image. Check blob storage availability and try again.", ex);
                }
            }

            public async Task DeleteImageAsync(string imageUrl)
            {
                if (string.IsNullOrEmpty(imageUrl)) return;

                try
                {
                    var uri = new Uri(imageUrl);
                    var fileName = Path.GetFileName(uri.LocalPath);
                    var container = await GetContainerAsync();
                    var blobClient = container.GetBlobClient(fileName);
                    await blobClient.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    // Log and swallow delete errors to avoid crashing caller flows
                    _logger?.LogWarning(ex, "Failed to delete blob '{ImageUrl}'", imageUrl);
                }
            }

            // Delete an image from the configured event images container.
            public async Task DeleteEventImageAsync(string imageUrl)
            {
                if (string.IsNullOrEmpty(imageUrl)) return;

                try
                {
                    var uri = new Uri(imageUrl);
                    var fileName = Path.GetFileName(uri.LocalPath);
                    var container = await GetContainerAsync(_eventImagesContainerName);
                    var blobClient = container.GetBlobClient(fileName);
                    await blobClient.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to delete event blob '{ImageUrl}'", imageUrl);
                }
            }
        }
    }
