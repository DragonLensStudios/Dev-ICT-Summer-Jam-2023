using System;
using DLS.Utilities;

namespace DLS.Game.Messages
{
    public struct AchievementMenuMessage
    {
        public bool ShowMenu { get; }

        public AchievementMenuMessage(bool showMenu)
        {
            ShowMenu = showMenu;
        }
        
    }
}