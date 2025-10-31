using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

public interface IPlayerSaveService : IService
{
    void AddSaveRecord(BaseRecord record);
    List<BaseRecord> RecordsForSaving { get; }
        
    UniTask<T> GetSavedData<T>(string saveId);

    UniTask<string> GetSavedJson(string saveId);

    UniTask SaveData<T>(T save, string saveId);

    UniTask SyncPlayerData();
}