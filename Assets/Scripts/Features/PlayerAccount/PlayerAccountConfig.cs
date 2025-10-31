using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [System.Serializable]
    public class PlayerAccountConfig : BaseConfig
    {
        public bool CreateNewPlayerAutomatically;

        public TextAsset NewPlayerRecords;
    }
}