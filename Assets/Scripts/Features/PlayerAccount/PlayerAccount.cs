using System;
using System.Collections.Generic;
using System.Linq;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;

namespace Game
{
    public class PlayerAccount : BaseFeature, IPlayerAccount, IAppLaunchAgent
    {
        [Inject] public PlayerAccountRecord Record { get; set; }
        [Inject] public IPlayerSaveService Saver { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }
        [Inject] public ILogoutAgent LogoutAgent { get; set; }

        public bool IsLoggedIn => Record.PlayerId.HasContent();
        public string PlayerId => Record.PlayerId;

        private PlayerAccountConfig Config { get; set; }
        
        public UniTask AppLaunch()
        {
            Config = ConfigService.GetConfig<PlayerAccountConfig>();
            return UniTask.CompletedTask;
        }

        public async UniTask Login()
        {
            if (IsLoggedIn)
            {
                await Saver.SyncPlayerData();
                await Logout();
            }

            var savedPlayerAccount = await Saver.GetSavedJson(Record.Id);
            if (savedPlayerAccount.IsNullOrEmpty())
            {
                if (Config.CreateNewPlayerAutomatically)
                {
                    await CreateNewPlayer();
                    await Saver.SyncPlayerData();
                    return;
                }
                else
                {
                    //The caller must create User manually
                    return;
                }
            }
            else
            {
                Record.Populate(savedPlayerAccount);
            }

            if (Record.Version < PlayerAccountRecord.MigrationRecord)
            {
                //This means that a new version is available. Currently we dont have migrations, just restarting Player Data
                Notebook.NoteCritical($"Migration activated for player {Record.PlayerId}");
                
                await CreateNewPlayer();
                await Saver.SyncPlayerData();
                
                return;
            }

            var records = Saver.RecordsForSaving;
            foreach (var record in records)
            {
                if (record.Id == Record.Id)
                    continue; //This Record is already populated
                
                var saveJson = await Saver.GetSavedJson(record.Id);
                if(saveJson.IsNullOrEmpty())
                {
                    Notebook.NoteError($"Do not have any save for {record.Id}, maybe save was corrupted, or migration?");
                    continue;
                }
                record.Populate(saveJson);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f)); //Intentional delay to test loading
        }

        public UniTask Logout()
        {
            LogoutAgent.Logout();
        
            Record.Reset();
        
            return UniTask.CompletedTask;
        }

        public UniTask CreateNewPlayer()
        {
            Record.PlayerId = Guid.NewGuid().GetHashCode().ToString();
            
            Notebook.NoteCritical($"New User Created {Record.PlayerId}");

            var records = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Config.NewPlayerRecords.text);

            foreach (var recordKVP in records)
            {
                var recordToStart = Saver.RecordsForSaving.FirstOrDefault(r => r.Id == recordKVP.Key);
                if(recordToStart == null)
                {
                    Notebook.NoteError($"Record {recordKVP.Key} not present in the Records For Saving, Make sure you bootstrapped this record to have save support");
                    continue;
                }
                recordToStart.Populate(recordKVP.Value);
            }

            Record.Version = PlayerAccountRecord.MigrationRecord;

            return UniTask.CompletedTask;
        }
    }
}