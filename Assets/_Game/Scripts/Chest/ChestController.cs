using System;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.Chests
{
    [RequireComponent(typeof(Animator), typeof(BoxCollider2D))]
    public class ChestController : MonoBehaviour
    {
        [field: SerializeField] public string OpenMessage { get; set; }
        [field: SerializeField] public string CloseMessage { get; set; }
        [field: SerializeField] public float MessageDisplayTime { get; set; }
        [field: SerializeField] public PopupPosition MessageDisplayLocation { get; set; }
        [field: SerializeField] public bool IsOpen { get; set; }


        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public bool Open()
        {
            IsOpen = true;
            anim.SetBool("IsOpen", IsOpen);
            if (!string.IsNullOrWhiteSpace(OpenMessage))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage(OpenMessage, PopupType.Notification, MessageDisplayLocation,MessageDisplayTime));
            }
            return true;
        }

        public bool Close()
        {
            IsOpen = false;
            anim.SetBool("IsOpen", IsOpen);
            if (!string.IsNullOrWhiteSpace(CloseMessage))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage(CloseMessage, PopupType.Notification, MessageDisplayLocation,MessageDisplayTime));
            }

            return true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                Open();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Close();
            }
        }
    }
}
