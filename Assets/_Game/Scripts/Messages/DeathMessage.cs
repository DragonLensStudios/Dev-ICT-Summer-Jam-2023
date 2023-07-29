using DLS.Game.Utilities;

namespace DLS.Game.Messages
{
    public struct DeathMessage
    {
        public SerializableGuid ObjectID { get; }

        public DeathMessage(SerializableGuid objectID)
        {
            ObjectID = objectID;
        }
    }
}