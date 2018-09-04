using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class OptionsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown graphicsQualityDropdown;
    public Dropdown antialiasingDropdown;
    public Toggle vSyncToggle;
    public Slider musicVolumeSlider;
    public Button revertToDefaultsButton;

    //public AudioSource musicSource;
    public AudioManager audioManager;

    public Resolution[] resolutions;

    public Options options;

    void OnEnable()
    {
        options = new Options();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullScreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        graphicsQualityDropdown.onValueChanged.AddListener(delegate { OnGraphicsQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        vSyncToggle.onValueChanged.AddListener(delegate { OnVSyncToggle(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        revertToDefaultsButton.onClick.AddListener(delegate { OnRevertToDefaultsButtonClick(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        LoadOptions();
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

    public void OnGraphicsQualityChange()
    {
        options.graphicsQuality = graphicsQualityDropdown.value;
        QualitySettings.SetQualityLevel(options.graphicsQuality);

        if (QualitySettings.vSyncCount == 1)
        {
            vSyncToggle.isOn = true;
            options.vSync = 1;
        }
        else if (QualitySettings.vSyncCount == 0)
        {
            vSyncToggle.isOn = false;
            options.vSync = 0;
        }

        options.antialiasing = antialiasingDropdown.value = QualitySettings.antiAliasing;

        SaveOptions();
    }

    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = (int)Mathf.Pow(2f, antialiasingDropdown.value);
        options.antialiasing = antialiasingDropdown.value;

        SaveOptions();
    }

    public void OnVSyncToggle()
    {
        if (vSyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
            options.vSync = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            options.vSync = 0;
        }

        SaveOptions();
    }

    public void OnMasterVolumeChange()
    {

    }

    public void OnMusicVolumeChange()
    {
        if (audioManager.transform.childCount > 0)
            audioManager.sounds[0].volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value;

        /*foreach (Sound sound in audioManager.sounds)
        {
            if (sound.soundName == "Music")
            {
                sound.volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value;
            }
        }*/

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

        QualitySettings.SetQualityLevel(5);
        graphicsQualityDropdown.value = options.graphicsQuality = 5;

        antialiasingDropdown.value = 3;
        QualitySettings.antiAliasing = (int)Mathf.Pow(2f, antialiasingDropdown.value);
        options.antialiasing = antialiasingDropdown.value;

        vSyncToggle.isOn = true;
        QualitySettings.vSyncCount = options.vSync = 1;

        audioManager.sounds[0].volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value = 0.3f;

        SaveOptions();
    }

    public void SaveOptions()
    {
        string jsonData = JsonUtility.ToJson(options, true);
        File.WriteAllText(Application.persistentDataPath + "/gameOptions.json", jsonData);
    }

    public void LoadOptions()
    {
        options = JsonUtility.FromJson<Options>(File.ReadAllText(Application.persistentDataPath + "/gameOptions.json"));

        fullscreenToggle.isOn = options.fullscreen;
        Screen.fullScreen = options.fullscreen;
        resolutionDropdown.value = options.resolutionIndex;
        graphicsQualityDropdown.value = options.graphicsQuality;
        antialiasingDropdown.value = options.antialiasing;
        if (options.vSync == 0)
            vSyncToggle.isOn = false;
        else if (options.vSync == 1)
            vSyncToggle.isOn = true;
        musicVolumeSlider.value = options.musicVolume;

        resolutionDropdown.RefreshShownValue();
    }
}
