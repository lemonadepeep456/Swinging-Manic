using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        int firstLevelIndex = 1;

        if (firstLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError(
                "Level1 is not included in the active Build Profile."
            );

            return;
        }

        Debug.Log("Loading Level1...");

        SceneManager.LoadSceneAsync(firstLevelIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        Application.Quit();
    }
}