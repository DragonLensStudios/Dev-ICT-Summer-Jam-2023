using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DLS.Core.Audio
{
    public class SceneMusicPlayer : MonoBehaviour
    {
        public List<string> musicList = new List<string>();

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if(musicList.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(musicList[scene.buildIndex]))
                {
                    return;;
                }
                if (AudioManager.instance.CurrentlyPlayingMusic.Contains(musicList[scene.buildIndex]))
                {
                    return;
                }
                if (scene.buildIndex > 0)
                {
                    AudioManager.instance.StopMusic(musicList[scene.buildIndex - 1]);
                }
                AudioManager.instance.PlayMusic(musicList[scene.buildIndex]);
            }
        }
    }
}
