using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace PhoneBookSharedTools.ExcelFileWorks._DocumentFormatXML
{
    public class ExcelFileWorks_DocumentXML : IExcelFileWorks
    {
        public byte[] ExcelDosyasiGetir(DataSet dataSet)
        {
            MemoryStream ms = new();
            using (var spreadSheetDocument = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //Çalışma kitabı ekleme parametreleri
                WorkbookPart workbookPart = spreadSheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorkbookStylesPart workbookStylesPart = spreadSheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
                workbookStylesPart.Stylesheet = GenerateStylesheet2();
                workbookStylesPart.Stylesheet.Save();

                Sheets sheets = spreadSheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());


                uint sheetId = 0;

                //Sayfa eklemeleri
                foreach (DataTable dt in dataSet.Tables)
                {
                    sheetId++;

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    SheetData sheetData = new();
                    worksheetPart.Worksheet = new Worksheet(sheetData);


                    Sheet sheet = new Sheet()
                    {
                        Id = spreadSheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = sheetId,
                        Name = dt.TableName
                    };

                    //İlk satır kolonlar
                    Row headerRow = new();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName != "STYLE")
                        {
                            headerRow.AppendChild(CreateCell(dc.Caption, true));
                        }
                    }
                    sheetData.AppendChild(headerRow);




                    //Veri satırları 
                    foreach (DataRow dr in dt.Rows)
                    {
                        Row row = new();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.ColumnName == "STYLE")
                            {
                                if (dr[dc.ColumnName] != null)
                                {
                                    var cells = row.ChildElements.Where(x => x.GetType() == typeof(Cell)).ToList();
                                    for (int i = 0; i < cells.Count; i++)
                                    {
                                        StyleParse((Cell)cells[i], dr[dc.ColumnName].ToString(), dt.Columns[i].Caption);
                                    }
                                }

                            }
                            else
                            {
                                row.AppendChild(CreateCell(dr[dc.ColumnName], false));
                            }
                        }
                        sheetData.AppendChild(row);
                    }



                    sheets.AppendChild(sheet);
                }

                workbookPart.Workbook.Save();

                spreadSheetDocument.Close();
            }



            return ms.ToArray();
        }

        #region Ek parçalar

        private void StyleParse(Cell cell, string styles, string columnCaption)
        {

            string[] styleMaster = styles.Split('|');
            foreach (string styleDetail in styleMaster)
            {
                var style = styleDetail.Split(':');
                if (style.Length == 2)
                {
                    if (style[0] == "ROW")
                    {
                        cell.StyleIndex = style[1] switch
                        {
                            "BLUE" => 6,
                            "YELLOW" => 9,
                            "GREEN" => 7,
                            "ORANGE" => 8,
                            _ => cell.StyleIndex
                        };
                    }

                    if (style[0] == columnCaption)
                    {
                        cell.StyleIndex = style[1] switch
                        {
                            "BLUE" => 6,
                            "YELLOW" => 9,
                            "GREEN" => 7,
                            "ORANGE" => 8,
                            _ => cell.StyleIndex
                        };
                    }

                }
            }

        }


        private Cell CreateCell(dynamic content, bool isHeaderRow)
        {
            try
            {
                Cell cell = new();
                uint headerStyleIndex = 5U;

                if (content.GetType() == typeof(TimeSpan))
                {
                    content = TimeSpanToString(content);
                }
                else if(content.GetType()== typeof(DateTime))
                {
                    content = Convert.ToDateTime(content.ToString());
                }

                cell.DataType = ResolveCellDataType(content.GetType());
                cell.StyleIndex = (isHeaderRow) ? headerStyleIndex : GetStyleIndex(content.GetType());
                cell.CellValue = new CellValue(content);
                return cell;
            }
            catch
            {
                return new Cell();
            }
        }

        private EnumValue<CellValues> ResolveCellDataType(Type type)
        {

            if (type == typeof(DateTime))
            {
                return CellValues.Date;
            }
            else if (type == typeof(double) || type == typeof(decimal) || type == typeof(int))
            {
                return CellValues.Number;
            }
            else if (type == typeof(bool))
            {
                return CellValues.Boolean;
            }
            else
            {
                return CellValues.String;
            }

        }

        private uint GetStyleIndex(Type type)
        {
            uint indexNo = 0U;
            if (type == typeof(decimal) || type == typeof(double))
            {
                indexNo = 3U;
            }
            else if (type == typeof(DateTime))
            {
                indexNo = 1U;
            }
            return indexNo;
        }

        private string TimeSpanToString(TimeSpan content)
        {
            List<string> veri = new();
            if (content.Days > 0)
                veri.Add($"{content.Days} G");
            if (content.Hours > 0)
                veri.Add($"{content.Hours} Sa");
            if (content.Minutes > 0)
                veri.Add($"{content.Minutes} Dk");
            if (content.Seconds > 0)
                veri.Add($"{content.Seconds} San");

            return string.Join(" ", veri.ToArray());
        }

        /// <summary>
        /// Stil sayfasını oluşturmak için kullanılan method.
        /// 1. Stil sayfası içinde font vb tüm varyasyonlar hazırlanır.
        /// 2. Bu varyasyonlar CellFormat sınıfında kullanılarak birden fazla formatta cellformat lar hazırlanır.
        /// 3. Cell oluşturulurken CellFormatlardan kullanılacak olanın index numarasını StyleIndex ile kullanarak istenen format
        ///    kullanılmış olur.
        /// </summary>
        /// <returns></returns>
        private Stylesheet GenerateStylesheet2()
        {

            Stylesheet ss = new Stylesheet();

            //Normal yazı 0
            Fonts fts = new Fonts();
            DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            ft.FontName = new FontName() { Val = "Calibri" };
            ft.FontSize = new FontSize() { Val = 11 };
            ft.Bold = new Bold() { Val = false };
            fts.Append(ft);

            //Başlık için 1
            DocumentFormat.OpenXml.Spreadsheet.Font ft2 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            ft2.FontName = new FontName() { Val = "Calibri" };
            ft2.FontSize = new FontSize() { Val = 11 };
            ft2.Bold = new Bold() { Val = true };
            fts.Append(ft2);

            fts.Count = (uint)fts.ChildElements.Count;

            Fills fills = new Fills();
            Fill fill;
            PatternFill patternFill;

            //Normal 0 
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.None;
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Gray 125 1
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Gray125;
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Başlık için sarı boya 2
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Solid;
            patternFill.ForegroundColor = new ForegroundColor() { Rgb = "FFFF00" };
            patternFill.BackgroundColor = new BackgroundColor() { Indexed = 64U };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Başlık için lightblue boya 3
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Solid;
            patternFill.ForegroundColor = new ForegroundColor() { Rgb = "99CCFF" };
            patternFill.BackgroundColor = new BackgroundColor() { Indexed = 64U };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Başlık için lightgreen boya 4
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Solid;
            patternFill.ForegroundColor = new ForegroundColor() { Rgb = "99FF99" };
            patternFill.BackgroundColor = new BackgroundColor() { Indexed = 64U };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Başlık için orange boya 5
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Solid;
            patternFill.ForegroundColor = new ForegroundColor() { Rgb = "FF9933" };
            patternFill.BackgroundColor = new BackgroundColor() { Indexed = 64U };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            fills.Count = (uint)fills.ChildElements.Count;

            Borders borders = new Borders();
            Border border = new Border();
            border.LeftBorder = new LeftBorder();
            border.RightBorder = new RightBorder();
            border.TopBorder = new TopBorder();
            border.BottomBorder = new BottomBorder();
            border.DiagonalBorder = new DiagonalBorder();
            borders.Append(border);
            borders.Count = (uint)borders.ChildElements.Count;

            //****** Yukarıdaki varyasyonları aşağıdaki cellformatlarda kullan ve istenen formatın indeks (sıra numarası) StyleIndex olarak gönder.

            CellStyleFormats csfs = new CellStyleFormats();
            CellFormat cf = new CellFormat();
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            csfs.Append(cf);
            csfs.Count = (uint)csfs.ChildElements.Count;

            uint iExcelIndex = 164;
            NumberingFormats nfs = new NumberingFormats();
            CellFormats cfs = new CellFormats();

            cf = new CellFormat(); // 0 Normal
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cfs.Append(cf);

            NumberingFormat nf;
            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "dd/MM/yyyy HH:mm:ss";
            nfs.Append(nf);
            cf = new CellFormat(); // 1 Date format
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "#,##0.0000";
            nfs.Append(nf);
            cf = new CellFormat(); // 2 Number format 4 dec.
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            // #,##0.00 is also Excel style index 4
            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "#,##0.00";
            nfs.Append(nf);
            cf = new CellFormat(); // 3 Number format
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            // @ is also Excel style index 49
            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "@";
            nfs.Append(nf);
            cf = new CellFormat(); // 4 different format
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            cf = new CellFormat(); // 5 For header format - yellow
            cf.NumberFormatId = 0;
            cf.FontId = 1;
            cf.FillId = 2;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cfs.Append(cf);

            cf = new CellFormat(); // 6 lightblue
            cf.NumberFormatId = 166;
            cf.FontId = 0;
            cf.FillId = 3;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            cf = new CellFormat(); // 7 lightgreen
            cf.NumberFormatId = 166;
            cf.FontId = 0;
            cf.FillId = 4;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            cf = new CellFormat(); // 8 orange
            cf.NumberFormatId = 166;
            cf.FontId = 0;
            cf.FillId = 5;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            cf = new CellFormat(); // 9 yellow
            cf.NumberFormatId = 166;
            cf.FontId = 0;
            cf.FillId = 2;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);


            nfs.Count = (uint)nfs.ChildElements.Count;
            cfs.Count = (uint)cfs.ChildElements.Count;

            ss.Append(nfs);
            ss.Append(fts);
            ss.Append(fills);
            ss.Append(borders);
            ss.Append(csfs);
            ss.Append(cfs);

            CellStyles css = new CellStyles();
            CellStyle cs = new CellStyle();
            cs.Name = "Normal";
            cs.FormatId = 0;
            cs.BuiltinId = 0;
            css.Append(cs);
            css.Count = (uint)css.ChildElements.Count;
            ss.Append(css);

            DifferentialFormats dfs = new DifferentialFormats();
            dfs.Count = 0;
            ss.Append(dfs);

            TableStyles tss = new TableStyles();
            tss.Count = 0;
            tss.DefaultTableStyle = "TableStyleMedium9";
            tss.DefaultPivotStyle = "PivotStyleLight16";
            ss.Append(tss);

            return ss;
        }

        #endregion

    }
}
