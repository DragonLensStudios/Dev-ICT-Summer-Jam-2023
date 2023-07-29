using DLS.Game.Enums;
using DLS.Game.Utilities;
using UnityEngine;

namespace DLS.Game.Messages
{
    public struct LevelMessage
    {
        public SerializableGuid LevelID { get; }
        public string LevelName { get; }
        public LevelState LevelState { get; }
        public Vector2 Position { get; }
        
        public LevelMessage(SerializableGuid levelID, string levelName, LevelState levelState, Vector2 position)
        {
            LevelID = levelID;
            LevelName = levelName;
            LevelState = levelState;
            Position = position;
        }
    }
}