using BigTwo.Bot;
using BigTwo.Bot.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigTwo
{
    public class PlayerBot : Player, EventListener<EventGameState>, EventListener<EventPlayerTurn>
    {
        protected Brain<PlayerBot> Brain { get; set; }

        protected virtual void OnEnable()
        {
            EventManager.AddListener<EventGameState>(this);
            EventManager.AddListener<EventPlayerTurn>(this);
        }

        protected virtual void OnDisable()
        {
            EventManager.RemoveListener<EventGameState>(this);
            EventManager.RemoveListener<EventPlayerTurn>(this);
        }

        protected virtual void Update()
        {
            Brain?.Update();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void InitializeHand(Card[] cards)
        {
            base.InitializeHand(cards);

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].ToggleVisibility(false);
            }
        }

        public CardCombination GetCardCombination(bool firstTurn = false)
        {
            CardCombination cardCombination = new CardCombination();

            Card[] sortedCards = Hand.ListOfCard.OrderBy(card => card.CardData.Rank + card.CardData.Suit / (Constant.CARD_SUIT_MAX + 1)).ToArray();
            Dictionary<int, List<int>> dictionaryOfCardRank = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> dictionaryOfCardSuit = new Dictionary<int, List<int>>();

            List<int> listOfCardRank = new List<int>();
            List<int> listOfCardSuit = new List<int>();

            List<int> listOfCardIndex;

            for (int i = 0; i < sortedCards.Length; i++)
            {
                CardData cardData = sortedCards[i].CardData;
                if (dictionaryOfCardRank.TryGetValue(cardData.Rank, out listOfCardIndex))
                {
                    listOfCardIndex.Add(i);
                }
                else
                {
                    listOfCardRank.Add(cardData.Rank);
                    dictionaryOfCardRank.Add(cardData.Rank, new List<int>() { i });
                }

                if (dictionaryOfCardSuit.TryGetValue(cardData.Suit, out listOfCardIndex))
                {
                    listOfCardIndex.Add(i);
                }
                else
                {
                    listOfCardSuit.Add(cardData.Suit);
                    dictionaryOfCardSuit.Add(cardData.Suit, new List<int>() { i });
                }
            }

            listOfCardSuit.Sort();

            Array cardCombinationTypes = Enum.GetValues(typeof(CardCombination.Type));
            for (int i = (int)cardCombinationTypes.GetValue(cardCombinationTypes.Length - 1); i >= 0; i--)
            {
                switch ((CardCombination.Type)i)
                {
                    case CardCombination.Type.Straight:
                        int headIndex = -1;

                        for (int j = Constant.CARD_RANK_MIN + 4; j <= Constant.CARD_RANK_MAX; j++)
                        {
                            int rank = j;
                            if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                            {
                                headIndex = listOfCardIndex[0];

                                if (headIndex > 0)
                                {
                                    Card card = sortedCards[headIndex];
                                    int headRank = card.CardData.Rank;
                                    int backtrackIteration = 1;

                                    Card[] combinationCards = new Card[5];
                                    combinationCards[combinationCards.Length - 1] = card;

                                    for (int k = 0; k < 4; k++)
                                    {
                                        int accessRank = headRank - k - 1;
                                        if (dictionaryOfCardRank.TryGetValue(accessRank, out listOfCardIndex))
                                        {
                                            combinationCards[combinationCards.Length - 2 - k] = sortedCards[listOfCardIndex[0]];
                                            backtrackIteration++;
                                        }
                                        else
                                        {
                                            headIndex = -1;
                                            j = j - backtrackIteration + 4;
                                            break;
                                        }
                                    }

                                    if (backtrackIteration == 5)
                                    {
                                        cardCombination.Submit(CardCombination.Type.Straight, headRank, card.CardData.Suit, combinationCards);

                                        i = -1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                headIndex = -1;
                            }

                            if (firstTurn)
                            {
                                break;
                            }
                        }
                        break;
                    case CardCombination.Type.Flush:
                        for (int j = 0; j < listOfCardSuit.Count; j++)
                        {
                            int suit = listOfCardSuit[j];

                            if (dictionaryOfCardSuit.TryGetValue(suit, out listOfCardIndex))
                            {
                                if (listOfCardIndex.Count >= 5)
                                {
                                    Card[] combinationCards = new Card[5];
                                    for (int k = 0; k < combinationCards.Length; k++)
                                    {
                                        combinationCards[k] = sortedCards[listOfCardIndex[k]];
                                    }
                                    int combinationRank = combinationCards[combinationCards.Length - 1].CardData.Rank;
                                    int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                    cardCombination.Submit(CardCombination.Type.Flush, combinationRank, combinationSuit, combinationCards);

                                    i = -1;
                                    break;
                                }

                                if (firstTurn)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    case CardCombination.Type.FullHouse:
                        int triplesRank = 0;
                        int pairsRank = 0;

                        for (int j = 0; j < listOfCardRank.Count; j++)
                        {
                            int rank = listOfCardRank[j];
                            if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                            {
                                if (listOfCardIndex.Count >= 3)
                                {
                                    if (triplesRank == 0)
                                    {
                                        triplesRank = rank;
                                    }
                                    else
                                    {
                                        pairsRank = rank;
                                        triplesRank = triplesRank - pairsRank;
                                        pairsRank = triplesRank + pairsRank;
                                        triplesRank = pairsRank - triplesRank;
                                    }
                                }
                                else if (listOfCardIndex.Count >= 2 && pairsRank == 0)
                                {
                                    pairsRank = rank;
                                }

                                if (triplesRank > 0 && pairsRank > 0)
                                {
                                    break;
                                }
                            }

                            if (firstTurn && (triplesRank != 1 || pairsRank != 1))
                            {
                                break;
                            }
                        }

                        if (triplesRank > 0 && pairsRank > 0)
                        {
                            Card[] combinationCards = new Card[5];
                            for (int j = 0; j < combinationCards.Length; j++)
                            {
                                if (j < 3) //Triples
                                {
                                    listOfCardIndex = dictionaryOfCardRank[triplesRank];
                                    combinationCards[j] = sortedCards[listOfCardIndex[j]];
                                }
                                else //Pairs
                                {
                                    listOfCardIndex = dictionaryOfCardRank[pairsRank];
                                    combinationCards[j] = sortedCards[listOfCardIndex[j - 3]];
                                }
                            }
                            int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                            cardCombination.Submit(CardCombination.Type.FullHouse, triplesRank, combinationSuit, combinationCards);

                            i = -1;
                        }
                        break;
                    case CardCombination.Type.FourOfAKind:
                        int quadruplesRank = 0;
                        int singleRank = 0;

                        for (int j = 0; j < listOfCardRank.Count; j++)
                        {
                            int rank = listOfCardRank[j];
                            if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                            {
                                if (listOfCardIndex.Count >= 4)
                                {
                                    if (quadruplesRank == 0)
                                    {
                                        quadruplesRank = rank;
                                    }
                                    else
                                    {
                                        singleRank = rank;
                                        quadruplesRank = quadruplesRank - singleRank;
                                        singleRank = quadruplesRank + singleRank;
                                        quadruplesRank = singleRank - quadruplesRank;
                                    }
                                }
                                else if (singleRank == 0)
                                {
                                    singleRank = rank;
                                }
                            }

                            if (firstTurn && (quadruplesRank != 1 || singleRank != 1))
                            {
                                break;
                            }
                        }

                        if (quadruplesRank > 0 && singleRank > 0)
                        {
                            Card[] combinationCards = new Card[5];
                            for (int j = 0; j < combinationCards.Length; j++)
                            {
                                if (j < 4) //Quadruples
                                {
                                    listOfCardIndex = dictionaryOfCardRank[quadruplesRank];
                                    combinationCards[j] = sortedCards[listOfCardIndex[j]];
                                }
                                else //Single
                                {
                                    listOfCardIndex = dictionaryOfCardRank[singleRank];
                                    combinationCards[j] = sortedCards[listOfCardIndex[j - 4]];
                                }
                            }
                            int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                            cardCombination.Submit(CardCombination.Type.Flush, quadruplesRank, combinationSuit, combinationCards);

                            i = -1;
                        }
                        break;
                    case CardCombination.Type.StraightFlush:
                        headIndex = -1;

                        for (int j = Constant.CARD_RANK_MIN + 4; j <= Constant.CARD_RANK_MAX; j++)
                        {
                            int rank = j;
                            if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                            {
                                headIndex = listOfCardIndex[0];

                                if (headIndex > 0)
                                {
                                    int headRank = sortedCards[headIndex].CardData.Rank;
                                    int headSuit = sortedCards[headIndex].CardData.Suit;
                                    int backtrackIteration = 1;

                                    Card[] combinationCards = new Card[5];
                                    combinationCards[combinationCards.Length - 1] = sortedCards[headIndex];

                                    for (int k = 0; k < 5 - 1; k++)
                                    {
                                        int accessRank = headRank - k - 1;
                                        bool isBacktrackIterationSuccess = false;
                                        if (dictionaryOfCardRank.TryGetValue(accessRank, out listOfCardIndex))
                                        {
                                            for (int l = 0; l < listOfCardIndex.Count; l++)
                                            {
                                                int cardIndex = listOfCardIndex[l];
                                                Card card = sortedCards[cardIndex];

                                                if (card.CardData.Suit == headSuit)
                                                {
                                                    combinationCards[combinationCards.Length - 2 - k] = sortedCards[listOfCardIndex[l]];
                                                    backtrackIteration++;
                                                    isBacktrackIterationSuccess = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (!isBacktrackIterationSuccess)
                                        {
                                            headIndex = -1;
                                            j = j - backtrackIteration + 4;
                                            break;
                                        }
                                    }

                                    if (backtrackIteration == 5)
                                    {
                                        cardCombination.Submit(CardCombination.Type.StraightFlush, headRank, headSuit, combinationCards);

                                        i = -1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                headIndex = -1;
                            }

                            if (firstTurn)
                            {
                                break;
                            }
                        }
                        break;
                    case CardCombination.Type.Triples:
                        triplesRank = 0;
                        for (int j = 0; j < listOfCardRank.Count; j++)
                        {
                            int rank = listOfCardRank[j];
                            if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                            {
                                if (listOfCardIndex.Count >= 3)
                                {
                                    triplesRank = rank;
                                    Card[] combinationCards = new Card[3];
                                    for (int k = 0; k < combinationCards.Length; k++)
                                    {
                                        combinationCards[k] = sortedCards[listOfCardIndex[k]];
                                    }
                                    int triplesSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                    cardCombination.Submit(CardCombination.Type.Triples, triplesRank, triplesSuit, combinationCards);

                                    i = -1;
                                    break;
                                }
                            }

                            if (firstTurn)
                            {
                                break;
                            }
                        }
                        break;
                    case CardCombination.Type.Pairs:
                        pairsRank = 0;

                        for (int j = 0; j < listOfCardRank.Count; j++)
                        {
                            int rank = listOfCardRank[j];
                            if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                            {
                                if (listOfCardIndex.Count >= 2)
                                {
                                    pairsRank = rank;
                                    Card[] combinationCards = new Card[2];

                                    for (int k = 0; k < listOfCardIndex.Count; k++)
                                    {
                                        if (k < 2)
                                        {
                                            combinationCards[k] = sortedCards[listOfCardIndex[k]];
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    int pairsSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;
                                    cardCombination.Submit(CardCombination.Type.Pairs, pairsRank, pairsSuit, combinationCards);

                                    i = -1;
                                    break;
                                }
                            }

                            if (firstTurn)
                            {
                                break;
                            }
                        }
                        break;
                    case CardCombination.Type.Single:
                        Card singleCard = sortedCards[0];
                        cardCombination.Submit(CardCombination.Type.Single, singleCard.CardData.Rank, singleCard.CardData.Suit, new Card[] { singleCard });

                        i = -1;
                        break;
                }
            }

            return cardCombination;
        }

        public CardCombination GetHigherCardCombination()
        {
            CardCombination cardCombination = new CardCombination();

            CardCombination cardCombinationOnField = Field.Instance.CardCombination;

            Card[] sortedCards = Hand.ListOfCard.OrderBy(card => card.CardData.Rank + card.CardData.Suit / (Constant.CARD_SUIT_MAX + 1)).ToArray();
            Dictionary<int, List<int>> dictionaryOfCardRank = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> dictionaryOfCardSuit = new Dictionary<int, List<int>>();

            List<int> listOfCardRank = new List<int>();
            List<int> listOfCardSuit = new List<int>();

            List<int> listOfCardIndex;

            for (int i = 0; i < sortedCards.Length; i++)
            {
                CardData cardData = sortedCards[i].CardData;
                if (dictionaryOfCardRank.TryGetValue(cardData.Rank, out listOfCardIndex))
                {
                    listOfCardIndex.Add(i);
                }
                else
                {
                    listOfCardRank.Add(cardData.Rank);
                    dictionaryOfCardRank.Add(cardData.Rank, new List<int>() { i });
                }

                if (dictionaryOfCardSuit.TryGetValue(cardData.Suit, out listOfCardIndex))
                {
                    listOfCardIndex.Add(i);
                }
                else
                {
                    listOfCardSuit.Add(cardData.Suit);
                    dictionaryOfCardSuit.Add(cardData.Suit, new List<int>() { i });
                }
            }

            listOfCardSuit.Sort();

            switch (cardCombinationOnField.Cards.Length)
            {
                case 1:
                    for (int i = cardCombinationOnField.Rank; i <= Constant.CARD_RANK_MAX; i++)
                    {
                        int rank = i;
                        if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                        {
                            if (rank == cardCombinationOnField.Rank)
                            {
                                for (int j = 0; j < listOfCardIndex.Count; j++)
                                {
                                    int cardIndex = listOfCardIndex[j];
                                    Card card = sortedCards[cardIndex];
                                    int suit = card.CardData.Suit;

                                    if (suit > cardCombinationOnField.Suit)
                                    {
                                        cardCombination.Submit(CardCombination.Type.Single, rank, suit, new Card[] { card });

                                        i = Constant.CARD_RANK_MAX + 1;
                                        break;
                                    }
                                }
                            }
                            else if (rank > cardCombinationOnField.Rank)
                            {
                                Card card = sortedCards[listOfCardIndex[0]];
                                int suit = card.CardData.Suit;
                                cardCombination.Submit(CardCombination.Type.Single, rank, suit, new Card[] { card });

                                break;
                            }
                        }
                    }
                    break;
                case 2:
                    int pairsRank = 0;

                    for (int i = 0; i < listOfCardRank.Count; i++)
                    {
                        int rank = listOfCardRank[i];
                        listOfCardIndex = dictionaryOfCardRank[rank];

                        if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                        {
                            if (rank >= cardCombinationOnField.Rank && listOfCardIndex.Count >= 2)
                            {
                                Card[] combinationCards = new Card[2];
                                bool isValid = false;
                                pairsRank = rank;

                                for (int j = 0; j < listOfCardIndex.Count; j++)
                                {
                                    int suit = sortedCards[listOfCardIndex[j]].CardData.Suit;
                                    if (rank > cardCombinationOnField.Rank || suit > cardCombinationOnField.Suit)
                                    {
                                        isValid = true;
                                    }

                                    if (j < 2)
                                    {
                                        combinationCards[j] = sortedCards[listOfCardIndex[j]];
                                    }
                                    else if (isValid)
                                    {
                                        break;
                                    }
                                }

                                if (isValid)
                                {
                                    int pairsSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;
                                    cardCombination.Submit(CardCombination.Type.Pairs, pairsRank, pairsSuit, combinationCards);

                                    break;
                                }
                            }
                        }
                    }
                    break;
                case 3:
                    int triplesRank = 0;
                    for (int i = 0; i < listOfCardRank.Count; i++)
                    {
                        int rank = listOfCardRank[i];
                        if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                        {
                            if (rank > cardCombinationOnField.Rank && listOfCardIndex.Count >= 3)
                            {
                                triplesRank = rank;
                                Card[] combinationCards = new Card[3];
                                for (int j = 0; j < combinationCards.Length; j++)
                                {
                                    combinationCards[j] = sortedCards[listOfCardIndex[j]];
                                }
                                int triplesSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                cardCombination.Submit(CardCombination.Type.Triples, triplesRank, triplesSuit, combinationCards);
                                break;
                            }
                        }
                    }
                    break;
                case 5:
                    Array cardCombinationTypes = Enum.GetValues(typeof(CardCombination.Type));
                    for (int i = (int)cardCombinationOnField.CombinationType; i < cardCombinationTypes.Length; i++)
                    {
                        CardCombination.Type cardCombinationType = (CardCombination.Type)cardCombinationTypes.GetValue(i);

                        switch ((CardCombination.Type)i)
                        {
                            case CardCombination.Type.Straight:
                                int headIndex = -1;

                                for (int j = cardCombinationOnField.Rank; j <= Constant.CARD_RANK_MAX; j++)
                                {
                                    int rank = j;
                                    if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                                    {
                                        if (j == cardCombinationOnField.Rank)
                                        {
                                            for (int k = 0; k < listOfCardIndex.Count; k++)
                                            {
                                                int cardIndex = listOfCardIndex[k];
                                                if (sortedCards[cardIndex].CardData.Suit > cardCombinationOnField.Suit)
                                                {
                                                    headIndex = cardIndex;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            headIndex = listOfCardIndex[0];
                                        }

                                        if (headIndex > 0)
                                        {
                                            Card card = sortedCards[headIndex];
                                            int headRank = card.CardData.Rank;
                                            int backtrackIteration = 1;

                                            Card[] combinationCards = new Card[cardCombinationOnField.Cards.Length];
                                            combinationCards[combinationCards.Length - 1] = card;

                                            for (int k = 0; k < cardCombinationOnField.Cards.Length - 1; k++)
                                            {
                                                int accessRank = headRank - k - 1;
                                                if (dictionaryOfCardRank.TryGetValue(accessRank, out listOfCardIndex))
                                                {
                                                    combinationCards[combinationCards.Length - 2 - k] = sortedCards[listOfCardIndex[0]];
                                                    backtrackIteration++;
                                                }
                                                else
                                                {
                                                    headIndex = -1;
                                                    j = j - backtrackIteration + cardCombinationOnField.Cards.Length - 1;
                                                    break;
                                                }
                                            }

                                            if (backtrackIteration == cardCombinationOnField.Cards.Length)
                                            {
                                                cardCombination.Submit(CardCombination.Type.Straight, headRank, card.CardData.Suit, combinationCards);

                                                i = cardCombinationTypes.Length;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        headIndex = -1;
                                    }
                                }
                                break;
                            case CardCombination.Type.Flush:
                                for (int j = Constant.CARD_SUIT_MIN; j <= Constant.CARD_SUIT_MAX; j++)
                                {
                                    int suit = j;

                                    if (dictionaryOfCardSuit.TryGetValue(suit, out listOfCardIndex))
                                    {
                                        if (suit >= cardCombinationOnField.Suit && listOfCardIndex.Count >= cardCombinationOnField.Cards.Length)
                                        {
                                            if (cardCombinationOnField.CombinationType == CardCombination.Type.Flush && suit == cardCombinationOnField.Suit)
                                            {
                                                for (int k = cardCombinationOnField.Cards.Length - 1; k < listOfCardIndex.Count; k++)
                                                {
                                                    Card card = sortedCards[listOfCardIndex[k]];
                                                    if (card.CardData.Rank > cardCombinationOnField.Rank)
                                                    {
                                                        Card[] combinationCards = new Card[cardCombinationOnField.Cards.Length];
                                                        int endIndex = k;
                                                        int startIndex = k - combinationCards.Length + 1;
                                                        for (int l = startIndex; l < endIndex + 1; l++)
                                                        {
                                                            combinationCards[l - startIndex] = sortedCards[listOfCardIndex[l]];
                                                        }
                                                        int combinationRank = combinationCards[combinationCards.Length - 1].CardData.Rank;
                                                        int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                                        cardCombination.Submit(CardCombination.Type.Flush, combinationRank, combinationSuit, combinationCards);

                                                        j = Constant.CARD_SUIT_MAX + 1;
                                                        i = cardCombinationTypes.Length;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Card[] combinationCards = new Card[cardCombinationOnField.Cards.Length];
                                                for (int k = 0; k < combinationCards.Length; k++)
                                                {
                                                    combinationCards[k] = sortedCards[listOfCardIndex[k]];
                                                }
                                                int combinationRank = combinationCards[combinationCards.Length - 1].CardData.Rank;
                                                int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                                cardCombination.Submit(CardCombination.Type.Flush, combinationRank, combinationSuit, combinationCards);

                                                i = cardCombinationTypes.Length;
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case CardCombination.Type.FullHouse:
                                triplesRank = 0;
                                pairsRank = 0;

                                for (int j = 0; j < listOfCardRank.Count; j++)
                                {
                                    int rank = listOfCardRank[j];
                                    listOfCardIndex = dictionaryOfCardRank[rank];
                                    if (rank > cardCombinationOnField.Rank && listOfCardIndex.Count >= 3)
                                    {
                                        if (triplesRank == 0)
                                        {
                                            triplesRank = rank;
                                        }
                                        else
                                        {
                                            triplesRank = triplesRank - pairsRank;
                                            pairsRank = triplesRank + pairsRank;
                                            triplesRank = pairsRank - triplesRank;
                                        }
                                    }
                                    else if (listOfCardIndex.Count >= 2 && pairsRank == 0)
                                    {
                                        pairsRank = rank;
                                    }
                                }

                                if (triplesRank > 0 && pairsRank > 0)
                                {
                                    Card[] combinationCards = new Card[cardCombinationOnField.Cards.Length];
                                    for (int j = 0; j < combinationCards.Length; j++)
                                    {
                                        if (j < 3) //Triples
                                        {
                                            listOfCardIndex = dictionaryOfCardRank[triplesRank];
                                            combinationCards[j] = sortedCards[listOfCardIndex[j]];
                                        }
                                        else //Pairs
                                        {
                                            listOfCardIndex = dictionaryOfCardRank[pairsRank];
                                            combinationCards[j] = sortedCards[listOfCardIndex[j - 3]];
                                        }
                                    }
                                    int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                    cardCombination.Submit(CardCombination.Type.FullHouse, triplesRank, combinationSuit, combinationCards);

                                    i = cardCombinationTypes.Length;
                                }
                                break;
                            case CardCombination.Type.FourOfAKind:
                                int quadruplesRank = 0;
                                int singleRank = 0;

                                for (int j = 0; j < listOfCardRank.Count; j++)
                                {
                                    int rank = listOfCardRank[j];
                                    listOfCardIndex = dictionaryOfCardRank[rank];
                                    if (rank > cardCombinationOnField.Rank && listOfCardIndex.Count >= 4)
                                    {
                                        if (quadruplesRank == 0)
                                        {
                                            quadruplesRank = rank;
                                        }
                                        else
                                        {
                                            quadruplesRank = quadruplesRank - singleRank;
                                            singleRank = quadruplesRank + singleRank;
                                            quadruplesRank = singleRank - quadruplesRank;
                                        }
                                    }
                                    else if (singleRank == 0)
                                    {
                                        singleRank = rank;
                                    }
                                }

                                if (quadruplesRank > 0 && singleRank > 0)
                                {
                                    Card[] combinationCards = new Card[cardCombinationOnField.Cards.Length];
                                    for (int j = 0; j < combinationCards.Length; j++)
                                    {
                                        if (j < 4) //Quadruples
                                        {
                                            listOfCardIndex = dictionaryOfCardRank[quadruplesRank];
                                            combinationCards[j] = sortedCards[listOfCardIndex[j]];
                                        }
                                        else //Single
                                        {
                                            listOfCardIndex = dictionaryOfCardRank[singleRank];
                                            combinationCards[j] = sortedCards[listOfCardIndex[j - 4]];
                                        }
                                    }
                                    int combinationSuit = combinationCards[combinationCards.Length - 1].CardData.Suit;

                                    cardCombination.Submit(CardCombination.Type.Flush, quadruplesRank, combinationSuit, combinationCards);

                                    i = cardCombinationTypes.Length;
                                }
                                break;
                            case CardCombination.Type.StraightFlush:
                                headIndex = -1;

                                for (int j = cardCombinationOnField.Rank; j <= Constant.CARD_RANK_MAX; j++)
                                {
                                    int rank = j;
                                    if (dictionaryOfCardRank.TryGetValue(rank, out listOfCardIndex))
                                    {
                                        if (j == cardCombinationOnField.Rank)
                                        {
                                            for (int k = 0; k < listOfCardIndex.Count; k++)
                                            {
                                                int cardIndex = listOfCardIndex[k];
                                                if (sortedCards[cardIndex].CardData.Suit > cardCombinationOnField.Suit)
                                                {
                                                    headIndex = cardIndex;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            headIndex = listOfCardIndex[0];
                                        }

                                        if (headIndex > 0)
                                        {
                                            int headRank = sortedCards[headIndex].CardData.Rank;
                                            int headSuit = sortedCards[headIndex].CardData.Suit;
                                            int backtrackIteration = 1;

                                            Card[] combinationCards = new Card[cardCombinationOnField.Cards.Length];
                                            combinationCards[combinationCards.Length - 1] = sortedCards[headIndex];

                                            for (int k = 0; k < cardCombinationOnField.Cards.Length - 1; k++)
                                            {
                                                int accessRank = headRank - k - 1;
                                                bool isBacktrackIterationSuccess = false;
                                                if (dictionaryOfCardRank.TryGetValue(accessRank, out listOfCardIndex))
                                                {
                                                    for (int l = 0; l < listOfCardIndex.Count; l++)
                                                    {
                                                        int cardIndex = listOfCardIndex[l];
                                                        Card card = sortedCards[cardIndex];

                                                        if (card.CardData.Suit == headSuit)
                                                        {
                                                            combinationCards[combinationCards.Length - 2 - k] = sortedCards[listOfCardIndex[l]];
                                                            backtrackIteration++;
                                                            isBacktrackIterationSuccess = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (!isBacktrackIterationSuccess)
                                                {
                                                    headIndex = -1;
                                                    j = j - backtrackIteration + cardCombinationOnField.Cards.Length - 1;
                                                    break;
                                                }
                                            }

                                            if (backtrackIteration == cardCombinationOnField.Cards.Length)
                                            {
                                                cardCombination.Submit(CardCombination.Type.StraightFlush, headRank, headSuit, combinationCards);

                                                i = cardCombinationTypes.Length;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        headIndex = -1;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            return cardCombination;
        }

        public void OnEventInvoked(EventGameState eventType)
        {

        }

        public void OnEventInvoked(EventPlayerTurn eventType)
        {
            if (eventType.Player == this)
            {
                Brain = new Brain<PlayerBot>(this, new Idle());
            }
        }
    }
}
