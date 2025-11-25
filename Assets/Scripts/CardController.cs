using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    public int cardID;
    public int index; // position in the board (used for save/load)
    public Image frontImage;
    public Image backImage;

    private bool isFlipped = false;
    private bool isMatched = false;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        backImage.gameObject.SetActive(true);
        frontImage.gameObject.SetActive(false);
    }

    public void OnCardClicked()
    {
        // keep per-card protection but allow continuous flipping behaviour via GameManager
        if (isFlipped || isMatched)
            return;

        Flip();
        gameManager.CardFlipped(this);
    }

    public void Flip()
    {
        isFlipped = !isFlipped;

        // DOTween flip animation
        transform.DORotate(new Vector3(0, 90, 0), 0.2f).OnComplete(() =>
        {
            frontImage.gameObject.SetActive(isFlipped);
            backImage.gameObject.SetActive(!isFlipped);
            transform.DORotate(Vector3.zero, 0.2f);
        });
    }

    public void SetMatched()
    {
        isMatched = true;

        // Scale out animation for matched cards
        transform.DOScale(Vector3.zero, 0.3f).SetEase(DG.Tweening.Ease.InBack);
    }

    // Instant matched state used when loading a save (no animation)
    public void SetMatchedInstant()
    {
        isMatched = true;
        isFlipped = true;
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        transform.localScale = Vector3.zero; // hide instantly (same visual as SetMatched)
    }

    // Punch animation for mismatch 
    public void PunchAnimation()
    {
        transform.DOPunchPosition(new Vector3(20, 0, 0), 0.3f, 10, 1);
        // also shake rotation slightly
        transform.DOPunchRotation(new Vector3(0, 0, 10), 0.3f, 10, 1);
    }

    public bool IsMatched() => isMatched;
}
