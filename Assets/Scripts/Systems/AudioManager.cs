using UnityEngine;
using System.Collections.Generic;
using FPSGame.Utilities;

namespace FPSGame.Systems
{
    /// <summary>
    /// Manages all audio in the game with 2D and 3D spatial audio support.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            public AudioType audioType;
            
            [Range(0f, 1f)]
            public float volume = 1f;
            
            [Range(0.1f, 3f)]
            public float pitch = 1f;
            
            public bool loop = false;
            public bool is3D = false;
            
            [HideInInspector]
            public AudioSource source;
        }

        [Header("Audio Settings")]
        [SerializeField] private List<Sound> sounds = new List<Sound>();
        
        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
        
        private Dictionary<string, Sound> soundDictionary;

        /// <summary>
        /// Gets or sets the master volume.
        /// </summary>
        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = Mathf.Clamp01(value);
                UpdateAllVolumes();
            }
        }

        /// <summary>
        /// Gets or sets the music volume.
        /// </summary>
        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                UpdateAllVolumes();
            }
        }

        /// <summary>
        /// Gets or sets the SFX volume.
        /// </summary>
        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = Mathf.Clamp01(value);
                UpdateAllVolumes();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            soundDictionary = new Dictionary<string, Sound>();
            
            foreach (Sound sound in sounds)
            {
                if (sound.clip == null)
                    continue;

                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                
                if (sound.is3D)
                {
                    sound.source.spatialBlend = 1f;
                    sound.source.rolloffMode = AudioRolloffMode.Linear;
                    sound.source.maxDistance = 50f;
                }
                else
                {
                    sound.source.spatialBlend = 0f;
                }

                soundDictionary[sound.name] = sound;
            }
        }

        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        public void Play(string soundName)
        {
            if (!soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound {soundName} not found!");
                return;
            }

            Sound sound = soundDictionary[soundName];
            sound.source.volume = CalculateVolume(sound);
            sound.source.Play();
        }

        /// <summary>
        /// Plays a 3D sound at a specific position.
        /// </summary>
        public void Play3D(string soundName, Vector3 position)
        {
            if (!soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound {soundName} not found!");
                return;
            }

            Sound sound = soundDictionary[soundName];
            
            // Create temporary audio source at position
            GameObject audioObject = new GameObject($"Audio_{soundName}");
            audioObject.transform.position = position;
            
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.volume = CalculateVolume(sound);
            audioSource.pitch = sound.pitch;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 50f;
            
            audioSource.Play();
            
            Destroy(audioObject, sound.clip.length);
        }

        /// <summary>
        /// Stops a sound by name.
        /// </summary>
        public void Stop(string soundName)
        {
            if (!soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound {soundName} not found!");
                return;
            }

            soundDictionary[soundName].source.Stop();
        }

        /// <summary>
        /// Pauses a sound by name.
        /// </summary>
        public void Pause(string soundName)
        {
            if (!soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound {soundName} not found!");
                return;
            }

            soundDictionary[soundName].source.Pause();
        }

        /// <summary>
        /// Resumes a sound by name.
        /// </summary>
        public void Resume(string soundName)
        {
            if (!soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound {soundName} not found!");
                return;
            }

            soundDictionary[soundName].source.UnPause();
        }

        /// <summary>
        /// Checks if a sound is playing.
        /// </summary>
        public bool IsPlaying(string soundName)
        {
            if (!soundDictionary.ContainsKey(soundName))
                return false;

            return soundDictionary[soundName].source.isPlaying;
        }

        /// <summary>
        /// Calculates the final volume for a sound based on type and master volume.
        /// </summary>
        private float CalculateVolume(Sound sound)
        {
            float typeVolume = sound.audioType == AudioType.BackgroundMusic ? musicVolume : sfxVolume;
            return sound.volume * typeVolume * masterVolume;
        }

        /// <summary>
        /// Updates all sound volumes.
        /// </summary>
        private void UpdateAllVolumes()
        {
            foreach (var kvp in soundDictionary)
            {
                Sound sound = kvp.Value;
                if (sound.source != null && sound.source.isPlaying)
                {
                    sound.source.volume = CalculateVolume(sound);
                }
            }
        }

        /// <summary>
        /// Adds a new sound at runtime.
        /// </summary>
        public void AddSound(string name, AudioClip clip, AudioType audioType, float volume = 1f, bool loop = false, bool is3D = false)
        {
            if (soundDictionary.ContainsKey(name))
            {
                Debug.LogWarning($"Sound {name} already exists!");
                return;
            }

            Sound sound = new Sound
            {
                name = name,
                clip = clip,
                audioType = audioType,
                volume = volume,
                loop = loop,
                is3D = is3D
            };

            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = clip;
            sound.source.volume = volume;
            sound.source.loop = loop;
            sound.source.spatialBlend = is3D ? 1f : 0f;

            soundDictionary[name] = sound;
            sounds.Add(sound);
        }

        /// <summary>
        /// Stops all sounds.
        /// </summary>
        public void StopAll()
        {
            foreach (var kvp in soundDictionary)
            {
                kvp.Value.source.Stop();
            }
        }

        /// <summary>
        /// Pauses all sounds.
        /// </summary>
        public void PauseAll()
        {
            foreach (var kvp in soundDictionary)
            {
                kvp.Value.source.Pause();
            }
        }

        /// <summary>
        /// Resumes all sounds.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var kvp in soundDictionary)
            {
                kvp.Value.source.UnPause();
            }
        }
    }
}
