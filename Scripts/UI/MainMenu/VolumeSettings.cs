using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RPG.UI
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider musicVolume;
        [SerializeField] private Slider sfxVolume;
        private void Awake()
        {
            masterVolume.onValueChanged.AddListener(SetMasterVolume);
            musicVolume.onValueChanged.AddListener(SetMusicVolume);
            sfxVolume.onValueChanged.AddListener(SetSFXVolume);
        }
        private void Start()
        {
            masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
            musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            sfxVolume.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        }
        private void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MasterVolume", masterVolume.value);
        }
        private void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);
        }
        private void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume.value);
        }
    }
}