using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Card_Subreme : Card
{
    public Card_Subreme() : base(CardType.Rock) { }
    public override void Lose(RPSPlayerState self, RPSPlayerState opponent) { }
    public override void Tie(RPSPlayerState self, RPSPlayerState opponent) { }
    public override void Win(RPSPlayerState self, RPSPlayerState opponent) { opponent.TakeDamage(11); }

    public override string Name() { return "Subreme brick"; }
    public override string Desc() { return "Win: deal 11 damage."; }
}
