namespace Game.Characters.CharacterComponents
{
    public interface ICanReceiveDamage
    {
        public void ReceiveDamage(int damage);
        public Faction Faction => Faction.None;
    }
}