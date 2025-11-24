using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    
    private List<int> cardIDs = new List<int>();

    private void Start()
    {
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
        yield return new WaitForSeconds(0.5f);

        CardController first = flippedCards[0];
        CardController second = flippedCards[1];

        if (first.cardID == second.cardID)
        {
            score++;
            
            first.SetMatched();
            second.SetMatched();

            audioSource?.PlayOneShot(matchSound);

            
        }
        else
        {
            
            first.Flip();
            second.Flip();

            // Punch animation
            first.PunchAnimation();
            second.PunchAnimation();

            audioSource?.PlayOneShot(mismatchSound);
        }

        flippedCards.Clear();
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
}
