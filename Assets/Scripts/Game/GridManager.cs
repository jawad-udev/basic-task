using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFrontSprites;
    [SerializeField] private float spacingX = 10f;
    [SerializeField] private float spacingY = 10f;

    private CustomGridLayout customGridLayout;
    private ScoreManager scoreManager;

    private int gridRows;
    private int gridColumns;
    private List<Card> cards = new List<Card>();
    private List<Card> selectedCards = new List<Card>();
    private List<Card> cardsToFlipBack = new List<Card>();
    private Queue<List<Card>> comparisonQueue = new Queue<List<Card>>();
    private int matchedPairs = 0;
    private bool isComparing = false;

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

        // Create and shuffle cards first
        CreateCards(totalCards, pairsNeeded);
        ShuffleCards();

        // Configure grid layout after cards are created
        ConfigureGridLayout();
    }

    private void ConfigureGridLayout()
    {
        if (customGridLayout == null)
        {
            customGridLayout = gridContainer.GetComponent<CustomGridLayout>();
            if (customGridLayout == null)
            {
                Debug.LogError("CustomGridLayout not found on grid container!");
                return;
            }
        }

        if (scoreManager == null)
        {
            float containerWidth = containerRect.rect.width;
            float containerHeight = containerRect.rect.height;

            float cellWidth = containerWidth / gridColumns - gridLayoutGroup.spacing.x;
            float cellHeight = containerHeight / gridRows - gridLayoutGroup.spacing.y;

        // Set grid dimensions and spacing
        customGridLayout.SetGridDimensions(gridColumns, gridRows);
        customGridLayout.SetSpacing(spacingX, spacingY);
        customGridLayout.SetAutoFitCells(true);

        // Reset all cards to back side after 3 seconds
        Invoke(nameof(ResetAllCardsToBack), 3f);
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
            newCard.OnCardSelected += OnCardSelected;
            cards.Add(newCard);
        }
    }

    private void ShuffleCards()
    {
        // Fisher-Yates shuffle on the list
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        // Reorder GameObjects in hierarchy to match shuffled list
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(i);
            Debug.Log($"Card {cards[i].CardId} moved to position {i}");
        }
    }

    private void OnCardSelected(Card selectedCard)
    {
        // Prevent selecting matched cards
        if (selectedCard.IsMatched)
        {
            Debug.LogWarning($"Card {selectedCard.CardId} already matched, cannot select");
            return;
        }

        // Prevent adding the same card twice to the selection
        if (selectedCards.Contains(selectedCard))
        {
            Debug.LogWarning($"Card {selectedCard.CardId} already in selection, ignoring duplicate click");
            return;
        }

        selectedCards.Add(selectedCard);
        Debug.Log($"Card selected: ID={selectedCard.CardId}, Total selected: {selectedCards.Count}");

        // Once 2 cards are selected, queue for comparison
        if (selectedCards.Count == 2)
        {
            Debug.Log($"Two cards selected - Card1 ID: {selectedCards[0].CardId}, Card2 ID: {selectedCards[1].CardId}");
            // Queue this pair for comparison
            List<Card> pairToCompare = new List<Card>(selectedCards);
            comparisonQueue.Enqueue(pairToCompare);
            selectedCards.Clear();

            // If not currently comparing, start the next comparison
            if (!isComparing)
            {
                ProcessNextComparison();
            }
        }
    }

    private void ProcessNextComparison()
    {
        if (comparisonQueue.Count == 0)
        {
            isComparing = false;
            return;
        }

        isComparing = true;
        selectedCards = comparisonQueue.Dequeue();
        CheckForMatch();
    }

    private void CheckForMatch()
    {
        Card card1 = selectedCards[0];
        Card card2 = selectedCards[1];

        Debug.Log($"Checking match - Card1 ID: {card1.CardId}, Card2 ID: {card2.CardId}");

        if (card1.CardId == card2.CardId)
        {
            // Match found
            Debug.Log($"MATCH FOUND! Cards with ID {card1.CardId} matched!");
            card1.MatchCard();
            card2.MatchCard();
            matchedPairs++;

            selectedCards.Clear();

            // Check if game is won
            if (matchedPairs == cards.Count / 2)
            {
                OnGameWon?.Invoke();
                isComparing = false;
            }
        }
        else
        {
            // No match, flip back after delay
            Invoke(nameof(FlipBackCards), 1f);
        }
    }

    private void RemoveMatchedCards()
    {
        // Remove all matched cards from the grid
        int removedCount = 0;
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i].IsMatched)
            {
                Debug.Log($"Removing matched card at index {i}");
                Destroy(cards[i].gameObject);
                cards.RemoveAt(i);
                removedCount++;
            }
        }
        Debug.Log($"Removed {removedCount} matched cards. Remaining cards: {cards.Count}");

        // Process next comparison in queue, or allow new selections
        ProcessNextComparison();
    }

    private void FlipBackCards()
    {
        Debug.Log($"FlipBackCards called - cardsToFlipBack count: {cardsToFlipBack.Count}");
        if (cardsToFlipBack.Count == 2)
        {
            Debug.Log($"Flipping back card 1: {cardsToFlipBack[0].CardId} and card 2: {cardsToFlipBack[1].CardId}");
            cardsToFlipBack[0].FlipBack();
            cardsToFlipBack[1].FlipBack();
            cardsToFlipBack.Clear();
            Debug.Log("Cards flipped back, ready for new selection");
            // Process next comparison in queue
            ProcessNextComparison();
        }
        else
        {
            Debug.LogWarning($"Cards to flip back count is not 2, it's {cardsToFlipBack.Count}");
            cardsToFlipBack.Clear();
            ProcessNextComparison();
        }
    }

    private void ResetAllCardsToBack()
    {
        Debug.Log("Resetting all cards to back side");
        foreach (Card card in cards)
        {
            card.ResetToBack();
        }
    }

    public int GetGridRows() => gridRows;
    public int GetGridColumns() => gridColumns;
    public int GetTotalCards() => cards.Count;
    public int GetMatchedPairs() => matchedPairs;
}
