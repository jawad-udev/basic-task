using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFrontSprites;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private MovesManager movesManager;
    [SerializeField] private float spacingX = 10f;
    [SerializeField] private float spacingY = 10f;

    private CustomGridLayout customGridLayout;

    private int gridRows;
    private int gridColumns;
    private List<Card> cards = new List<Card>();
    private List<Card> selectedCards = new List<Card>();
    private List<Card> cardsToFlipBack = new List<Card>();
    private Queue<List<Card>> comparisonQueue = new Queue<List<Card>>();
    private int matchedPairs = 0;
    private bool isComparing = false;

    public System.Action OnGameWon { get; set; }
    public System.Action OnGameOver { get; set; }

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
            scoreManager = FindAnyObjectByType<ScoreManager>();
        }

        scoreManager.ResetRound();

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
        }
    }

    private void OnCardSelected(Card selectedCard)
    {
        // Prevent selecting matched cards
        if (selectedCard.IsMatched)
        {
            return;
        }

        // Prevent adding the same card twice to the selection
        if (selectedCards.Contains(selectedCard))
        {
            return;
        }

        // Check if moves are available
        if (movesManager != null && movesManager.IsGameOver)
        {
            return;
        }

        selectedCards.Add(selectedCard);

        // Once 2 cards are selected, queue for comparison
        if (selectedCards.Count == 2)
        {
            // Decrement moves when a pair is selected
            if (movesManager != null)
            {
                movesManager.DecrementMoves();
            }

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

        if (card1.CardId == card2.CardId)
        {
            // Match found
            card1.MatchCard();
            card2.MatchCard();
            matchedPairs++;

            // Add score and combo for matching
            if (scoreManager != null)
            {
                scoreManager.AddMatchPoints();
                Services.UserService.AddMatch();
                Services.AudioService.PlayMatchSound();
            }

            // Remove matched cards with delay for visual feedback
            // Keep isComparing = true until cards are removed
            Invoke(nameof(RemoveMatchedCards), 0.5f);

            selectedCards.Clear();
        }
        else
        {
            Services.AudioService.PlayMissmatchSound();
            // No match, save these cards for flip back and clear selection for new matches

            // Reset combo on mismatch
            if (scoreManager != null)
            {
                scoreManager.ResetCombo();
            }

            cardsToFlipBack.Clear();
            cardsToFlipBack.Add(card1);
            cardsToFlipBack.Add(card2);
            selectedCards.Clear();
            Invoke(nameof(FlipBackCards), 1f);
        }
    }

    private void RemoveMatchedCards()
    {
        // Remove all matched cards from the grid
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i].IsMatched)
            {
                Destroy(cards[i].gameObject);
                cards.RemoveAt(i);
            }
        }

        // Check if game is won after removing matched cards
        if (cards.Count == 0)
        {
            if (scoreManager != null)
            {
                scoreManager.FinishGame();
            }
            OnGameWon?.Invoke();
            isComparing = false;
        }
        else
        {
            // Process next comparison in queue, or allow new selections
            ProcessNextComparison();
        }
    }

    private void FlipBackCards()
    {
        if (cardsToFlipBack.Count == 2)
        {
            cardsToFlipBack[0].FlipBack();
            cardsToFlipBack[1].FlipBack();
            cardsToFlipBack.Clear();
            // Process next comparison in queue
            ProcessNextComparison();
        }
        else
        {
            cardsToFlipBack.Clear();
            ProcessNextComparison();
        }
    }

    private void ResetAllCardsToBack()
    {
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
