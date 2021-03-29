using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using DataClasses;

namespace Repository
{
    public class SpielRepository : ISpielRepository
    {
        private const string FileName = "data.csv";


        public IQueryable<Spiel> GetAll()
        {
            using (var csvReader = new CsvReader(new StreamReader(FileName), CultureInfo.InvariantCulture))
            {
                var spiele = csvReader.GetRecords<Spiel>();
                return spiele.AsQueryable();
            }
        }

        public Spiel GetById(int id)
        {
            var result = GetAll().FirstOrDefault(i => i.Id == id);
            if (result == null)
                throw new Exception("Entry does not exist!");

            return result;
        }

        public Spiel Add(Spiel spiel)
        {
            throw new NotImplementedException();
        }
    }
}