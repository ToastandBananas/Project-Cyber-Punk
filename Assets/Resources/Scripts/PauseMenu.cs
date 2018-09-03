using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameMaster gameMaster;
    AudioManager audioManager;
    
    public GameObject quitConfirmation;

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

    public void QuitConfirmation()
    {
        quitConfirmation.SetActive(true);
    }

    public void Quit()
    {
        // Might change this to quit to main menu, but for now will just quit game alltogether
        Application.Quit();
    }

    public void NoQuit()
    {
        quitConfirmation.SetActive(false);
    }
}
