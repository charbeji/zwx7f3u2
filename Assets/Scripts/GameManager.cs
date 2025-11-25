using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text comboText;

    [Header("Card Sprites")]
    public Sprite[] cardFrontSprites;
    public Sprite cardBackSprite;

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

    private List<CardController> flippedCards = new List<CardController>();
    private bool comparing = false; // prevents overlap of comparison logic

    private int rows, columns;
    private int score = 0;
    private int turns = 0;
    private int comboCount = 0;

    private List<int> cardIDs = new List<int>();
    private List<bool> matchedStateList = new List<bool>();
    private bool loadingSavedGame = false;

    private void Start()
    {
        comboText.gameObject.SetActive(false);
        comboText.transform.localScale = Vector3.zero;

        // LOAD SYSTEM
        if (SaveSystem.HasSave())
        {
            loadingSavedGame = true;

            rows = SaveSystem.LoadRows();
            columns = SaveSystem.LoadColumns();
            score = SaveSystem.LoadScore();
            turns = SaveSystem.LoadTurns();
            comboCount = SaveSystem.LoadCombo();

            cardIDs = SaveSystem.LoadCardIDs();
            matchedStateList = SaveSystem.LoadMatchedStates();

            if (cardIDs == null || cardIDs.Count != rows * columns)
                loadingSavedGame = false;

            if (matchedStateList == null || matchedStateList.Count != rows * columns)
                matchedStateList = Enumerable.Repeat(false, rows * columns).ToList();
        }

        if (!loadingSavedGame)
        {
            rows = PlayerPrefs.GetInt("Rows", 2);
            columns = PlayerPrefs.GetInt("Columns", 2);

            GenerateCardIDs();
            matchedStateList = Enumerable.Repeat(false, rows * columns).ToList();
        }

        GenerateGrid();
        UpdateUI();
    }

    void GenerateCardIDs()
    {
        int total = rows * columns;
        int pairs = total / 2;

        cardIDs = new List<int>();

        for (int i = 0; i < pairs; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // shuffle
        for (int i = 0; i < cardIDs.Count; i++)
        {
            int r = Random.Range(0, cardIDs.Count);
            int temp = cardIDs[i];
            cardIDs[i] = cardIDs[r];
            cardIDs[r] = temp;
        }
    }

    void GenerateGrid()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        for (int i = 0; i < cardIDs.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridParent);
            CardController card = cardObj.GetComponent<CardController>();

            card.index = i;
            card.cardID = cardIDs[i];

            if (cardFrontSprites.Length > card.cardID)
                card.frontImage.sprite = cardFrontSprites[card.cardID];

            card.backImage.sprite = cardBackSprite;

            if (loadingSavedGame && matchedStateList[i])
                card.SetMatchedInstant();
            

        }
        gridParent.GetComponent<GridAutoResizer>().SetGrid(rows, columns);

    }

    public void CardFlipped(CardController card)
    {
        audioSource?.PlayOneShot(flipSound);
        flippedCards.Add(card);

        // Only check when 2 are flipped AND no comparison is happening
        if (!comparing && flippedCards.Count >= 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        comparing = true;

        // pick first 2 cards
        CardController c1 = flippedCards[0];
        CardController c2 = flippedCards[1];

        turns++;
        UpdateUI();

        yield return new WaitForSeconds(0.4f);

        if (c1.cardID == c2.cardID)
        {
            comboCount++;
            score += 1 + (comboCount - 1);

            c1.SetMatched();
            c2.SetMatched();

            matchedStateList[c1.index] = true;
            matchedStateList[c2.index] = true;

            audioSource?.PlayOneShot(matchSound);
            ShowCombo(comboCount);
        }
        else
        {
            comboCount = 0;

            

            c1.Flip();
            c2.Flip();

            audioSource?.PlayOneShot(mismatchSound);
        }

        // Remove only the two processed cards, leave the rest for continuous flipping
        flippedCards.RemoveAt(0);
        flippedCards.RemoveAt(0);

        SaveState();
        CheckGameOver();

        comparing = false;

        // If already 2 waiting, process next automatically
        if (flippedCards.Count >= 2)
            StartCoroutine(CheckMatch());
    }

    public void SaveState()
    {
        matchedStateList = new List<bool>();

        for (int i = 0; i < gridParent.childCount; i++)
        {
            var card = gridParent.GetChild(i).GetComponent<CardController>();
            matchedStateList.Add(card.IsMatched());
        }

        SaveSystem.SaveGame(rows, columns, score, turns, comboCount, cardIDs, matchedStateList);
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
            if (!t.GetComponent<CardController>().IsMatched())
                return;
        }

        audioSource?.PlayOneShot(gameOverSound);
        SaveSystem.ClearSave();

        Panel_GameOver.SetActive(true);
        gameOverScoreText.text = scoreText.text;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowCombo(int combo)
    {
        if (combo <= 1) return;

        comboText.text = "x" + combo;
        comboText.gameObject.SetActive(true);

        comboText.transform.localScale = Vector3.zero;
        comboText.alpha = 1;

        comboText.transform.DOScale(1.5f, 0.3f).SetEase(DG.Tweening.Ease.OutBack)
        .OnComplete(() =>
        {
            comboText.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() =>
            {
                comboText.gameObject.SetActive(false);
                comboText.alpha = 1;
            });
        });
    }
}
