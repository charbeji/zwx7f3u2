using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject setupPanel;

    [Header("Setup Inputs")]
    public TMP_InputField rowsInput;
    public TMP_InputField columnsInput;
    public TMP_Text errorText;

    [Header("Main Menu Buttons")]
    public GameObject continueButton;

    private void Start()
    {
        setupPanel.SetActive(false);

        // Disable Continue button if no save exists
        string hasSave = PlayerPrefs.GetString("HasSave", "0");
        if (hasSave == "0" && continueButton != null)
        {
            continueButton.SetActive(false);
        }
    }

    // --- Main Menu Buttons ---
    public void OnPlayClicked()
    {
        mainMenuPanel.SetActive(false);
        setupPanel.SetActive(true);
    }

    public void OnContinueClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    // --- Setup Buttons ---
    public void OnStartGameClicked()
    {
        if (!int.TryParse(rowsInput.text, out int rows) ||
            !int.TryParse(columnsInput.text, out int cols))
        {
            errorText.text = "Rows and Columns must be numeric.";
            return;
        }

        int total = rows * cols;
        if (rows == 0 || cols == 0)
        {
            errorText.text = "Rows and Columns cannot be 0.";
            return;
        }
        if (total > 30)
        {
            errorText.text = "Total cards cannot exceed 30.";
            return;
        }

        if (total % 2 != 0)
        {
            errorText.text = "Total cards must be an EVEN number.";
            return;
        }

        PlayerPrefs.SetInt("Rows", rows);
        PlayerPrefs.SetInt("Columns", cols);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }

    public void OnBackClicked()
    {
        setupPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        errorText.text = "";
    }
}
