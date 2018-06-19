using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	public void ResumeGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
