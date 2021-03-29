using System.Linq;
using DataClasses;

namespace Repository
{
    public interface ISpielRepository
    {
        IQueryable<Spiel> GetAll();
        Spiel GetById(int id);
        Spiel Add(Spiel spiel);
    }
}