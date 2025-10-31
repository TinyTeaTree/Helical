using System;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class PlayerAccount : BaseFeature, IPlayerAccount
    {
        [Inject] public IPlayerSaveService Saver { get; set; }

        [Inject] public PlayerAccountRecord Record { get; set; }

        public void CreateNewPlayer()
        {
            Record.PlayerId = System.Guid.NewGuid().ToString();
            Record.CreationDate = DateTime.UtcNow;

            Record.NickName = string.Empty;
            Record.AvatarId = AvatarId.Empty;
        }

        public UniTask LinkCredentials()
        {
            throw new System.NotImplementedException();
        }

        public async UniTask Login()
        {
            var savedJson = await Saver.GetSavedJson(Saves.PlayerAccount);
            if(savedJson.IsNullOrEmpty())
            {
                CreateNewPlayer();
                await SyncPlayerData();
            }
            else
            {
                Record.Populate(savedJson);
            }

            Record.SessionId = System.Guid.NewGuid().ToString();
        }

        public UniTask Logout()
        {
            throw new System.NotImplementedException();
        }

        public async UniTask SyncPlayerData()
        {
            await Saver.SaveData(Record, Saves.PlayerAccount);
        }
    }
}