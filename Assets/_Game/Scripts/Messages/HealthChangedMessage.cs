using DLS.Core;
using DLS.Game.Utilities;

namespace DLS.Game.Messages
{
    public struct HealthChangedMessage
    {
        public SerializableGuid ObjectID { get; }
        public int Amount { get; }
        public HealthChangeType ChangeType { get;}

        public HealthChangedMessage(SerializableGuid objectID, int amount, HealthChangeType changeType)
        {
            ObjectID = objectID;
            Amount = amount;
            ChangeType = changeType;
        }
    }
}