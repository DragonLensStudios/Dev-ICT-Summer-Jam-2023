using System.Collections.Generic;
using UnityEngine;

namespace DLS.Sprite_To_Animation
{
    [CreateAssetMenu(fileName = "NewTemplate", menuName = "DLS/Sprite Animation Template")]
    public class SpriteAnimationTemplate : ScriptableObject
    {
        public string templateName;
        public Vector2Int gridSize;  // Grid size for slicing
        public List<AnimationDetail> animationDetails;  // Animation details per grid cell or row
    }
}