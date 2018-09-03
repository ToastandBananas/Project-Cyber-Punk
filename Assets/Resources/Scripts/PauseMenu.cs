using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameMaster gameMaster;
    AudioManager audioManager;
    
    public GameObject quitConfirmation;
    public GameObject optionsMenu;

    private void Start()
    {
        gameMaster = GameMaster.gm;
        audioManager = AudioManager.instance;
    }

    public void PlayButtonHoverSound()
    {
        audioManager.PlaySound("ButtonHover");
    }

    public void ResumeGame()
    {
        gameMaster.TogglePauseMenu();
    }

    public void ShowOptionsMenu()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        gameObject.SetActive(true);
    }

    public void RevertOptionsToDefaults()
    {
        print("Reverting to default options");
    }

    public void ShowQuitConfirmation()
    {
        quitConfirmation.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        // Might change this to quit to main menu, but for now will just quit game altogether
        print("Quitting Game");
        Application.Quit();
    }

    public void NoQuit()
    {
        quitConfirmation.SetActive(false);
        gameObject.SetActive(true);
    }
}
