using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Button revertToDefaultsButton;
    
    public AudioMixer masterAudioMixer;
    AudioManager audioManager;

    public Resolution[] resolutions;

    public Options options;

    public static OptionsManager instance;

    void Start()
    {
        audioManager = AudioManager.instance;
        LoadOptions();
    }

    void OnEnable()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }

        options = new Options();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullScreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        revertToDefaultsButton.onClick.AddListener(delegate { OnRevertToDefaultsButtonClick(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }

    public void OnFullScreenToggle()
    {
        options.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;

        SaveOptions();
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        options.resolutionIndex = resolutionDropdown.value;

        SaveOptions();
    }

    public void OnMasterVolumeChange()
    {
        masterAudioMixer.SetFloat("MasterVolume", masterVolumeSlider.value);
        options.masterVolume = masterVolumeSlider.value;

        SaveOptions();
    }

    public void OnMusicVolumeChange()
    {
        if (audioManager.transform.childCount > 0)
            audioManager.sounds[0].volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value;

        SaveOptions();
    }

    public void OnRevertToDefaultsButtonClick()
    {
        fullscreenToggle.isOn = true;
        options.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;

        Screen.SetResolution(1920, 1080, true);
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                options.resolutionIndex = i;
                resolutionDropdown.value = i;
            }
        }

        masterVolumeSlider.value = -16.0f;
        masterAudioMixer.SetFloat("MasterVolume", masterVolumeSlider.value);
        options.masterVolume = masterVolumeSlider.value;

        audioManager.sounds[0].volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value = 1.2f;

        SaveOptions();
    }

    public void SaveOptions()
    {
        string jsonData = JsonUtility.ToJson(options, true);
        File.WriteAllText(Application.persistentDataPath + "/gameOptions.json", jsonData);
    }

    public void LoadOptions()
    {
        if (File.Exists(Application.persistentDataPath + "/gameOptions.json"))
        {
            options = JsonUtility.FromJson<Options>(File.ReadAllText(Application.persistentDataPath + "/gameOptions.json"));

            fullscreenToggle.isOn = options.fullscreen;
            Screen.fullScreen = options.fullscreen;

            resolutionDropdown.value = options.resolutionIndex;
            Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
            resolutionDropdown.RefreshShownValue();

            masterVolumeSlider.value = options.masterVolume;
            masterAudioMixer.SetFloat("MasterVolume", masterVolumeSlider.value);
            
            musicVolumeSlider.value = options.musicVolume;
            audioManager.sounds[0].volume = options.musicVolume;
            audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume;
        }
    } 
}
