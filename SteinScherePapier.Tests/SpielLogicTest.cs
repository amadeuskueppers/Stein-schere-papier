using System.Collections.Generic;
using System.Linq;
using DataClasses;
using DeepEqual.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository;
using Stein_schere_papier;

namespace SteinScherePapier.Tests
{
    [TestClass]
    public class SpielLogicTest
    {
        private readonly Mock<ISpielRepository> _spielRepositoryMock = new();
        private SpielLogic _spielLogic;

        [TestInitialize]
        public void Initialize()
        {
            _spielLogic = new SpielLogic(_spielRepositoryMock.Object);
        }

        [TestMethod]
        public void GetAlleSpiele_ReturnsEmptyList()
        {
            // Arrange
            var expected = new List<Spiel>
            {
                new()
                {
                    Id = 1,
                    ErstenSpieler = new Spieler
                    {
                        Name = "Max",
                        Gegenstandsauswahl = Gegenstandsauswahl.Stein,
                        Id = 1
                    }
                },
                new()
                {
                    Id = 2,
                    ErstenSpieler = new Spieler
                    {
                        Name = "Lisa",
                        Gegenstandsauswahl = Gegenstandsauswahl.Schere,
                        Id = 2
                    },
                    ZweitenSpieler = new Spieler
                    {
                        Name = "Johann",
                        Gegenstandsauswahl = Gegenstandsauswahl.Papier,
                        Id = 4
                    },
                    Gewinner = new Spieler
                    {
                        Name = "Lisa",
                        Gegenstandsauswahl = Gegenstandsauswahl.Schere,
                        Id = 2
                    }
                }
            };

            _spielRepositoryMock.Setup(m => m.GetAll()).Returns(expected.AsQueryable());

            // Act
            var actual = _spielLogic.GetAlleSpiele();

            // Assert
            actual.ShouldDeepEqual(expected);
        }


        [TestMethod]
        public void NeuesSpielErstellen_KeineSpieleVorhanden_ReturnsExpected()
        {
            // Arrange
            var entries = new List<Spiel>().AsQueryable();

            _spielRepositoryMock.Setup(m => m.GetAll()).Returns(entries);

            var expected = new Spiel
            {
                Id = 1
            };

            // Act
            var actual = _spielLogic.NeuesSpielErstellen();

            // Assert
            actual.ShouldDeepEqual(expected);
        }

        [TestMethod]
        public void NeuesSpielERstellen_VorherigeSpieleVorhanden_ReturnsExpected()
        {
            // Arrange
            var existingEntries = new List<Spiel>
            {
                new()
                {
                    Id = 1,
                    ErstenSpieler = new Spieler
                    {
                        Name = "Max",
                        Gegenstandsauswahl = Gegenstandsauswahl.Stein,
                        Id = 1
                    }
                },
                new()
                {
                    Id = 2,
                    ErstenSpieler = new Spieler
                    {
                        Name = "Lisa",
                        Gegenstandsauswahl = Gegenstandsauswahl.Schere,
                        Id = 2
                    },
                    ZweitenSpieler = new Spieler
                    {
                        Name = "Johann",
                        Gegenstandsauswahl = Gegenstandsauswahl.Papier,
                        Id = 4
                    },
                    Gewinner = new Spieler
                    {
                        Name = "Lisa",
                        Gegenstandsauswahl = Gegenstandsauswahl.Schere,
                        Id = 2
                    }
                }
            };

            _spielRepositoryMock.Setup(m => m.GetAll()).Returns(existingEntries.AsQueryable);

            var expected = new Spiel
            {
                Id = 3
            };

            // Act 
            var actual = _spielLogic.NeuesSpielErstellen();

            // Assert
            actual.ShouldDeepEqual(expected);
        }
    }
}