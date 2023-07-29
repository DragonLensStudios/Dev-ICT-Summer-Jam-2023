using System;
using UnityEngine;

namespace DLS.Dialogue
{
    [Serializable]
    public class ComparableVector3 : IComparable<ComparableVector3>, IEquatable<ComparableVector3>
    {
        [SerializeField]
        private float x;
        [SerializeField]
        private float y;
        [SerializeField]
        private float z;

        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public float Z
        {
            get => z;
            set => z = value;
        }

        public Vector3 Value
        {
            get => new Vector3(x, y, z);
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public ComparableVector3(Vector3 value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        public int CompareTo(ComparableVector3 other)
        {
            return Value.sqrMagnitude.CompareTo(other.Value.sqrMagnitude);
        }

        public bool Equals(ComparableVector3 other)
        {
            return Value == other.Value;
        }

        // Addition operator
        public static ComparableVector3 operator +(ComparableVector3 a, ComparableVector3 b)
        {
            return new ComparableVector3(a.Value + b.Value);
        }

        // Subtraction operator
        public static ComparableVector3 operator -(ComparableVector3 a, ComparableVector3 b)
        {
            return new ComparableVector3(a.Value - b.Value);
        }

        // Element-wise multiplication operator
        public static ComparableVector3 operator *(ComparableVector3 a, ComparableVector3 b)
        {
            return new ComparableVector3(new Vector3(a.x * b.x, a.y * b.y, a.z * b.z));
        }

        // Element-wise division operator
        public static ComparableVector3 operator /(ComparableVector3 a, ComparableVector3 b)
        {
            return new ComparableVector3(new Vector3(a.x / b.x, a.y / b.y, a.z / b.z));
        }
        
        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}
