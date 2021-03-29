using System.Collections.Generic;
using DataClasses;

namespace Stein_schere_papier
{
    public interface ISpielLogic
    {
        IEnumerable<Spiel> GetAlleSpiele();
        Spiel NeuesSpielErstellen();
        Spiel SpielerZuSpielHinzufuegen(int spielId, Spieler spieler);
        Spieler GewinnerErmitteln(int spielId);
    }
}