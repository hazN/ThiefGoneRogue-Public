using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace RPG.Sounds
{
    public class MusicPlayer : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private AudioClip[] clips = null;

        [SerializeField] private AudioMixerGroup mixerGroup = null;

        [BoxGroup("Config")]
        [MinMaxSlider(0, 1)]
        [SerializeField]
        private Vector2 volume = new Vector2(0.5f, 0.5f);

        [BoxGroup("Config")]
        [MinMaxSlider(0, 3)]
        [SerializeField]
        private Vector2 pitch = new Vector2(1f, 1f);

        private enum SoundClipPlayOrder
        { random, in_order, reversed }

        [BoxGroup("Config")]
        [SerializeField]
        private SoundClipPlayOrder playOrder = SoundClipPlayOrder.random;

        [BoxGroup("Config")][DisplayAsString][SerializeField] private int playIndex = 0;

        private void Update()
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                if (GetComponent<AudioSource>().clip == null)
                {
                    GetComponent<AudioSource>().clip = GetAudioClip();
                    GetComponent<AudioSource>().Play();
                }
                else
                {
                    GetComponent<AudioSource>().Play();
                }
            }
        }
        public void PlayClip(AudioClip clip)
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }
        private AudioClip GetAudioClip()
        {
            // Grab current clip
            AudioClip clip = clips[playIndex >= clips.Length ? 0 : playIndex];

            // Get next clip
            switch (playOrder)
            {
                case SoundClipPlayOrder.random:
                    playIndex = Random.Range(0, clips.Length);
                    break;

                case SoundClipPlayOrder.in_order:
                    playIndex = (playIndex + 1) % clips.Length;
                    break;

                case SoundClipPlayOrder.reversed:
                    playIndex = (playIndex - 1) % clips.Length;
                    break;
            }

            return clip;
        }

        public AudioSource Play(AudioSource audioSourceParam = null)
        {
            if (clips.Length == 0)
            {
                Debug.LogWarning($"Missing sound clips for {name}");
                return null;
            }
            var source = audioSourceParam;
            if (source == null)
            {
                var _obj = new GameObject("Sound", typeof(AudioSource));
                source = _obj.GetComponent<AudioSource>();
            }
            // Set volume and pitch
            if (mixerGroup != null)
                source.outputAudioMixerGroup = mixerGroup;
            source.clip = GetAudioClip();
            source.volume = Random.Range(volume.x, volume.y);
            source.pitch = Random.Range(pitch.x, pitch.y);

            source.Play();

            return source;
        }
    }
}