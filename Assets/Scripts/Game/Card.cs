using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;
    [SerializeField] private float flipDuration = 0.3f;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private Sprite cardFrontSprite;

    private int cardId;
    private bool isMatched;
    private bool isFlipped;
    private Sprite actualCardSprite;

    public int CardId => cardId;
    public bool IsMatched => isMatched;
    public bool IsFlipped => isFlipped;

    public System.Action<Card> OnCardSelected { get; set; }

    private void OnEnable()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnClick);
        }
    }

    private void OnDisable()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(OnClick);
        }
    }

    public void Initialize(int id, Sprite frontSprite, Sprite backSprite)
    {
        cardId = id;
        isMatched = false;
        isFlipped = false;
        actualCardSprite = frontSprite;
        cardBackSprite = backSprite;
        cardFrontSprite = frontSprite;

        // Show back side initially
        if (cardImage != null)
        {
            cardImage.sprite = cardBackSprite;
        }
    }

    public void FlipCard()
    {
        if (isMatched || isFlipped) return;

        isFlipped = true;

        cardButton.interactable = false;

        // Y-axis rotation animation for flip effect
        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2)
            .OnComplete(() =>
            {
                // Change sprite at midpoint of animation
                cardImage.sprite = cardFrontSprite;

                transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2)
                    .OnComplete(() =>
                    {
                        cardButton.interactable = true;
                        OnCardSelected?.Invoke(this);
                    });
            });
    }

    public void FlipBack()
    {
        if (isMatched) return;

        isFlipped = false;
        cardButton.interactable = false;

        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2)
            .OnComplete(() =>
            {
                cardImage.sprite = cardBackSprite;

                transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2)
                    .OnComplete(() =>
                    {
                        cardButton.interactable = true;
                    });
            });
    }

    public void MatchCard()
    {
        isMatched = true;
        cardButton.interactable = false;

        // Optional: Add a fade out animation on match
        cardImage.DOFade(0.5f, 0.5f);
    }

    public void SetInteractable(bool interactable)
    {
        if (cardButton != null)
        {
            cardButton.interactable = interactable && !isMatched;
        }
    }

    private void OnClick()
    {
        if (!isFlipped && !isMatched)
        {
            FlipCard();
        }
    }
}
