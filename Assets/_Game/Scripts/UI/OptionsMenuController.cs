using System;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using PlayerPrefsPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DLS.Core.UI
{
    public class OptionsMenuController : MonoBehaviour
    {
        [field: SerializeField] public Scrollbar MasterVolumeScrollbar { get; set; }
        [field: SerializeField] public TMP_Text MasterVolumeText { get; set; }
        [field: SerializeField] public Scrollbar SFXVolumeScrollbar { get; set; }
        [field: SerializeField] public TMP_Text SFXVolumeText { get; set; }
        [field: SerializeField] public Scrollbar BGMVolumeScrollbar { get; set; }
        [field: SerializeField] public TMP_Text BGMVolumeText { get; set; }
        private void Start()
        {
            var masterVolume = Mathf.Pow(10, PPPlus.GetFloat("masterVolume") / 20f);
            var sfxVolume = Mathf.Pow(10, PPPlus.GetFloat("effectsVolume") / 20f);
            var bgmVolume = Mathf.Pow(10, PPPlus.GetFloat("musicVolume") / 20f);
            MasterVolumeScrollbar.value = masterVolume;
            SFXVolumeScrollbar.value = sfxVolume;
            BGMVolumeScrollbar.value = bgmVolume;        
        }
        public void SetMasterVolume(float volume)
        {
            MasterVolumeText.text = $"Master Volume: {volume * 100}%";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage("",AudioOperation.SetVolume, VolumeType.Master, volume));
        }
        
        public void SetBGMVolume(float volume)
        {
            BGMVolumeText.text = $"BGM Volume: {volume * 100}%";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage("",AudioOperation.SetVolume, VolumeType.Music, volume));
        }
        
        public void SetSFXVolume(float volume)
        {
            SFXVolumeText.text = $"SFX Volume: {volume * 100}%";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage("",AudioOperation.SetVolume, VolumeType.SoundEffects, volume));
        }
    }
}
