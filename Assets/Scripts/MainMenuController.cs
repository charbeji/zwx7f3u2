using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject difficultyPanel;

    [Header("Main Menu Buttons")]
    public GameObject continueButton;

    private void Start()
    {
        difficultyPanel.SetActive(false);

        // Hide Continue if no save
        string hasSave = PlayerPrefs.GetString("HasSave", "0");
        if (hasSave == "0")
            continueButton.SetActive(false);
    }

    // --- Main Menu buttons ---
    public void OnPlayClicked()
    {
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void OnContinueClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnBackClicked()
    {
        difficultyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // --- Difficulty Buttons ---
    public void SetVeryEasy()
    {
        PlayerPrefs.SetString("HasSave", "0");
        SetDifficulty(2, 2);
    }

    public void SetEasy()
    {
        PlayerPrefs.SetString("HasSave", "0");
        SetDifficulty(2, 3);
    }

    public void SetMedium()
    {
        PlayerPrefs.SetString("HasSave", "0");
        SetDifficulty(3, 4);
    }

    public void SetHard()
    {
        PlayerPrefs.SetString("HasSave", "0");
        SetDifficulty(4, 4);
    }

    public void SetVeryHard()
    {
        PlayerPrefs.SetString("HasSave", "0");
        SetDifficulty(5, 6); // 30 cards max
    }

    // Saves values & starts game
    private void SetDifficulty(int rows, int cols)
    {
        PlayerPrefs.SetInt("Rows", rows);
        PlayerPrefs.SetInt("Columns", cols);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}
