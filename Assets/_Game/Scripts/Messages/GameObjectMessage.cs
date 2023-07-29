using UnityEngine;

namespace DLS.Game.Messages
{
    public struct GameObjectMessage
    {
        public GameObject GameObject { get;}

        public GameObjectMessage(GameObject gameObject)
        {
            GameObject = gameObject;
        }
    }
}