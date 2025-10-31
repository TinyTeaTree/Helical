using UnityEngine;

namespace ChessRaid
{
    [CreateAssetMenu(fileName = "Hex", menuName = "Chess Raid/Definitions/Hex")]
    public class HexDef : ScriptableObject
    {
        public Color SelectedColor;
        public Color NotSelectedColor;

        public Color AttackColor;
        public Color MoveColor;
        public Color RotateColor;
    }
}