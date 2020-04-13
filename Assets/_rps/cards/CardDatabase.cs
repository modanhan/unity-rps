using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase
{
    static List<Card> cards = new List<Card>
    {
        new Card_5Dmg(CardType.Rock),
        new Card_5Dmg(CardType.Paper),
        new Card_5Dmg(CardType.Scissors),
        new Card_W3Dmg_T3Dmg(CardType.Rock),
        new Card_W3Dmg_T3Dmg(CardType.Paper),
        new Card_W3Dmg_T3Dmg(CardType.Scissors),
        new Card_W7Dmg_L2SD(CardType.Rock),
        new Card_W7Dmg_L2SD(CardType.Paper),
        new Card_W7Dmg_L2SD(CardType.Scissors),
        new Card_Subreme(),
    };
    public static void Init()
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].ID = i;
        }
    }
    public static Card GetCard(int ID)
    {
        return cards[ID];
    }

}

public class Deck
{
    public static List<int> deck_default = new List<int> {
        0,1,2,3,4,5,
        // 0,1,2,3,4,5,
        // 6,7,8,
        // 9,
    };
}
