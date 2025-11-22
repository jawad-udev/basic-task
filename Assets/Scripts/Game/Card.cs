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
    [SerializeField] private Image frontImage;
    [SerializeField] private GameObject frontImageObject;

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

    private void OnDestroy()
    {
        // Kill all DOTween animations on this transform to prevent warnings
        DOTween.Kill(transform);
        if (cardImage != null)
        {
            DOTween.Kill(cardImage);
        }
        if (frontImage != null)
        {
            DOTween.Kill(frontImage);
        }
    }

    public void Initialize(int id, Sprite frontSprite, Sprite backSprite)
    {
        cardId = id;
        isMatched = false;
        isFlipped = true; // Start as flipped (showing front)
        actualCardSprite = frontSprite;
        cardBackSprite = backSprite;
        cardFrontSprite = frontSprite;

        // Show front side initially
        if (frontImageObject != null)
        {
            frontImageObject.SetActive(true);
        }
        if (frontImage != null)
        {
            frontImage.sprite = cardFrontSprite;
        }
        if (cardImage != null)
        {
            cardImage.sprite = cardBackSprite;
        }
    }

    public void ResetToBack()
    {
        isFlipped = false;
        if (frontImageObject != null)
        {
            frontImageObject.SetActive(false);
        }
    }

    public void FlipCard()
    {
        if (isMatched || isFlipped)
        {
            Debug.LogWarning($"Cannot flip card - isMatched: {isMatched}, isFlipped: {isFlipped}");
            return;
        }

        Debug.Log($"Flipping card ID: {cardId}");
        isFlipped = true;

        cardButton.interactable = false;

        // Y-axis rotation animation for flip effect
        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2)
            .OnComplete(() =>
            {
                // Change sprite at midpoint of animation
                if (frontImageObject != null && frontImage != null)
                {
                    frontImageObject.SetActive(true);
                    frontImage.sprite = cardFrontSprite;
                    Debug.Log($"Card {cardId} sprite changed to front");
                }

                transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2)
                    .OnComplete(() =>
                    {
                        cardButton.interactable = true;
                        Debug.Log($"Card {cardId} animation complete - Invoking OnCardSelected");
                        // Invoke card selection callback
                        if (OnCardSelected != null)
                        {
                            Debug.Log($"OnCardSelected callback is assigned, invoking for card {cardId}");
                            OnCardSelected.Invoke(this);
                        }
                        else
                        {
                            Debug.LogError($"OnCardSelected callback is NULL for card {cardId}!");
                        }
                    });
            });
    }

    public void FlipBack()
    {
        if (isMatched) return;

        cardButton.interactable = false;

        Debug.Log($"Flipping back card ID: {cardId}");

        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2)
            .OnComplete(() =>
            {
                // Hide front and show back
                if (frontImageObject != null)
                {
                    frontImageObject.SetActive(false);
                    Debug.Log($"Card {cardId} front hidden");
                }

                transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2)
                    .OnComplete(() =>
                    {
                        isFlipped = false; // Set to false after animation completes
                        cardButton.interactable = true;
                        Debug.Log($"Card {cardId} flipped back complete - isFlipped set to false");
                    });
            });
    }

    public void MatchCard()
    {
        isMatched = true;
        cardButton.interactable = false;

        Debug.Log($"Card {cardId} matched!");

        // Add a fade out animation on match
        if (frontImage != null)
        {
            frontImage.DOFade(0.5f, 0.5f);
        }
        if (cardImage != null)
        {
            cardImage.DOFade(0.5f, 0.5f);
        }
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
        Debug.Log($"OnClick called for card {cardId} - isFlipped: {isFlipped}, isMatched: {isMatched}");
        if (!isFlipped && !isMatched)
        {
            FlipCard();
        }
        else
        {
            Debug.LogWarning($"Click rejected for card {cardId} - isFlipped: {isFlipped}, isMatched: {isMatched}");
        }
    }
}
