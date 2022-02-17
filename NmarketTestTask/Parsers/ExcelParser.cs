using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class ExcelParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheets.First();
            var houses = sheet.Cells().Where(c => c.GetValue<string>().Contains("Дом")).ToList();
            List<House> housesList = new List<House>(); 
            //Реализация цикла, внутри которого идёт поиск квартир в рамках одного дома
            for (int houseID = 0; houseID != houses.Count(); houseID++)
            {
                List<Flat> _flats = new List<Flat>();
                //Цикл поиска квартир от строки в которой находиться дом до следующего дома(либо вниз на 25 строк)
                for (int RowID = houses[houseID].WorksheetRow().RowNumber()+1; RowID != houses[houses.Count()-1].WorksheetRow().RowNumber()+25; RowID++)
                {
                    for (int ColumnID = 1; ColumnID < 16383; ColumnID++)
                    {
                        if (sheet.Cell(RowID, ColumnID).Value.ToString() != "")
                            if (sheet.Cell(RowID, ColumnID).Value.ToString().Contains("Дом")) break;
                        //Реализация справделива, только в случае, если под ячейкой квартиры находиться сумма и квартиры пронумерованы
                            else if (sheet.Cell(RowID, ColumnID).Value.ToString().Contains('№'))
                            {
                                var _flatcell = sheet.Cell(RowID, ColumnID);
                                _flats.Add(new Flat() { Number = _flatcell.Value.ToString(), Price = _flatcell.CellBelow().Value.ToString() });
                            }
                    }
                }
                housesList.Add(new House()
                {
                    Name = houses[houseID].Value.ToString(),
                    Flats = _flats
                });
            }
            return housesList;
        }
    }
}
