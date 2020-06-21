using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EpoMaker.resources;
using Microsoft.Win32;

namespace EpoMaker
{
    class ExcelFile
    {
        private readonly SpreadsheetDocument spreadsheetDocument;
        private readonly WorksheetPart worksheetPart;
        private readonly Sheets sheets;
        private readonly SheetData sheetData;

        public ExcelFile(ArrayList dataList)
        {
            SaveFileDialog excelFile = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = langDE.FILEDIALOG_FileTypeName_ExcelFile+" (*.xlsx)|*.xlsx"
            };
            if (excelFile.ShowDialog()!=true)
            {
                return;
            }
            this.spreadsheetDocument = SpreadsheetDocument.Create(excelFile.FileName, SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();
            sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());
            // Add Sheets to the Workbook.
            sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = langDE.TABLE_SheetName
            };
            sheets.Append(sheet);


            
            foreach (Array rowData in dataList)
            {
                Row row = new Row();
                foreach (string cellData in rowData)
                {
                    if (Regex.Replace(cellData, @"[0-3][0-9].[0-1]\d.\d\d\d\d", "").Length == 0)
                    {
                        row.Append(new Cell
                        {
                            CellValue = new CellValue(cellData),
                            DataType = CellValues.String
                        });
                    }
                    else if (Regex.Replace(cellData, @"[0-9.]+", "").Length == 0)
                    {
                        row.Append(new Cell
                        {
                            CellValue = new CellValue(cellData.Replace(".", ",")),
                            DataType = CellValues.String
                        });
                    }
                    else
                    {
                        row.Append(new Cell
                        {
                            CellValue = new CellValue(cellData),
                            DataType = CellValues.String
                        });
                    }
                }
                sheetData.AppendChild(row);
            }
            

            
            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }


    }
}
