using Core;

namespace Game
{
    public class TurnRecord : BaseRecord
    {
        public bool InTurn { get; set; }
        public int Turn { get; set; }
        public int UnitsExecuting { get; set; }
    }
}