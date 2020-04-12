using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    None, Rock, Paper, Scissors
}

public enum CardResult
{
    None, Win, Lose, Tie
}

public class Card
{
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
}