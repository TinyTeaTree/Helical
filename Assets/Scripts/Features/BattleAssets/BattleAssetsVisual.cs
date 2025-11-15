using Core;
using TMPro;
using UnityEngine;

namespace Game
{
    public class BattleAssetsVisual : BaseVisual<BattleAssetsFeature>
    {
        //TODO: API To turn this on and off
        [SerializeField] private GameObject _currencyBarRoot;

        //TOD: this needs to be the gold amount
        [SerializeField] private TMP_Text _goldAmount;
    }
}