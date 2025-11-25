using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    public int cardID;
    public int index; // position on grid
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
        if (isFlipped || isMatched)
            return;

        Flip();
        gameManager.CardFlipped(this);
    }

    public void Flip()
    {
        isFlipped = !isFlipped;

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
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
    }

    public void SetMatchedInstant()
    {
        isMatched = true;
        isFlipped = true;

        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);

        transform.localScale = Vector3.zero;
    }


    public bool IsMatched() => isMatched;
}
