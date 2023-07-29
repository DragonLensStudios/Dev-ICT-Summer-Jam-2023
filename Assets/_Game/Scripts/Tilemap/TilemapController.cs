using DLS.Core.Data_Persistence;
using DLS.Game.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DLS.Game.Tilemaps
{
    public class TilemapController : MonoBehaviour
    {
        [field: SerializeField] public SerializableDictionary<SerializableGuid, Tilemap> Tilemaps = new();
        private void Start()
        {
            if (Tilemaps.Count > 0) return;
            Tilemaps.Clear();
            foreach (var tilemap in GetComponentsInChildren<Tilemap>())
            {
                var Idd = tilemap.GetComponent<IID>();
                if (Idd != null)
                {
                    Tilemaps.Add(Idd.ID, tilemap);
                }
            }
        }

        public void GetTilemaps()
        {
            Tilemaps.Clear();
            foreach (var tilemap in GetComponentsInChildren<Tilemap>())
            {
                var Idd = tilemap.GetComponent<IID>();
                if (Idd != null)
                {
                    Tilemaps.Add(Idd.ID, tilemap);
                }
            }
        }
    }
}
