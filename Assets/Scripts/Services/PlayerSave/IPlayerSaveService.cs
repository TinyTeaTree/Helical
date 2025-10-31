using System.Threading.Tasks;
using Core;

namespace Services
{
    public interface IPlayerSaveService : IService
    {
        Task<T> GetSavedData<T>(string saveId);

        Task<string> GetSavedJson(string saveId);

        Task SaveData<T>(T save, string saveId);
    }
}