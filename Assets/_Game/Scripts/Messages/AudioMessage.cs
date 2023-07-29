using DLS.Game.Enums;

namespace DLS.Game.Messages
{
    public struct AudioMessage
    {
        public string AudioName { get; }
        public AudioOperation Operation { get; }
        public VolumeType? VolumeType { get; }
        public float? Volume { get; }

        public AudioMessage(string audioName, AudioOperation operation, VolumeType? volumeType = null, float? volume = null)
        {
            AudioName = audioName;
            Operation = operation;
            VolumeType = volumeType;
            Volume = volume;
        }
    }
}