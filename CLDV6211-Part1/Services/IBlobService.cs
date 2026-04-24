using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CLDV6211_Part1.Services
{
    public interface IBlobService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadEventImageAsync(IFormFile file);
        Task DeleteImageAsync(string imageUrl);
        Task DeleteEventImageAsync(string imageUrl);
    }
}
