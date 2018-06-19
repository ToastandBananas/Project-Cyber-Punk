using UnityEngine;

[System.Serializable]
public class Sound {

    public string soundName;
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 1.5f)] public float pitch = 1f;

    [Range(0f, 0.5f)] public float randomVolume;
    [Range(0f, 0.5f)] public float randomPitch;

    private AudioSource source;

    public void SetSource (AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f)); ;

        source.Play();
    }
}

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one AudioManager in the scene.");
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + " " + sounds[i].soundName);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource (_go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string _soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].soundName == _soundName)
            {
                sounds[i].Play();
                return;
            }
        }

        // No sound with _soundName
        Debug.LogWarning("AudioManager: Sound not found in list: " + _soundName);
    }
}
