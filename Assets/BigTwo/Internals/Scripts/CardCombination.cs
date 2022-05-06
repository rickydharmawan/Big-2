using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigTwo
{
    public class CardCombination
    {
        public enum Type
        {
            None,
            Single,
            Pairs,
            Triples,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
        }

        private Card[] m_cards;
        private Type m_combinationType; //1 => Single, 2 => Pairs, 3 => Triples, 4 => Straight, 5 => Flush, 6 => Full House, 7 => Four of a Kind, 8 => Straight Flush
        private int m_rank;
        private int m_suit;

        public Card[] Cards
        {
            get
            {
                return m_cards;
            }
        }

        public Type CombinationType
        {
            get
            {
                return m_combinationType;
            }
        }

        public int Rank
        {
            get
            {
                return m_rank;
            }
        }

        public int Suit
        {
            get
            {
                return m_suit;
            }
        }

        public static Type GetCardCombinationType(Card[] cards)
        {
            switch (cards.Length)
            {
                case 1:
                    return Type.Single;
                case 2:
                    if (IsCardsInTheSameRank(cards))
                    {
                        return Type.Pairs;
                    }
                    break;
                case 3:
                    if (IsCardsInTheSameRank(cards))
                    {
                        return Type.Triples;
                    }
                    break;
                case 5:
                    Card[] sortedCards = cards.OrderBy(card => card.CardData.Rank + card.CardData.Suit / (Constant.CARD_SUIT_MAX + 1)).ToArray();

                    int firstCardRank = 0;
                    int firstCardSuit = 0;
                    bool isConsecutive = true;
                    bool isSameSuit = true;
                    Dictionary<int, List<int>> dictionaryOfCardRank = new Dictionary<int, List<int>>();
                    List<int> listOfCardRank = new List<int>();
                    for (int i = 0; i < sortedCards.Length; i++)
                    {
                        CardData cardData = sortedCards[i].CardData;
                        if (i == 0)
                        {
                            firstCardRank = cardData.Rank;
                            firstCardSuit = cardData.Suit;
                        }
                        else
                        {
                            if (isConsecutive && !((cardData.Rank == firstCardRank + i && cardData.Rank != Constant.CARD_RANK_MAX) || (cardData.Rank == Constant.CARD_RANK_MAX && firstCardRank == Constant.CARD_RANK_MIN)))
                            {
                                isConsecutive = false;
                            }
                            else if (isSameSuit && cardData.Suit != firstCardSuit)
                            {
                                isSameSuit = false;
                            }
                        }

                        if (dictionaryOfCardRank.TryGetValue(cardData.Rank, out List<int> listOfCardIndex))
                        {
                            listOfCardIndex.Add(i);
                        }
                        else
                        {
                            listOfCardRank.Add(cardData.Rank);
                            dictionaryOfCardRank.Add(cardData.Rank, new List<int>() { i });
                        }
                    }

                    if (isConsecutive && isSameSuit)
                    {
                        return Type.StraightFlush;
                    }
                    else if (isConsecutive)
                    {
                        return Type.Straight;
                    }
                    else if (isSameSuit)
                    {
                        return Type.Flush;
                    }
                    else
                    {
                        if (listOfCardRank.Count == 2)
                        {
                            firstCardRank = listOfCardRank[0];
                            int firstCardRankCount = dictionaryOfCardRank[firstCardRank].Count;

                            if (firstCardRankCount == 2 || firstCardRankCount == 3)
                            {
                                return Type.FullHouse;
                            }
                            else if (firstCardRankCount == 1 || firstCardRankCount == 4)
                            {
                                return Type.FourOfAKind;
                            }
                        }

                        return Type.None;
                    }
            }

            return Type.None;
        }

        public static bool IsCardsInTheSameRank(Card[] cards)
        {
            int firstCardRank = cards[0].CardData.Rank;
            for (int i = 1; i < cards.Length; i++)
            {
                if (cards[i].CardData.Rank != firstCardRank)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidToChallenge(CardCombination otherCardCombination)
        {
            switch (m_combinationType)
            {
                case Type.Single:
                case Type.Pairs:
                case Type.Triples:
                    return m_combinationType == otherCardCombination.CombinationType;
                case Type.Straight:
                case Type.Flush:
                case Type.FullHouse:
                case Type.FourOfAKind:
                case Type.StraightFlush:
                    return otherCardCombination.CombinationType >= Type.Flush && otherCardCombination.CombinationType <= Type.StraightFlush;
                default:
                    return false;
            }
        }

        public bool IsHigherThan(CardCombination otherCardCombination)
        {
            switch (m_combinationType)
            {
                case Type.Single:
                    return m_cards[0].IsHigherThan(otherCardCombination.Cards[0]);
                case Type.Pairs:
                    return Rank > otherCardCombination.Rank || (Rank == otherCardCombination.Rank && Suit > otherCardCombination.Suit);
                case Type.Triples:
                    return Rank > otherCardCombination.Rank;
                case Type.Flush:
                    if (m_combinationType > otherCardCombination.CombinationType)
                    {
                        return true;
                    }
                    else if (m_combinationType == otherCardCombination.CombinationType)
                    {
                        return Suit > otherCardCombination.Suit || (Suit == otherCardCombination.Suit && Rank > otherCardCombination.Rank);
                    }
                    else
                    {
                        return false;
                    }
                case Type.Straight:
                case Type.FullHouse:
                case Type.FourOfAKind:
                case Type.StraightFlush:
                    if (m_combinationType > otherCardCombination.CombinationType)
                    {
                        return true;
                    }
                    else if (m_combinationType == otherCardCombination.CombinationType)
                    {
                        return Rank > otherCardCombination.Rank || (Rank == otherCardCombination.Rank && Suit > otherCardCombination.Suit);
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        public void Submit(Type cardCombinationType, int rank, int suit, Card[] cards)
        {
            m_combinationType = cardCombinationType;
            m_rank = rank;
            m_suit = suit;

            m_cards = cards;
        }

        public void Submit(Type cardCombinationType, Card[] cards)
        {
            m_combinationType = cardCombinationType;
            Card[] sortedCards = cards.OrderBy(card => card.CardData.Rank + (float)((float)card.CardData.Suit / (float)(Constant.CARD_SUIT_MAX + 1))).ToArray();

            switch (m_combinationType)
            {
                case Type.Single:
                case Type.Pairs:
                case Type.Triples:
                case Type.Straight:
                case Type.Flush:
                case Type.StraightFlush:
                    m_rank = sortedCards[sortedCards.Length - 1].CardData.Rank;
                    m_suit = sortedCards[sortedCards.Length - 1].CardData.Suit;
                    break;
                case Type.FullHouse:
                    int rank = 0;
                    int iteration = 0;
                    for (int i = 0; i < sortedCards.Length; i++)
                    {
                        Card card = sortedCards[i];
                        if (i == 0)
                        {
                            rank = card.CardData.Rank;
                            iteration++;
                        }
                        else if (card.CardData.Rank == rank)
                        {
                            iteration++;
                            if (iteration == 3)
                            {
                                m_rank = card.CardData.Rank;
                                m_suit = card.CardData.Suit;
                                break;
                            }
                        }
                        else if (iteration < 3)
                        {
                            m_rank = card.CardData.Rank;
                            m_suit = sortedCards[sortedCards.Length - 1].CardData.Rank;
                            break;
                        }
                    }
                    break;
                case Type.FourOfAKind:
                    rank = 0;
                    iteration = 0;
                    for (int i = 0; i < sortedCards.Length; i++)
                    {
                        Card card = sortedCards[i];
                        if (i == 0)
                        {
                            rank = card.CardData.Rank;
                            iteration++;
                        }
                        else if (card.CardData.Rank == rank)
                        {
                            iteration++;
                            if (iteration == 4)
                            {
                                m_rank = card.CardData.Rank;
                                m_suit = card.CardData.Suit;
                                break;
                            }
                        }
                        else if (iteration < 4)
                        {
                            m_rank = card.CardData.Rank;
                            m_suit = sortedCards[sortedCards.Length - 1].CardData.Rank;
                            break;
                        }
                    }
                    break;

            }

            m_cards = cards;
        }
    }
}
