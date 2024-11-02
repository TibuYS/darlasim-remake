using UnityEngine;

public class SlowMotionScript : MonoBehaviour
{
    public static SlowMotionScript instance;
    public float slowMotionTimeScale = 0.5f;
    public bool slowMotionEnabled = false;

    [System.Serializable]
    public class AudioSourceData
    {
        public AudioSource audioSource;
        public float defaultPitch;
    }

    AudioSourceData[] audioSources;

    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        
    }

    public void SlowMotionEffect(bool enabled)
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        audioSources = new AudioSourceData[audios.Length];

        for (int i = 0; i < audios.Length; i++)
        {
            AudioSourceData tmpData = new AudioSourceData();
            tmpData.audioSource = audios[i];
            tmpData.defaultPitch = audios[i].pitch;
            audioSources[i] = tmpData;
        }

        Time.timeScale = enabled ? slowMotionTimeScale : 1;
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].audioSource)
            {
                audioSources[i].audioSource.pitch = 1 * Time.timeScale;
            }
        }
    }
}