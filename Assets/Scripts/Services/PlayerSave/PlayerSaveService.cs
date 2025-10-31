using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Services
{
    public class PlayerSaveService : BaseService, IPlayerSaveService
    {
        private string FolderPath => Application.isEditor ? Application.dataPath : Application.persistentDataPath;
        public List<BaseRecord> RecordsForSaving { get; } = new();
        
        public void AddSaveRecord(BaseRecord record)
        {
            RecordsForSaving.Add(record);
        }

        public async UniTask<T> GetSavedData<T>(string saveId)
        {
            var saveText = await GetSavedJson(saveId);
            if(saveText.IsNullOrEmpty())
            {
                return default;
            }

            var saveObject = JsonConvert.DeserializeObject<T>(saveText);
            return saveObject;
        }
        
        public async UniTask SyncPlayerData()
        {
            var records = RecordsForSaving;
            foreach (var record in records)
            {
                await SaveData(record, record.Id);
            }
        }

        public UniTask<string> GetSavedJson(string saveId)
        {
            var filePath = Path.Combine(FolderPath, "Player Data", saveId + ".json");
            if (!File.Exists(filePath))
            {
                return UniTask.FromResult(string.Empty);
            }
            else
            {
                var text = File.ReadAllText(filePath);
                return UniTask.FromResult(text);
            }
        }

        public UniTask SaveData<T>(T save, string saveId)
        {
            var saveText = JsonConvert.SerializeObject(save, Formatting.Indented);

            var userDataFolderPath = Path.Combine(FolderPath, "Player Data");
            if (!Directory.Exists(userDataFolderPath))
            {
                Directory.CreateDirectory(userDataFolderPath);
            }

            var filePath = Path.Combine(userDataFolderPath, saveId + ".json");
            File.WriteAllText(filePath, saveText);
            
            return UniTask.CompletedTask;
        }
    }
}