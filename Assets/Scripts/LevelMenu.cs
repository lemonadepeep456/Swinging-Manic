using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;

    private void Awake()
    {
        if (levelButtons == null)
        {
            Debug.LogError("LevelMenu: Level Buttons object is not assigned.");
            return;
        }

        ButtonsToArray();

        int unlockedLevel =
            PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].interactable = false;
            }
        }

        for (int i = 0; i < unlockedLevel && i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].interactable = true;
            }
        }
    }

    public void OpenLevel(int levelID)
    {
        string levelName = "Level" + levelID;

        Debug.Log("Loading scene: " + levelName);

        SceneManager.LoadScene(levelName);
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;

        buttons = new Button[childCount];

        for (int i = 0; i < childCount; i++)
        {
            buttons[i] =
                levelButtons.transform
                .GetChild(i)
                .GetComponent<Button>();

            if (buttons[i] == null)
            {
                Debug.LogWarning(
                    "LevelMenu: Child " +
                    levelButtons.transform.GetChild(i).name +
                    " does not have a Button component."
                );
            }
        }
    }
}