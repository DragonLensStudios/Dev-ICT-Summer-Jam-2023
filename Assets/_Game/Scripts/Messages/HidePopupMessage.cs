using DLS.Game.Enums;

namespace DLS.Game.Messages
{
    public struct HidePopupMessage
    {
        public PopupType PopupType { get; }

        public HidePopupMessage(PopupType popupType)
        {
            PopupType = popupType;
        }
    }
}