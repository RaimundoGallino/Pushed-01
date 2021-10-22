using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = .5f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool isLoop;
    public bool hasCooldown;

    [System.NonSerialized] public AudioSource source;
    [System.NonSerialized] public SoundCategory myCategory;

    public void Play()
    {
        source.pitch = pitch;
        source.volume = volume;
        source.loop = isLoop;

        source.Play();
        myCategory.lastClipPlayed = clip;
    }

    public void PlayAtPosition(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(clip, pos, source.volume);
        myCategory.lastClipPlayed = clip;
    }
}

[System.Serializable]
public class SoundEntity
{
    public string name;
    public SoundCategory[] soundCategories;
}

[System.Serializable]
public class SoundCategory
{
    public string name;
    public Sound[] sounds;
    [System.NonSerialized] public AudioClip lastClipPlayed;
    public bool hasCooldown;

    public AudioMixerGroup mixer;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;

    public SoundEntity[] soundEntities;


    private static Dictionary<Sound, float> soundTimerDictionary;
    private static Dictionary<SoundCategory, float> categoryTimerDictionary;

    private void Awake()
    {
        if (current && current != this)
            Destroy(gameObject);

        current = this;
        DontDestroyOnLoad(gameObject);


        soundTimerDictionary = new Dictionary<Sound, float>();
        categoryTimerDictionary = new Dictionary<SoundCategory, float>();

        // Sounds initializer
        foreach (SoundEntity entity in soundEntities)
        {
            foreach (SoundCategory category in entity.soundCategories)
            {
                foreach (Sound sound in category.sounds)
                {
                    sound.source = gameObject.AddComponent<AudioSource>();
                    sound.source.clip = sound.clip;

                    sound.source.volume = sound.volume;
                    sound.source.pitch = sound.pitch;
                    sound.source.loop = sound.isLoop;
                    sound.source.outputAudioMixerGroup = category.mixer;

                    sound.myCategory = category;

                    if (sound.hasCooldown)
                        soundTimerDictionary[sound] = 0f;
                }

                if (category.hasCooldown)
                    categoryTimerDictionary[category] = 0f;
            }
        }

        current = this;
    }

    private void Start()
    {
        // Add this part after having a theme song
        // Play('Theme');
        AudioManager.current.Play("Game", "Music", "Lobby");
    }

    /// <summary> Reproduce un sonido </summary>
    public void Play(string entityName, string categoryName, string soundName)
    {
        Sound sound = GetSound(entityName, categoryName, soundName);
        if (sound == null)
            return;

        sound.Play();
    }

    /// <summary> Reproduce un sonido en la posicion indicada en el world space </summary>
    public void PlayAtPosition(string entityName, string categoryName, string soundName, Vector3 pos)
    {
        Sound sound = GetSound(entityName, categoryName, soundName);

        if (sound == null)
            return;

        AudioClip targetClip = sound.clip;

        sound.PlayAtPosition(pos);
    }

    public void PlayRandomClip(string entityName, string categoryName)
    {
        SoundCategory category = GetCategory(entityName, categoryName);
        if (category == null)
            return;

        int randIndex = UnityEngine.Random.Range(0, category.sounds.Length);
        Sound sound = category.sounds[randIndex];
        AudioClip targetClip = sound.clip;

        if (!CanPlaySound(sound)) return;

        sound.Play();
    }

    public void PlayRandomClipAtPosition(string entityName, string categoryName, Vector3 pos)
    {
        SoundCategory category = GetCategory(entityName, categoryName);
        if (category == null)
            return;

        int randIndex = UnityEngine.Random.Range(0, category.sounds.Length);
        Sound sound = category.sounds[randIndex];
        AudioClip targetClip = sound.clip;

        if (!CanPlaySound(sound)) return;

        sound.PlayAtPosition(pos);
    }

    private SoundCategory GetCategory(string entityName, string categoryName)
    {
        SoundEntity entity = Array.Find(current.soundEntities, ent => ent.name == entityName);
        if (entity == null)
        {
            Debug.LogError($"SoundEntity {entityName} not found!");
            return null;
        }
        SoundCategory category = Array.Find(entity.soundCategories, category => category.name == categoryName);
        if (category == null)
        {
            Debug.LogError($"SoundCategory in {entityName} named {categoryName} not found!");
            return null;
        }

        if (!CanPlayCategory(category)) return null;

        return category;
    }
    private Sound GetSound(string entityName, string categoryName, string soundName)
    {
        SoundCategory category = GetCategory(entityName, categoryName);
        if (category == null) return null;

        Sound sound = Array.Find(category.sounds, s => s.name == soundName);
        if (sound == null) {
            Debug.LogError($"Sound inside {entityName} called {soundName} not found!");
            return null;
        }

        if (!CanPlaySound(sound)) return null;

        return sound;
    }

    /// <summary> Detiene la reproduccion de un sonido </summary>
    public void Stop(string entityName, string categoryName, string soundName)
    {
        Sound sound = GetSound(entityName, categoryName, soundName);

        if (sound == null) return;

        sound.source.Stop();
    }

    private bool CanPlayCategory(SoundCategory category)
    {
        if (!categoryTimerDictionary.ContainsKey(category)) return true;

        float lastTimePlayed = categoryTimerDictionary[category];

        if (category.lastClipPlayed == null)
            return true;

        if (lastTimePlayed + category.lastClipPlayed.length < Time.time)
        {
            categoryTimerDictionary[category] = Time.time;
            return true;
        }

        return false;
    }

    private bool CanPlaySound(Sound sound)
    {
        if (!soundTimerDictionary.ContainsKey(sound)) return true;

        float lastTimePlayed = soundTimerDictionary[sound];

        if (lastTimePlayed + sound.clip.length < Time.time)
        {
            soundTimerDictionary[sound] = Time.time;
            return true;
        }

        return false;
    }
}
