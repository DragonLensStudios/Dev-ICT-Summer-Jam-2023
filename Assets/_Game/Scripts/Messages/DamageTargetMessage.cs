using DLS.Game.Utilities;
using UnityEngine;

namespace DLS.Game.Messages
{
    public struct DamageTargetMessage
    {
        public GameObject SourceAttacker { get; }
        public SerializableGuid ID { get; }
        public int Damage { get; }
        
        public float KnockBack { get; }

        public DamageTargetMessage(GameObject sourceAttacker, SerializableGuid id, int damage, float knockBack)
        {
            SourceAttacker = sourceAttacker;
            ID = id;
            Damage = damage;
            KnockBack = knockBack;
        }
    }
}