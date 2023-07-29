using UnityEngine;
using UnityEngine.Serialization;

namespace DLS.Sprite_To_Animation
{
    [System.Serializable]
    public class AnimationDetail
    {
        /// <summary>
        /// Row index for animation
        /// </summary>
        public string animationName;
        /// <summary>
        /// Starting column index for animation
        /// </summary>
        public int row;        
        /// <summary>
        /// Ending column index for animation using 1 based index
        /// </summary>
        public int startColumn;
        /// <summary>
        /// Ending column index for animation using 1 based index
        /// </summary>
        public int endColumn;
        /// <summary>
        /// Framerate for the animation
        /// </summary>
       public int frameRate = 12;
        /// <summary>
        /// The animation type to geenerate
        /// </summary>
        public AnimationType animationType;
        /// <summary>
        /// the direction for the animation
        /// </summary>
        public Vector2 direction;
        /// <summary>
        /// Whether to loop animation
        /// </summary>
        public bool loop = true;
    }
}