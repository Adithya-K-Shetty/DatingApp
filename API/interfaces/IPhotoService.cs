using CloudinaryDotNet.Actions;

namespace API.interfaces
{
    public interface IPhotoService
    {
        //basically it creats two methods
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);//represent a file sent using http request
        Task<DeletionResult> DeletionResultAsync(string publicId);
    }
}