using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNewLevel();

            if (SceneController.instance != null)
            {
                SceneController.instance.NextLevel();
            }
            else
            {
                Debug.LogError(
                    "FinishPoint: No SceneController instance exists."
                );
            }
        }
    }

    void UnlockNewLevel()
    {
        int currentBuildIndex =
            SceneManager.GetActiveScene().buildIndex;

        int reachedIndex =
            PlayerPrefs.GetInt("ReachedIndex", 0);

        if (currentBuildIndex >= reachedIndex)
        {
            PlayerPrefs.SetInt(
                "ReachedIndex",
                currentBuildIndex + 1
            );

            PlayerPrefs.SetInt(
                "UnlockedLevel",
                PlayerPrefs.GetInt("UnlockedLevel", 1) + 1
            );

            PlayerPrefs.Save();
        }
    }
}