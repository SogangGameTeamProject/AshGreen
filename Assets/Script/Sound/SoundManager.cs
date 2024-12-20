using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

namespace AshGreen.Sound
{
    public class SoundManager : Singleton<SoundManager>
    {
        public AudioMixer audioMixer = null;
        // BGM과 SFX를 위한 AudioSources
        public AudioSource bgmSource;
        public AudioSource sfxSource;
        public string BGM_VOLUME_KEY = "BGMSoundScale";
        public string SFX_VOLUME_Key = "SFXSoundScale";

        private void Start()
        {
            //사운드 값 초기화
            if (audioMixer)
            {
                float bgmValue = PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
                float sfxValue = PlayerPrefs.GetFloat(SFX_VOLUME_Key);
                audioMixer.SetFloat("BGM", bgmValue);
                audioMixer.SetFloat("SFX", sfxValue);
            }
            
        }

        private void OnDisable()
        {
            if (audioMixer)
            {
                //사운드 값 저장
                float bgmValue;
                audioMixer.GetFloat("BGM", out bgmValue);
                float sfxValue;
                audioMixer.GetFloat("SFX", out sfxValue);


                PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmValue);
                PlayerPrefs.SetFloat(SFX_VOLUME_Key, sfxValue);
                PlayerPrefs.Save(); // 즉시 저장
            }
        }

        // BGM 재생
        public void PlayBGM(AudioClip bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayBGMRpc(AudioClip bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }

        // SFX 재생
        public void PlaySFX(AudioClip sfxClip)
        {
            sfxSource.PlayOneShot(sfxClip);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlaySFXRpc(AudioClip sfxClip)
        {
            sfxSource.PlayOneShot(sfxClip);
        }
    }
}

