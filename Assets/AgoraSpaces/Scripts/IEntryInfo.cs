using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agora.Spaces
{
    public interface IEntryInfo
    {
        public string UserName { get; set; }
        public string NetworkAddress { get; set; }
        public string RoomName { get; set; }
        public bool IsHost { get; set; }
    }
}
