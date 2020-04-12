using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Card_5Dmg : Card
{
    public Card_5Dmg(CardType type) : base(type) { }
    public override void Lose(RPSPlayerState self, RPSPlayerState opponent) { }
    public override void Tie(RPSPlayerState self, RPSPlayerState opponent) { }
    public override void Win(RPSPlayerState self, RPSPlayerState opponent) { opponent.TakeDamage(5); }

    public override string Name() { return "" + type; }
    public override string Desc() { return "Win: deal 5 damage."; }
}

class Card_W3Dmg_T3Dmg : Card
{
    public Card_W3Dmg_T3Dmg(CardType type) : base(type) { }
    public override void Lose(RPSPlayerState self, RPSPlayerState opponent) { }
    public override void Tie(RPSPlayerState self, RPSPlayerState opponent) { opponent.TakeDamage(3); }
    public override void Win(RPSPlayerState self, RPSPlayerState opponent) { opponent.TakeDamage(3); }

    public override string Name() { return "Tied " + type; }
    public override string Desc() { return "Win: deal 3 damage. Tie: deal 3 damage."; }
}

class Card_W7Dmg_L2SD : Card
{
    public Card_W7Dmg_L2SD(CardType type) : base(type) { }
    public override void Lose(RPSPlayerState self, RPSPlayerState opponent) { self.TakeDamage(2); }
    public override void Tie(RPSPlayerState self, RPSPlayerState opponent) { }
    public override void Win(RPSPlayerState self, RPSPlayerState opponent) { opponent.TakeDamage(7); }

    public override string Name()
    {
        switch (type)
        {
            case CardType.Rock: return "Heavy " + type;
            case CardType.Paper: return "Big " + type;
            case CardType.Scissors: return "Sharp " + type;
        }
        return "" + type;
    }
    public override string Desc() { return "Win: deal 7 damage. Lost: take 2 damage."; }
}
