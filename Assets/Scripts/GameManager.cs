using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text comboText;  

    [Header("Card Sprites")]
    public Sprite[] cardFrontSprites; // 15 sprites
    public Sprite cardBackSprite;     // 1 back sprite

    [Header("References")]
    public GameObject cardPrefab;
    public Transform gridParent;
    public TMP_Text scoreText;
    public TMP_Text gameOverScoreText;
    public TMP_Text turnsText;
    public AudioSource audioSource;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;
    public GameObject Panel_GameOver;
    [HideInInspector] public bool IsProcessing = false;

    private List<CardController> flippedCards = new List<CardController>();
    private int rows, columns;
    private int score = 0;
    private int turns = 0;
    private int comboCount = 0; // Tracks consecutive matches

    private List<int> cardIDs = new List<int>();

    private void Start()
    {
        comboText.gameObject.SetActive(false);
        comboText.transform.localScale = Vector3.zero;
        rows = PlayerPrefs.GetInt("Rows", 2);
        columns = PlayerPrefs.GetInt("Columns", 2);
        GenerateCardIDs();
        GenerateGrid();
        UpdateUI();
    }

    // Generate pairs of cards and shuffle
    void GenerateCardIDs()
    {
        int totalCards = rows * columns;
        cardIDs.Clear();
        int pairs = totalCards / 2;

        for (int i = 0; i < pairs; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // Shuffle
        for (int i = 0; i < cardIDs.Count; i++)
        {
            int temp = cardIDs[i];
            int randomIndex = Random.Range(0, cardIDs.Count);
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }
    }

    void GenerateGrid()
    {
       
        for (int i = 0; i < cardIDs.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridParent);
            CardController card = cardObj.GetComponent<CardController>();
            card.cardID = cardIDs[i];
        
            // Assign front/back images
            if (cardFrontSprites != null && cardFrontSprites.Length > card.cardID)
            {
                card.frontImage.sprite = cardFrontSprites[card.cardID];
            }
            if (cardBackSprite != null)
            {
                card.backImage.sprite = cardBackSprite;
            }

        }
    }

    public void CardFlipped(CardController card)
    {
        flippedCards.Add(card);

        // Play flip sound
        audioSource?.PlayOneShot(flipSound);

        if (flippedCards.Count == 2)
        {
            turns++;
            UpdateUI();
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        IsProcessing = true;

        // Wait a short time so player sees the flipped cards
        yield return new WaitForSeconds(0.5f);

        CardController first = flippedCards[0];
        CardController second = flippedCards[1];

        if (first.cardID == second.cardID)
        {
            // ✅ Match found
            comboCount++; // Increase combo

            int scoreIncrement = 1 + (comboCount - 1); // Base 1 + combo bonus
            score += scoreIncrement;

            first.SetMatched();
            second.SetMatched();

            // Play match sound
            audioSource?.PlayOneShot(matchSound);

            // Show animated combo if combo >1
            ShowCombo(comboCount);
        }
        else
        {
            // ❌ Mismatch resets combo
            comboCount = 0;

            // Punch animation for mismatch
            first.PunchAnimation();
            second.PunchAnimation();

            // Flip cards back
            first.Flip();
            second.Flip();

            // Play mismatch sound
            audioSource?.PlayOneShot(mismatchSound);
        }

        flippedCards.Clear(); // Clear immediately for continuous flipping
        IsProcessing = false;

        UpdateUI();
        CheckGameOver();
    }



    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        turnsText.text = "Turns: " + turns;
       
    }

    void CheckGameOver()
    {
        foreach (Transform t in gridParent)
        {
            CardController card = t.GetComponent<CardController>();
            if (!card.IsMatched())
                return;
        }

        // Game over logic
        audioSource?.PlayOneShot(gameOverSound);
        Debug.Log("Game Over! Final Score: " + score + " Turns: " + turns);

        
        Panel_GameOver.SetActive(true);
        gameOverScoreText.text= scoreText.text;
    }
    public void ResetGame()
    {
        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private void ShowCombo(int combo)
    {
        if (combo <= 1)
            return;

        comboText.text = "x" + combo;
        comboText.gameObject.SetActive(true);

        // Reset scale & alpha
        comboText.transform.localScale = Vector3.zero;
        comboText.alpha = 1;

        // Animate scale punch
        comboText.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Fade out after 0.5s
                comboText.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() =>
                {
                    comboText.gameObject.SetActive(false);
                    comboText.alpha = 1; // Reset alpha for next time
                });
            });
    }

}
