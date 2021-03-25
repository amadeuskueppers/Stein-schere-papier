using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DataClasses;
using RestSharp;

namespace ClientApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var restClient = new RestClient("https://localhost:44347");

            // Namen des Spielers ermitteln

            var spielerName = FrageSpielerNachNamen();

            var spielWiederholen = false;

            do
            {
                // Gegenstandauswahl

                var gegenstandAuswahl = FrageSpielerNachGegenstand();


                // Spiel erstellen oder bestehendem Spiel beitreten
                var bestehendemSpielBeitreten = FrageSpielerNachSpielbeitritt();

                Spiel spiel;

                if (bestehendemSpielBeitreten)
                {
                    var request = new RestRequest("spiele?nurOffeneSpiele=true");
                    var spieleVomServer = restClient.Get<IEnumerable<Spiel>>(request).Data;

                    var offenSpieleGefunden = spieleVomServer != null && spieleVomServer.Count() != 0;

                    if (offenSpieleGefunden)
                    {
                        spiel = FrageSpielerNachSpielauswahl(spieleVomServer);
                    }
                    else
                    {
                        Console.WriteLine("Es gibt keine offenen Spiele. Es wird ein neues Spiel erstellt!");
                        spiel = NeuesSpielErstellen(restClient);
                    }
                }
                else
                {
                    spiel = NeuesSpielErstellen(restClient);
                }

                // Spieler zum Spiel hinzufügen

                var spieler = new Spieler
                {
                    Name = spielerName,
                    Gegenstandsauswahl = gegenstandAuswahl
                };

                var spielerHinzufuegenRequest = new RestRequest($"spiele/{spiel.Id}/spieler").AddJsonBody(spieler);
                var geantertesSpielResponse = restClient.Post<Spiel>(spielerHinzufuegenRequest);
                spiel = geantertesSpielResponse.Data;

                // Gewinner ermitteln

                var spielVolsstaendig = spiel.ZweitenSpieler != null;

                if (!spielVolsstaendig)
                    // auf den 2. Spieler warten
                    do
                    {
                        Console.WriteLine("Es wird auf den zweiten Spieler gewartet...");
                        Thread.Sleep(3000);

                        var spielRequest = new RestRequest($"spiele/{spiel.Id}", DataFormat.Json);
                        spiel = restClient.Get<Spiel>(spielRequest).Data;

                        spielVolsstaendig = spiel.ZweitenSpieler != null;
                    } while (!spielVolsstaendig);

                var gewinnerErmittelnRequest = new RestRequest($"spiele/{spiel.Id}/gewinner");
                var gewinner = restClient.Get<Spieler>(gewinnerErmittelnRequest).Data;

                var binIchGewinner = gewinner.Name == spielerName;

                var ergebnisText = binIchGewinner
                    ? "Herzlichen Glückwunsch. Du hast Gewonnen!"
                    : $"Du hast leider verloren. {gewinner.Name} hat gewonnen.";
                Console.WriteLine(ergebnisText);

                // Noch ein spiel???
                Console.WriteLine("Noch ein Spiel? (j/n)");
                var auswahl = Console.ReadLine();
                spielWiederholen = auswahl == "j";
            } while (spielWiederholen);

            Console.ReadKey();
        }

        private static Spiel NeuesSpielErstellen(RestClient restClient)
        {
            Spiel spiel;
            Console.WriteLine("Neues Spiel wird erstellt....");

            var request = new RestRequest("spiele", DataFormat.Json);
            spiel = restClient.Post<Spiel>(request).Data;

            Console.WriteLine($"Spiel mit Id {spiel.Id} erfolgreich erstellt.");
            return spiel;
        }

        private static Spiel FrageSpielerNachSpielauswahl(IEnumerable<Spiel> spieleVomServer)
        {
            Console.WriteLine("Welchem Spiel möchtest du beitreten? (Nummer eingeben)");

            foreach (var spiel in spieleVomServer)
                Console.WriteLine($"{spiel.Id} - eröffnet von '{spiel.ErstenSpieler.Name}'");

            var ausgewaehltesSpiel = Console.ReadLine();

            int spielId;
            var gueltigeEingabe = int.TryParse(ausgewaehltesSpiel, out spielId) &&
                                  spieleVomServer.Any(s => s.Id == spielId);


            if (!gueltigeEingabe) return FrageSpielerNachSpielauswahl(spieleVomServer);

            var auswahl = spieleVomServer.First(s => s.Id == spielId);

            return auswahl;
        }

        private static bool FrageSpielerNachSpielbeitritt()
        {
            var bestehendemSpielBeitreten = true;

            Console.WriteLine("Möchtest einem Spiel betreten (1) oder ein neues Spiel beginnen (2) ?");
            var auswahl = Console.ReadLine();

            switch (auswahl)
            {
                case "1":
                    bestehendemSpielBeitreten = true;
                    break;
                case "2":
                    bestehendemSpielBeitreten = false;
                    break;
                default:
                    Console.WriteLine("Fehlerhaft Eingabe. Erneuter Veruch wird gestartet...");
                    bestehendemSpielBeitreten = FrageSpielerNachSpielbeitritt();
                    break;
            }

            return bestehendemSpielBeitreten;
        }

        private static Gegenstandsauswahl FrageSpielerNachGegenstand()
        {
            Gegenstandsauswahl auswahl;

            var auswahlMoeglichkeitenText = "1. Stein\n2. Schere\n3. Papier";

            var auswahlText = $"Welchen Gegenstand möchtest du wählen?\n{auswahlMoeglichkeitenText}";

            Console.WriteLine(auswahlText);

            var eingabe = Console.ReadLine();

            int eingabeEnumSchluessel;
            var validerInteger = int.TryParse(eingabe, out eingabeEnumSchluessel);

            if (!validerInteger)
            {
                Console.WriteLine("Ungültige Eingabe. Bitte versuche es erneut!");
                auswahl = FrageSpielerNachGegenstand();
            }

            try
            {
                var enumWert = Enum.GetValues(typeof(Gegenstandsauswahl)).GetValue(eingabeEnumSchluessel - 1);
                auswahl = (Gegenstandsauswahl) enumWert;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Ungültige Eingabe. Bitte versuche es erneut!");
                auswahl = FrageSpielerNachGegenstand();
            }

            return auswahl;
        }

        private static string FrageSpielerNachNamen()
        {
            Console.WriteLine("Bitte gib deinen Namen ein:");

            var spielerName = Console.ReadLine();

            if (string.IsNullOrEmpty(spielerName)) spielerName = FrageSpielerNachNamen();

            return spielerName;
        }
    }
}