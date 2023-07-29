using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using PlayerPrefsPlus;
using UnityEngine;
using UnityEngine.Audio;

namespace DLS.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Preferences")]
        [SerializeField]
        private float volumeThreshold = -80.0f;

        [Header("References")]
        [SerializeField]
        private AudioMixer mixer;

        [SerializeField]
        private Audio[] music;

        [SerializeField]
        private Audio[] soundEffects;

        [SerializeField]
        private List<string> currentlyPlayingSfx, currentlyPlayingMusic;


        public static AudioManager instance;

        public Audio[] Music { get => music; }
        public Audio[] SoundEffects { get => soundEffects; }
        public List<string> CurrentlyPlayingSfx { get => currentlyPlayingSfx; }
        public List<string> CurrentlyPlayingMusic { get => currentlyPlayingMusic; }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AudioMessage>(MessageChannels.Audio, SetAudio);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AudioMessage>(MessageChannels.Audio, SetAudio);
        }

        // Awake is always called before any Start functions
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            for (int i = 0; i < music.Length; i++)
            {
                GameObject audioObject = new GameObject("Music_" + i + "_" + music[i].audioName);
                audioObject.transform.parent = transform;
                audioObject.AddComponent<AudioSource>();
                audioObject.GetComponent<AudioSource>().outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
                audioObject.GetComponent<AudioSource>().loop = music[i].audioLoop;
                audioObject.GetComponent<AudioSource>().volume = music[i].audioVolume;
                audioObject.GetComponent<AudioSource>().clip = music[i].audioClip;
                music[i].audioSource = audioObject.GetComponent<AudioSource>();
            }

            for (int i = 0; i < soundEffects.Length; i++)
            {
                GameObject audioObject = new GameObject("Effects_" + i + "_" + soundEffects[i].audioName);
                audioObject.transform.parent = transform;
                audioObject.AddComponent<AudioSource>();
                audioObject.GetComponent<AudioSource>().outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
                audioObject.GetComponent<AudioSource>().loop = soundEffects[i].audioLoop;
                audioObject.GetComponent<AudioSource>().volume = soundEffects[i].audioVolume;
                audioObject.GetComponent<AudioSource>().clip = soundEffects[i].audioClip;
                soundEffects[i].audioSource = audioObject.GetComponent<AudioSource>();
            }

            DontDestroyOnLoad(gameObject);

        }

        // Use this for initialization
        void Start()
        {
            mixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("masterVolume"));
            mixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
            mixer.SetFloat("effectsVolume", PlayerPrefs.GetFloat("effectsVolume"));
        }

        /// <summary>
        /// Play music from the array.
        /// </summary>
        public void PlayMusic(string name)
        {
            for (int i = 0; i < music.Length; i++)
            {
                if (music[i].audioName == name)
                {
                    currentlyPlayingMusic.Add(name);
                    music[i].Play();
                    StartCoroutine(StopPlayingMusicAfterAudioClipPlays(soundEffects[i].audioClip, name));
                    return;
                }
            }
        }

        /// <summary>
        /// Play a sound-effect from the array.
        /// </summary>
        public void PlaySound(string name)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                if (soundEffects[i].audioName == name)
                {
                    currentlyPlayingSfx.Add(name);
                    soundEffects[i].Play();
                    StartCoroutine(StopPlayingSoundAfterAudioClipPlays(soundEffects[i].audioClip, name));
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Pause music from the array.
        /// </summary>
        public void PauseMusic(string name)
        {
            for (int i = 0; i < music.Length; i++)
            {
                if (music[i].audioName == name)
                {
                    music[i].Pause();
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Pause a sound from the array.
        /// </summary>
        public void PauseSound(string name)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                if (soundEffects[i].audioName == name)
                {
                    soundEffects[i].Pause();
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Resume music from the array.
        /// </summary>
        public void ResumeMusic(string name)
        {
            for (int i = 0; i < music.Length; i++)
            {
                if (music[i].audioName == name)
                {
                    music[i].Resume();
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Resume a sound-effect from the array.
        /// </summary>
        public void ResumeSound(string name)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                if (soundEffects[i].audioName == name)
                {
                    soundEffects[i].Resume();
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Stop music from the array.
        /// </summary>
        public void StopMusic(string name)
        {
            for (int i = 0; i < music.Length; i++)
            {
                if (music[i].audioName == name)
                {
                    currentlyPlayingMusic.Remove(name);
                    music[i].Stop();
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Stop a sound-effect from the array.
        /// </summary>
        public void StopSound(string name)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                if (soundEffects[i].audioName == name)
                {
                    currentlyPlayingSfx.Remove(name);
                    soundEffects[i].Stop();
                    return;
                }
            }

            Debug.Log("AudioManager: " + name + " not found in list.");
        }

        /// <summary>
        /// Set the master volume of the audio mixer.
        /// </summary>
        public void SetMasterVolume(float sliderValue)
        {
            if (sliderValue <= 0)
            {
                mixer.SetFloat("masterVolume", volumeThreshold);
                PPPlus.SetFloat("masterVolume", -80);
            }
            else
            {
                // Translate unit range to logarithmic value. 
                float value = 20f * Mathf.Log10(sliderValue);
                mixer.SetFloat("masterVolume", value);
                PPPlus.SetFloat("masterVolume", value);
            }
        }

        /// <summary>
        /// Set the music volume of the audio mixer.
        /// </summary>
        public void SetMusicVolume(float sliderValue)
        {
            if (sliderValue <= 0)
            {
                mixer.SetFloat("musicVolume", volumeThreshold);
                PPPlus.SetFloat("musicVolume", -80);
            }
            else
            {
                // Translate unit range to logarithmic value. 
                float value = 20f * Mathf.Log10(sliderValue);
                mixer.SetFloat("musicVolume", value);
                PPPlus.SetFloat("musicVolume", value);
            }
        }

        /// <summary>
        /// Set the SFX volume of the audio mixer.
        /// </summary>
        public void SetSoundEffectsVolume(float sliderValue)
        {
            if (sliderValue <= 0)
            {
                mixer.SetFloat("effectsVolume", volumeThreshold);
                PPPlus.SetFloat("effectsVolume", -80);
            }
            else
            {
                // Translate unit range to logarithmic value. 
                float value = 20f * Mathf.Log10(sliderValue);
                mixer.SetFloat("effectsVolume", value);
                PPPlus.SetFloat("effectsVolume", value);
            }
        }

        /// <summary>
        /// Clear the master volume of the audio mixer. This is useful for audio snapshots.
        /// </summary>
        public void ClearMasterVolume()
        {
            mixer.ClearFloat("masterVolume");
        }

        /// <summary>
        /// Clear the music volume of the audio mixer. This is useful for audio snapshots.
        /// </summary>
        public void ClearMusicVolume()
        {
            mixer.ClearFloat("musicVolume");
        }

        /// <summary>
        /// Clear the SFX volume of the audio mixer. This is useful for audio snapshots.
        /// </summary>
        public void ClearSoundEffectsVolume()
        {
            mixer.ClearFloat("effectsVolume");
        }

        private IEnumerator StopPlayingSoundAfterAudioClipPlays(AudioClip clip, string name)
        {
            yield return new WaitForSeconds(clip.length);
            currentlyPlayingSfx.Remove(name);
        }
    
        private IEnumerator StopPlayingMusicAfterAudioClipPlays(AudioClip clip, string name)
        {
            yield return new WaitForSeconds(clip.length);
            currentlyPlayingMusic.Remove(name);
        }
        
        private void SetAudio(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AudioMessage>().HasValue)
                return;

            var audioMessage = message.Message<AudioMessage>().Value;

            switch (audioMessage.Operation)
            {
                case AudioOperation.Play:
                    switch (audioMessage.VolumeType)
                    {
                        case VolumeType.Music:
                            PlayMusic(audioMessage.AudioName);
                            break;
                        case VolumeType.SoundEffects:
                            PlaySound(audioMessage.AudioName);
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.Resume:
                    switch (audioMessage.VolumeType)
                    {
                        case VolumeType.Music:
                            ResumeMusic(audioMessage.AudioName);
                            break;
                        case VolumeType.SoundEffects:
                            ResumeSound(audioMessage.AudioName);
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.Pause:
                    switch (audioMessage.VolumeType)
                    {
                        case VolumeType.Music:
                            PauseMusic(audioMessage.AudioName);
                            break;
                        case VolumeType.SoundEffects:
                            PauseSound(audioMessage.AudioName);
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.Stop:
                    switch (audioMessage.VolumeType)
                    {
                        case VolumeType.Music:
                            StopMusic(audioMessage.AudioName);
                            break;
                        case VolumeType.SoundEffects:
                            StopSound(audioMessage.AudioName);
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.SetVolume:
                    if (audioMessage.Volume.HasValue && audioMessage.VolumeType.HasValue)
                    {
                        switch (audioMessage.VolumeType.Value)
                        {
                            case VolumeType.Master:
                                SetMasterVolume(audioMessage.Volume.Value);
                                break;
                            case VolumeType.Music:
                                SetMusicVolume(audioMessage.Volume.Value);
                                break;
                            case VolumeType.SoundEffects:
                                SetSoundEffectsVolume(audioMessage.Volume.Value);
                                break;
                            default:
                                Debug.Log("Invalid volume type.");
                                break;
                        }
                    }
                    break;
                default:
                    Debug.Log("Invalid audio operation.");
                    break;
            }
        }

    }
}