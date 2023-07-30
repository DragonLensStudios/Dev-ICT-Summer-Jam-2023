using DLS.Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace DLS.Game.Messages
{
    public struct CrystalSpawnerMessage
    {
        public GameObject TowerPrefab { get; }
        public int CurrentStack { get; }
        public int MaxStack { get; }
        public List<GameObject> Crystals { get; }


        public CrystalSpawnerMessage(GameObject go, int currentStack, int maxStack, List<GameObject> crystals = default)
        {
            TowerPrefab = go;
            CurrentStack = currentStack;
            MaxStack = maxStack;
            Crystals = crystals;
        }
    }
}