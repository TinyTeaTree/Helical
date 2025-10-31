using System;
using Core;
using Newtonsoft.Json;

namespace Game
{
    public class PlayerAccountRecord : BaseRecord
    {
        public string PlayerId { get; set; }
        [JsonIgnore] public string SessionId { get; set; }
        public DateTime CreationDate { get; set; }

        public string NickName { get; set; }
        public AvatarId AvatarId { get; set; }
    }
}