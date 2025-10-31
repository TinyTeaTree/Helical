using Core;

namespace Game
{
    /*
     *
     * Version 0 - before migration system
     *
     * 
     */
    public class PlayerAccountRecord : BaseRecord
    {
        public static readonly int MigrationRecord = 0; //Increase this number if you want to Reset all the Users via a Migration Process
        //And document the reason for increment up above.
        
        public string PlayerId { get; set; }
        public string Nickname { get; set; } //TODO : Ask for a Nickname

        public void Reset()  //TODO: Add a Test that Resets makes this record the same as a New() record
        {
            PlayerId = null;
        }
    }
}