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
            return;
        }

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
                }

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

        cardButton.interactable = false;

        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2)
            .OnComplete(() =>
            {
                // Hide front and show back
                if (frontImageObject != null)
                {
                    frontImageObject.SetActive(false);
                }

                transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2)
                    .OnComplete(() =>
                    {
                        isFlipped = false; // Set to false after animation completes
                        cardButton.interactable = true;
                    });
            });
    }

    public void MatchCard()
    {
        isMatched = true;
        cardButton.interactable = false;

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
        if (!isFlipped && !isMatched)
        {
            FlipCard();
        }
    }
}
