using System;
using System.Collections.Generic;

public class RPSPlayerState
{
    public const int kDefaultHandSize = 5;
    public int health = 30;
    public List<int> deck;
    public List<int> hand;
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
    public RPSPlayerState()
    {
        deck = Deck.deck_default;
        deck.Shuffle();
        hand = new List<int>();
    }
    public bool Draw()
    {
        if (deck.Count == 0)
        {
            return false;
        }
        hand.Add(deck[0]);
        deck.RemoveAt(0);
        return true;
    }
}

public enum CardType
{
    None, Rock, Paper, Scissors
}

public enum CardResult
{
    None, Win, Lose, Tie
}

public abstract class Card
{
    public int ID;
    public CardType type;
    public static CardResult Compare(CardType a, CardType b)
    {
        if (a == CardType.None && b == CardType.None) return CardResult.Tie;
        if (a == CardType.None) return CardResult.Lose;
        if (b == CardType.None) return CardResult.Win;

        if (a == b) return CardResult.Tie;
        if (a == CardType.Rock && b == CardType.Scissors) return CardResult.Win;
        if (a == CardType.Paper && b == CardType.Rock) return CardResult.Win;
        if (a == CardType.Scissors && b == CardType.Paper) return CardResult.Win;
        return CardResult.Lose;
    }
    public Card(CardType type)
    {
        this.type = type;
    }

    public abstract void Win(RPSPlayerState self, RPSPlayerState opponent);
    public abstract void Tie(RPSPlayerState self, RPSPlayerState opponent);
    public abstract void Lose(RPSPlayerState self, RPSPlayerState opponent);
    public abstract string Name();
    public abstract string Desc();
}