using BinanceImporter.Domain.Entities;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceImporter.App
{
    public class CsvExtrator : ICsvExtrator
    {

        public List<T> GetObjects<T>(string file)
        {

            List<T> objects = new List<T>();

            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                objects = csv.GetRecords<T>().ToList();
            }

            return objects;

        }

    }
}
