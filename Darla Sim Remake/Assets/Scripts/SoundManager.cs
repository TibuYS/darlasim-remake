using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{

    public AudioMixer audioMixer;
    [Space]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup SFXGroup;
    public AudioMixerGroup voiceLinesGroup;
    public List<AudioClip> clips = new List<AudioClip>();
    public List<AudioSource> currentlyPlaying = new List<AudioSource>();


    public static SoundManager instance;

    public enum AudioGroup
    {
        Music,
        SFX,
        VoiceLines
    }


    public void GatherCurrentlyPlaying()
    {
        currentlyPlaying.Clear();
        AudioSource[] allAudioSources = GameObject.FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                currentlyPlaying.Add(audioSource);
            }
        }
    }

    public void ChangeAllPitch(float newPitch)
    {
        for (int i = currentlyPlaying.Count - 1; i >= 0; i--)
        {
            AudioSource source = currentlyPlaying[i];
            if (source != null)
            {
                source.pitch = newPitch;
            }
            else
            {
                currentlyPlaying.RemoveAt(i);
            }
        }
    }

    private void Update()
    {
        
    }

    public AudioClip getClip(string name)
    {
        foreach(AudioClip clip in clips)
        {
            if(clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// chooses a random audio to play from your provided list
    /// </summary>
    /// <param name="listOfAudios">the audios the function will choose from</param>
    /// <param name="volume">the volume of the audio that will play.</param>
    /// <param name="shouldLoop">should the audiosource be looped?.</param>
    /// <param name="source">the audiosource that's going to play the sound, if you leave it on null, the function will create one.</param>
    public void PlayRandomFromList(List<AudioClip> listOfAudios, float volume = 1, bool shouldLoop = true, AudioGroup group = AudioGroup.Music, AudioSource source = null)
    {
        AudioSource sourceToPlayFrom;

        if (source != null)
        {
            sourceToPlayFrom = source;
        }
        else
        {
            GameObject audioObject = new GameObject("AudioSourceObject");
            sourceToPlayFrom = audioObject.AddComponent<AudioSource>();
            sourceToPlayFrom.minDistance = 0.1f;
            sourceToPlayFrom.maxDistance = 1000000;
        }

        switch (group)
        {
            case AudioGroup.Music:
                sourceToPlayFrom.outputAudioMixerGroup = musicGroup;
                break;
            case AudioGroup.SFX:
                sourceToPlayFrom.outputAudioMixerGroup = SFXGroup;
                break;
            case AudioGroup.VoiceLines:
                sourceToPlayFrom.outputAudioMixerGroup = voiceLinesGroup;
                break;
        }

        sourceToPlayFrom.volume = volume;
        sourceToPlayFrom.loop = shouldLoop;

        AudioClip randomAudio = listOfAudios[Random.Range(0, listOfAudios.Count)];
        sourceToPlayFrom.clip = randomAudio;
        sourceToPlayFrom.gameObject.name = "Now Playing: " + randomAudio.name;
        sourceToPlayFrom.Play();
        currentlyPlaying.Add(sourceToPlayFrom);
    }

    /// <summary>
    /// plays an audio that can only be heard if you are between "minDistance" and "maxDistance", at a specific location.
    /// </summary>
    /// <param name="audio">the audio you'd like to play.</param>
    /// <param name="volume">the volume of the audio you'd like to play.</param>
    /// <param name="location">the audiosource will spawn at your desired location.</param>
    /// <param name="shouldLoop">should the audiosource be looped?.</param>
    /// <param name="minDistance">minimum distance, aka the closest you have to be in order to hear the music.</param>
    /// <param name="maxDistance">maximum distance, aka the furthest you can be in order to still hear the music..</param>
    public void PlayAudioAtLocation(AudioClip audio, float volume, Vector3 location, bool shouldLoop, float minDistance, float maxDistance, AudioGroup group)
    {
        AudioSource sourceToPlayFrom;
        GameObject audioObject = new GameObject("Now Playing: " + audio.name);
        sourceToPlayFrom = audioObject.AddComponent<AudioSource>();

        sourceToPlayFrom.clip = audio;
        sourceToPlayFrom.rolloffMode = AudioRolloffMode.Linear;
        sourceToPlayFrom.maxDistance = maxDistance;
        sourceToPlayFrom.minDistance = minDistance;
        sourceToPlayFrom.spatialBlend = 1;
        sourceToPlayFrom.volume = volume;
        sourceToPlayFrom.loop = shouldLoop;

        switch (group)
        {
            case AudioGroup.Music:
                sourceToPlayFrom.outputAudioMixerGroup = musicGroup;
                break;
            case AudioGroup.SFX:
                sourceToPlayFrom.outputAudioMixerGroup = SFXGroup;
                break;
            case AudioGroup.VoiceLines:
                sourceToPlayFrom.outputAudioMixerGroup = voiceLinesGroup;
                break;
        }

        audioObject.transform.position = location;
        sourceToPlayFrom.Play();
        currentlyPlaying.Add(sourceToPlayFrom);

        if (!shouldLoop)
        {
            Destroy(audioObject, audio.length);
        }
    }

    /// <summary>
    /// plays an audio that can be heard wherever the audio listener is in the scene.
    /// </summary>
    /// <param name="audio">the audio you'd like to play.</param>
    /// <param name="volume">the volume of the audio you'd like to play.</param>
    /// <param name="shouldLoop">should the audiosource be looped?.</param>
    public void PlaySound(AudioClip audio, float volume, bool shouldLoop, AudioGroup group)
    {
        AudioSource sourceToPlayFrom;
        GameObject audioObject = new GameObject("Now Playing: " + audio.name);
        sourceToPlayFrom = audioObject.AddComponent<AudioSource>();

        sourceToPlayFrom.clip = audio;
        sourceToPlayFrom.volume = volume;
        sourceToPlayFrom.minDistance = 0.1f;
        sourceToPlayFrom.maxDistance = 1000000;
        sourceToPlayFrom.loop = shouldLoop;

        switch (group)
        {
            case AudioGroup.Music:
                sourceToPlayFrom.outputAudioMixerGroup = musicGroup;
                break;
            case AudioGroup.SFX:
                sourceToPlayFrom.outputAudioMixerGroup = SFXGroup;
                break;
            case AudioGroup.VoiceLines:
                sourceToPlayFrom.outputAudioMixerGroup = voiceLinesGroup;
                break;
        }

        sourceToPlayFrom.Play();
        currentlyPlaying.Add(sourceToPlayFrom);

        if (!shouldLoop)
        {
            Destroy(audioObject, audio.length);
        }
    }

}