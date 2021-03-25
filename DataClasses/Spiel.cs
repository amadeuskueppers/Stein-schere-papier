namespace DataClasses
{
    public class Spiel
    {
        public int Id { get; set; }
        public Spieler ErstenSpieler { get; set; }
        public Spieler ZweitenSpieler { get; set; }
        public Spieler Gewinner { get; set; }
    }
}