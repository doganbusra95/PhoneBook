using ClosedXML.Excel;
using System.Data;

namespace PhoneBookSharedTools.ExcelFileWorks._ClosedXML
{
    public class ExcelFileWorks_ClosedXML : IExcelFileWorks
    {
        public byte[] ExcelDosyasiGetir(DataSet dataSet)
        {
            using (var wbook = new ClosedXML.Excel.XLWorkbook())
            {

                //Sayfalar
                foreach (DataTable dt in dataSet.Tables)
                {
                    var raporSheet = wbook.Worksheets.Add(dt.TableName);

                    int x = 1, y = 1;

                    //İlk satır kolonlar
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName != "STYLE")
                        {
                            raporSheet.Cell(x, y++).Value = dc.Caption;
                        }
                    }

                    raporSheet.Range(x, x, x, y).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                    raporSheet.Range(x, x, x, y).Style.Font.SetBold(true);

                    //Veri satırları
                    foreach (DataRow dr in dt.Rows)
                    {
                        x++;
                        y = 1;

                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.ColumnName == "STYLE")
                            {
                                if (dr[dc.ColumnName] != null)
                                {
                                    for (int i = 1; i < y - 1; i++)
                                    {
                                        StyleParse(raporSheet.Cell(x, i), dr[dc.ColumnName].ToString(), dt.Columns[i - 1].Caption);
                                    }
                                }
                            }
                            else
                            {
                                raporSheet.Cell(x, y++).Value = dr[dc.ColumnName];

                                if (dc.DataType == typeof(decimal))
                                {
                                    raporSheet.Cell(x, y - 1).Style.NumberFormat.SetFormat("#,##0.00");
                                }
                            }
                        }
                    }

                    raporSheet.Columns().AdjustToContents();

                }


                MemoryStream stream = new MemoryStream();
                wbook.SaveAs(stream);

                var bytes = stream.ToArray();
                return bytes;
            }

        }

        #region Ek Parçalar

        private void StyleParse(IXLCell cell, string styles, string columnCaption)
        {
            string[] styleMaster = styles.Split('|');
            foreach (string styleDetail in styleMaster)
            {
                var style = styleDetail.Split(':');
                if (style.Length == 2)
                {
                    if (style[0] == "ROW")
                    {
                        cell.Style.Fill.BackgroundColor = style[1] switch
                        {
                            "BLUE" => XLColor.LightBlue,
                            "YELLOW" => XLColor.Yellow,
                            "GREEN" => XLColor.LightGreen,
                            "ORANGE" => XLColor.Orange,
                            _ => XLColor.WhiteSmoke
                        };
                    }

                    if (style[0] == columnCaption)
                    {
                        cell.Style.Fill.BackgroundColor = style[1] switch
                        {
                            "BLUE" => XLColor.LightBlue,
                            "YELLOW" => XLColor.Yellow,
                            "GREEN" => XLColor.LightGreen,
                            "ORANGE" => XLColor.Orange,
                            _ => XLColor.WhiteSmoke
                        };
                    }

                }
            }

        }

        #endregion


    }
}
