using System;
using DLS.Game.Enums;
using DLS.Game.Utilities;

namespace DLS.Game.Messages
{
    public struct SaveLoadMessage
    {
        public SerializableGuid PlayerID { get; }
        
        public string PlayerName { get; }
        
        public SaveOperation SaveOperationType { get; }

        public SaveLoadMessage(SerializableGuid playerID, string playerName, SaveOperation saveOperationType)
        {
            PlayerID = playerID;
            PlayerName = playerName;
            SaveOperationType = saveOperationType;
        }
        
    }
}