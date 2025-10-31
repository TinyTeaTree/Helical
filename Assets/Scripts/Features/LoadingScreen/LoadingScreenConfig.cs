using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class LoadingScreenConfig : BaseConfig
    {
        [System.Serializable]
        public class TipData
        {
            [TextArea] [SerializeField] private string _tip;

            public string Tip => _tip;
        }

        [System.Serializable]
        public class Data
        {
            [SerializeField] private string _assetPath;
            [SerializeField] private LoadingScreenType _type;
            [SerializeField] private bool _isLocal;

            public string AssetPath => _assetPath;
            public LoadingScreenType Type => _type;
            public bool IsLocal => _isLocal;
        }

        public List<TipData> Tips;
        public List<Data> Datas;
    }
}