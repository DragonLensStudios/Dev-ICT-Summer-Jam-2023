using DLS.Core;

namespace DLS.Game.Messages
{
    public struct AchievementMessage
    {
        public string AchievementKey { get; }
        
        public AchievementOperator Operator { get; }

        public float Progress { get; }
        

        public AchievementMessage(string achievementKey, AchievementOperator op, float progress = 0f)
        {
            AchievementKey = achievementKey;
            Progress = progress;
            Operator = op;
        }
    }
}