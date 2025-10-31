using UnityEngine;

namespace ChessRaid
{
    public class RulesBox : DataBox
    {
        public const string BoxId = "Rules";
        public override string Id => BoxId;

        public RulesDef RulesSO { get; private set; }

        public override void Initialize()
        {
            RulesSO = Resources.Load<RulesDef>("Rules/Basic Rules");
        }
    }
}