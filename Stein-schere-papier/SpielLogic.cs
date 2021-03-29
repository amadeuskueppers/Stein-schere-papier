﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataClasses;
using Repository;

namespace Stein_schere_papier
{
    public class SpielLogic : ISpielLogic
    {
        private readonly ISpielRepository _spielRepository;

        public SpielLogic(ISpielRepository spielRepository)
        {
            _spielRepository = spielRepository;
        }

        public IEnumerable<Spiel> GetAlleSpiele()
        {
            var entries = _spielRepository.GetAll().ToList();
            return entries;
        }

        public Spiel NeuesSpielErstellen()
        {
            var repositoryEntries = _spielRepository.GetAll();

            var neueId = repositoryEntries.Any() ? repositoryEntries.Max(s => s.Id) + 1 : 1;

            var neuesSpiel = new Spiel
            {
                Id = neueId
            };

            _spielRepository.Add(neuesSpiel);

            return neuesSpiel;
        }

        public Spiel SpielerZuSpielHinzufuegen(int spielId, Spieler spieler)
        {
            var betroffenesSpiel = _spielRepository.GetById(spielId);

            var ersterSpielerNull = betroffenesSpiel.ErstenSpieler == null;
            var zweiterSpielerNull = betroffenesSpiel.ZweitenSpieler == null;

            if (ersterSpielerNull)
            {
                var neueSpielerId = GetMaxmimaleSpielerid() + 1;

                var neuerSpieler = new Spieler
                {
                    Id = neueSpielerId,
                    Gegenstandsauswahl = spieler.Gegenstandsauswahl,
                    Name = spieler.Name
                };

                betroffenesSpiel.ErstenSpieler = neuerSpieler;
            }
            else if (zweiterSpielerNull)
            {
                var neueSpielerId = GetMaxmimaleSpielerid() + 1;

                var neuerSpieler = new Spieler
                {
                    Id = neueSpielerId,
                    Gegenstandsauswahl = spieler.Gegenstandsauswahl,
                    Name = spieler.Name
                };

                betroffenesSpiel.ZweitenSpieler = neuerSpieler;
            }
            else
            {
                throw new Exception(
                    $"Maximale spielerzahl erreicht. Dem Spiel mit Id {spielId} kann kein Spieler mehr hinzugefuegt werden.");
            }

            return betroffenesSpiel;
        }


        public Spieler GewinnerErmitteln(int spielId)
        {
            Spieler gewinner = null;

            var betroffeneSpiel = _spielRepository.GetById(spielId);

            var spieler1GewaehlterGegenstand = betroffeneSpiel.ErstenSpieler.Gegenstandsauswahl;
            var spieler2GewaehlterGegenstand = betroffeneSpiel.ZweitenSpieler.Gegenstandsauswahl;

            if (spieler1GewaehlterGegenstand == Gegenstandsauswahl.Stein &&
                spieler2GewaehlterGegenstand == Gegenstandsauswahl.Schere)
                gewinner = betroffeneSpiel.ErstenSpieler;
            else if (spieler1GewaehlterGegenstand == Gegenstandsauswahl.Schere &&
                     spieler2GewaehlterGegenstand == Gegenstandsauswahl.Papier)
                gewinner = betroffeneSpiel.ErstenSpieler;
            else if (spieler1GewaehlterGegenstand == Gegenstandsauswahl.Papier &&
                     spieler2GewaehlterGegenstand == Gegenstandsauswahl.Stein)
                gewinner = betroffeneSpiel.ErstenSpieler;
            else if (spieler2GewaehlterGegenstand == Gegenstandsauswahl.Stein &&
                     spieler1GewaehlterGegenstand == Gegenstandsauswahl.Schere)
                gewinner = betroffeneSpiel.ZweitenSpieler;
            else if (spieler2GewaehlterGegenstand == Gegenstandsauswahl.Schere &&
                     spieler1GewaehlterGegenstand == Gegenstandsauswahl.Papier)
                gewinner = betroffeneSpiel.ZweitenSpieler;
            else if (spieler2GewaehlterGegenstand == Gegenstandsauswahl.Papier &&
                     spieler1GewaehlterGegenstand == Gegenstandsauswahl.Stein)
                gewinner = betroffeneSpiel.ZweitenSpieler;
            else
                throw new Exception("Keiner hat gewonnen. Unentschieden!");

            betroffeneSpiel.Gewinner = gewinner;

            return gewinner;
        }

        private int GetMaxmimaleSpielerid()
        {
            var maximaleSpielerId = 0;

            foreach (var spiel in _spielRepository.GetAll())
            {
                var spieler1IdHoeherMax = spiel.ErstenSpieler?.Id > maximaleSpielerId;
                if (spieler1IdHoeherMax) maximaleSpielerId = spiel.ErstenSpieler.Id;

                var spieler2IdHoeherMax = spiel.ZweitenSpieler?.Id > maximaleSpielerId;
                if (spieler2IdHoeherMax) maximaleSpielerId = spiel.ZweitenSpieler.Id;
            }

            return maximaleSpielerId;
        }
    }
}