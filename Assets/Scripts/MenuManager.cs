using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [SerializeField] string buttonHoverSound = "ButtonHover";
    [SerializeField] string buttonPressSound = "ButtonPress";

    AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No audio manager found.");
        }
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }

    public void OnMouseOver()
    {
        audioManager.PlaySound(buttonHoverSound);
    }

    public void OnMouseDown()
    {
        audioManager.PlaySound(buttonPressSound);
    }
}
