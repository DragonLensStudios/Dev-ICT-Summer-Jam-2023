using System;
using UnityEngine;
using Object = System.Object;

namespace DLS.Game.Utilities
{
    [Serializable]
    public class SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField]
        private string guidString;

        public Guid Guid
        {
            get { return !string.IsNullOrEmpty(guidString) ? new Guid(guidString) : Guid.Empty; }
            set
            {
                guidString = value.ToString();
            }
        }

        public SerializableGuid(Guid guid)
        {
            Guid = guid;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SerializableGuid);
        }

        public bool Equals(SerializableGuid other)
        {
            return other != null && Guid.Equals(other.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
        
        public static bool operator ==(SerializableGuid left, SerializableGuid right)
        {
            return left?.Equals(right) ?? ReferenceEquals(right, null);
        }

        public static bool operator !=(SerializableGuid left, SerializableGuid right)
        {
            return !(left == right);
        }
    }
}