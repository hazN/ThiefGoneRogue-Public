using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRandomizer : MonoBehaviour
{
    [SerializeField] private bool PlayOnAwake = false;
    [InfoBox("Set the minimum and maximum pitches to randomize between when playing a sound.")]
    [MinMaxSlider(0, 3, true)]
    public Vector2 MinMaxPitchSlider = new Vector2(0.9f, 1.1f);
    [SerializeField] private AudioClip[] sounds;
    AudioSource audioSource = null;
    public float GetMinPitch
    {
        get { return MinMaxPitchSlider.x; }
        set { MinMaxPitchSlider.x = value; }
    }

    public float GetMaxPitch
    {
        get { return MinMaxPitchSlider.y; }
        set { MinMaxPitchSlider.y = value; }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (PlayOnAwake) PlaySound();
    }
    public void PlaySound()
    {
        if (sounds.Length == 0) return;
        audioSource.pitch = Random.Range(GetMinPitch, GetMaxPitch);
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
}
