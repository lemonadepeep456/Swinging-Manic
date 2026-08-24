using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        int nextSceneIndex =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning(
                "There is no next scene in the Build Profile."
            );

            return;
        }

        SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError(
                "SceneController: Scene name is empty."
            );

            return;
        }

        SceneManager.LoadSceneAsync(sceneName);
    }
}