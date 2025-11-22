using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFrontSprites;

    private int gridRows;
    private int gridColumns;
    private List<Card> cards = new List<Card>();
    private List<Card> selectedCards = new List<Card>();
    private int matchedPairs = 0;
    private bool canSelect = true;

    public System.Action OnGameWon { get; set; }

    public void InitializeGrid(int rows, int columns)
    {
        gridRows = rows;
        gridColumns = columns;

        // Validate grid size
        int totalCards = rows * columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError($"Grid size {rows}x{columns} is invalid. Total cards must be even for matching.");
            return;
        }

        // Check if we have enough card sprites
        int pairsNeeded = totalCards / 2;
        if (cardFrontSprites.Count < pairsNeeded)
        {
            Debug.LogWarning($"Not enough card sprites. Need {pairsNeeded}, have {cardFrontSprites.Count}. Using available sprites cyclically.");
        }

        // Clear existing cards
        foreach (Card card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
        selectedCards.Clear();
        matchedPairs = 0;
        canSelect = true;

        // Configure GridLayoutGroup
        ConfigureGridLayout();

        // Create and shuffle cards
        CreateCards(totalCards, pairsNeeded);
        ShuffleCards();
    }

    private void ConfigureGridLayout()
    {
        if (gridLayoutGroup == null) return;

        // Set grid layout to use preferred size
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = gridColumns;

        // Calculate cell size based on available space
        RectTransform containerRect = gridContainer as RectTransform;
        if (containerRect != null)
        {
            float containerWidth = containerRect.rect.width;
            float containerHeight = containerRect.rect.height;

            float cellWidth = containerWidth / gridColumns - gridLayoutGroup.spacing.x;
            float cellHeight = containerHeight / gridRows - gridLayoutGroup.spacing.y;

            float cellSize = Mathf.Min(cellWidth, cellHeight);
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        }
    }

    private void CreateCards(int totalCards, int pairsNeeded)
    {
        List<int> cardIds = new List<int>();

        // Create pairs of cards
        for (int i = 0; i < pairsNeeded; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        // Instantiate card objects
        for (int i = 0; i < totalCards; i++)
        {
            Card newCard = Instantiate(cardPrefab, gridContainer);
            int spriteIndex = cardIds[i] % cardFrontSprites.Count;
            newCard.Initialize(cardIds[i], cardFrontSprites[spriteIndex], cardBackSprite);
            newCard.OnCardSelected = OnCardSelected;
            cards.Add(newCard);
        }
    }

    private void ShuffleCards()
    {
        // Fisher-Yates shuffle
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    private void OnCardSelected(Card selectedCard)
    {
        if (!canSelect || selectedCard.IsMatched || selectedCard.IsFlipped)
            return;

        selectedCards.Add(selectedCard);

        if (selectedCards.Count == 2)
        {
            canSelect = false;
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        Card card1 = selectedCards[0];
        Card card2 = selectedCards[1];

        if (card1.CardId == card2.CardId)
        {
            // Match found
            card1.MatchCard();
            card2.MatchCard();
            matchedPairs++;

            selectedCards.Clear();
            canSelect = true;

            // Check if game is won
            if (matchedPairs == cards.Count / 2)
            {
                OnGameWon?.Invoke();
            }
        }
        else
        {
            // No match, flip back after delay
            Invoke(nameof(FlipBackCards), 1f);
        }
    }

    private void FlipBackCards()
    {
        if (selectedCards.Count == 2)
        {
            selectedCards[0].FlipBack();
            selectedCards[1].FlipBack();
            selectedCards.Clear();
        }
        canSelect = true;
    }

    public int GetGridRows() => gridRows;
    public int GetGridColumns() => gridColumns;
    public int GetTotalCards() => cards.Count;
    public int GetMatchedPairs() => matchedPairs;
}
