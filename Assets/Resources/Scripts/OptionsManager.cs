using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown graphicsQualityDropdown;
    public Dropdown antialiasingDropdown;
    public Toggle vSyncToggle;
    public Slider musicVolumeSlider;

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

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }

    public void OnFullScreenToggle()
    {
        options.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
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
    }

    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = options.antialiasing = (int)Mathf.Pow(2f, antialiasingDropdown.value);
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
    }

    public void OnMusicVolumeChange()
    {
        audioManager.sounds[0].volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value;

        /*foreach (Sound sound in audioManager.sounds)
        {
            if (sound.soundName == "Music")
            {
                sound.volume = audioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = options.musicVolume = musicVolumeSlider.value;
            }
        }*/
    }

    public void SaveOptions()
    {

    }

    public void LoadSettings()
    {

    }
}
