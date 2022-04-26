//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using OraManagement_;
//using System.Linq.Dynamic;
//using System.ComponentModel;
//using System.Data.SqlClient;

//namespace Common.Engine
//{
//    public class ReportEngine
//    {
//        public HSSFWorkbook CreateOracle(ReportModel reportModel) 
//        {
//            HSSFWorkbook workBook = new HSSFWorkbook();
//            //if (reportModel == null)
//            //    throw new Exception("خطا:Report model is null");
//            //if (!reportModel.ReportNumber.HasValue)
//            //    throw new Exception("خطا:ReportNumber must be set");
//            //string conString = OraConfig.getConStr("!CUB6HeE6i51wYGWKgU34yB");
//            //using (OracleConnection con = new OracleConnection(conString))
//            //{
//            //    con.Open();
//            //    OracleCommand cmd = new OracleCommand("select SelectExpr, Group1Expr, Group2Expr from kar_report where id = " + reportModel.ReportNumber.Value, con);
//            //    OracleDataReader reader = cmd.ExecuteReader();
//            //    string strSelect = "";
//            //    string group1Expr = null;
//            //    string group2Expr = null;
//            //    if (reader.Read())
//            //    {
//            //        strSelect = reader.GetString(0);
//            //        group1Expr = reader.GetString(1);
//            //        group2Expr = reader.GetString(2);
//            //    }
//            //    else
//            //        throw new MyException("گزارش از سیستم حدف شده است برای نمایش گزارشهای موجود در سیستم دکمه بروزرسانی را بزنید.", 15);
//            //    if (reader != null)
//            //        reader.Close();
//            //    ICellStyle style1 = workBook.CreateCellStyle();
//            //    var format = workBook.CreateDataFormat();
//            //    style1.DataFormat = format.GetFormat("text");
//            //    IFont font = workBook.CreateFont();
//            //    font.Color = NPOI.HSSF.Util.HSSFColor.WHITE.index;
//            //    font.Boldweight = (short)FontBoldWeight.BOLD;
//            //    style1.SetFont(font);
//            //    style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BLUE_GREY.index;
//            //    style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
//            //    style1.Alignment = HorizontalAlignment.CENTER;
//            //    ISheet sheet1 = workBook.CreateSheet("Sheet1");
//            //    sheet1.CreateFreezePane(0, 1, 0, 1);
//            //    int colCount;
//            //    IList<string> listEn = new List<string>();
//            //    using (OracleConnection con1 = new OracleConnection(conString))
//            //    {
//            //        con1.Open();
//            //        cmd = new OracleCommand("select TitleFa, TitleEn from kar_reportparam where reportid=" + reportModel.ReportNumber.Value, con1);
//            //        reader = cmd.ExecuteReader();
//            //        IRow row = sheet1.CreateRow(0);
//            //        var headerCell = row.CreateCell(0);
//            //        headerCell.CellStyle = style1;
//            //        headerCell.SetCellValue("ردیف");
//            //        int j = 1;
//            //        while (reader.Read())
//            //        {
//            //            string enTitle = reader.GetString(1);
//            //            if (enTitle != reportModel.Field1 && enTitle != reportModel.Field2)
//            //            {
//            //                sheet1.AutoSizeColumn(j);
//            //                headerCell = row.CreateCell(j);
//            //                headerCell.SetCellValue(reader.GetString(0));
//            //                enTitle = enTitle.Substring(enTitle.LastIndexOf('.') + 1);
//            //                listEn.Add(enTitle);
//            //                headerCell.CellStyle = style1;
//            //                j++;
//            //            }
//            //        }
//            //        colCount = j;
//            //        if (reader != null)
//            //            reader.Close();
//            //        if (con1 != null)
//            //            con1.Close();
//            //    }
//            //    IList<object> objectList = new List<object>();
//            //    if (!string.IsNu1llOrEmpty(reportModel.Field1))
//            //        if (!strSelect.Contains(reportModel.Field1))
//            //        {
//            //            var index = strSelect.IndexOf('(') + 1;
//            //            strSelect = strSelect.Insert(index, reportModel.Field1 + ",");
//            //        }
//            //    if (!string.IsNul1lOrEmpty(reportModel.Field2))
//            //        if (!strSelect.Contains(reportModel.Field2))
//            //        {
//            //            var index = strSelect.IndexOf('(') + 1;
//            //            strSelect = strSelect.Insert(index, reportModel.Field2 + ",");
//            //        }
//            //    var type = reportModel.Data.AsQueryable().ElementType;
//            //    if (string.IsNullOrEmpty(group1Expr))
//            //        reportModel.Data = reportModel.Data.AsQueryable().Select_(strSelect);
//            //    else
//            //        reportModel.Data = reportModel.Data.AsQueryable().GroupBy_(group1Expr, group2Expr, null).Select_(strSelect);
//            //    int i = 1;
//            //    IList<Type> types = new List<Type>();
//            //    if (!string.IsNul1lOrEmpty(reportModel.Field1))
//            //    {
//            //        var attr = type.GetMyProperty(reportModel.Field1).GetCustomAttributes(typeof(DisplayNameAttribute), false);
//            //        if (attr == null || attr.Count() == 0)
//            //            throw new MyException("خطا:" + "DisplayName Attribute not set", 8);
//            //        var title1 = (attr[0] as DisplayNameAttribute).DisplayName;
//            //        string title2 = null;
//            //        if (!string.IsNu1llOrEmpty(reportModel.Field2))
//            //        {
//            //            attr = type.GetMyProperty(reportModel.Field2).GetCustomAttributes(typeof(DisplayNameAttribute), false);
//            //            if (attr == null || attr.Count() == 0)
//            //                throw new MyException("خطا:" + "DisplayName Attribute not set", 9);
//            //            title2 = (attr[0] as DisplayNameAttribute).DisplayName;
//            //        }
//            //        OfficeReport officerReport = new OfficeReport(reportModel, workBook);
//            //        officerReport.Grouping2(title1, title2, colCount - 1, listEn);
//            //    }
//            //    else
//            //    {
//            //        foreach (var item in reportModel.Data)
//            //        {
//            //            IRow row = sheet1.CreateRow(i);
//            //            row.CreateCell(0).SetCellValue(i);
//            //            int j = 1;
//            //            foreach (var str in listEn)
//            //            {
//            //                var pro = item.GetType().GetProperty(str);
//            //                var tempType = pro.PropertyType;
//            //                if (tempType.IsNullable())
//            //                    tempType = Nullable.GetUnderlyingType(tempType);
//            //                var value = pro.GetValue(item, null);
//            //                if (tempType.IsEnum)
//            //                {
//            //                    value = value.GetName();
//            //                    tempType = typeof(string);
//            //                }
//            //                if (tempType == typeof(string))
//            //                    row.CreateCell(j, CellType.STRING).SetCellValue(Convert.ToString(value));
//            //                else
//            //                    if (tempType == typeof(DateTime))
//            //                    {
//            //                        DateTime? date = (DateTime?)value;
//            //                        row.CreateCell(j, CellType.STRING).SetCellValue(date.ToPersianDate());
//            //                    }
//            //                    else
//            //                        row.CreateCell(j, CellType.NUMERIC).SetCellValue(Convert.ToDouble(value));
//            //                j++;
//            //                if (i == 1)
//            //                    types.Add(tempType);
//            //            }
//            //            i++;
//            //        }
//            //        style1 = workBook.CreateCellStyle();
//            //        style1.BorderTop = BorderStyle.THIN;
//            //        style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
//            //        style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
//            //        IRow final = sheet1.CreateRow(i);
//            //        for (int j = 0; j < colCount; j++)
//            //        {
//            //            var finalCell = final.CreateCell(j);
//            //            finalCell.CellStyle = style1;
//            //            if (j == 0)
//            //                finalCell.SetCellValue("جمع");
//            //            else
//            //                if (types.Any() && types.ElementAt(j - 1) != typeof(string) && types.ElementAt(j - 1) != typeof(DateTime))
//            //                    finalCell.CellFormula = "Sum(" + (char)(65 + j) + "2:" + (char)(65 + j) + i.ToString() + ')';
//            //        }
//            //    }
//            //}
//            return workBook;
//        }

//        public HSSFWorkbook CreateSql(ReportModel reportModel)
//        {
//            HSSFWorkbook workBook = new HSSFWorkbook();
//            if (reportModel == null)
//                throw new Exception("خطا:Report model is null");
//            if (!reportModel.ReportNumber.HasValue)
//                throw new Exception("خطا:ReportNumber must be set");
//            string conString = "Server=ALI-PC-PC\\SQLEXPRESS;DataBase=Test;Integrated Security=true";
//            using (SqlConnection con = new SqlConnection(conString))
//            {
//                con.Open();
//                SqlCommand cmd = new SqlCommand("select SelectExpr, Group1Expr, Group2Expr from kar_report where id = " + reportModel.ReportNumber.Value, con);
//                SqlDataReader reader = cmd.ExecuteReader();
//                string strSelect = "";
//                string group1Expr = null;
//                string group2Expr = null;
//                if (reader.Read())
//                {
//                    strSelect = reader.GetString(0);
//                    group1Expr = Convert.ToString(reader.GetValue(1));
//                    group2Expr = Convert.ToString(reader.GetValue(2));
//                }
//                else
//                    throw new MyException("گزارش از سیستم حدف شده است برای نمایش گزارشهای موجود در سیستم دکمه بروزرسانی را بزنید.", 15);
//                if (reader != null)
//                    reader.Close();
//                ICellStyle style1 = workBook.CreateCellStyle();
//                var format = workBook.CreateDataFormat();
//                style1.DataFormat = format.GetFormat("text");
//                IFont font = workBook.CreateFont();
//                font.Color = NPOI.HSSF.Util.HSSFColor.WHITE.index;
//                font.Boldweight = (short)FontBoldWeight.BOLD;
//                style1.SetFont(font);
//                style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BLUE_GREY.index;
//                style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
//                style1.Alignment = HorizontalAlignment.CENTER;
//                ISheet sheet1 = workBook.CreateSheet("Sheet1");
//                sheet1.CreateFreezePane(0, 1, 0, 1);
//                int colCount;
//                IList<string> listEn = new List<string>();
//                using (SqlConnection con1 = new SqlConnection(conString))
//                {
//                    con1.Open();
//                    cmd = new SqlCommand("select TitleFa, TitleEn from kar_reportparam where Reportid=" + reportModel.ReportNumber.Value, con1);
//                    reader = cmd.ExecuteReader();
//                    IRow row = sheet1.CreateRow(0);
//                    var headerCell = row.CreateCell(0);
//                    headerCell.CellStyle = style1;
//                    headerCell.SetCellValue("ردیف");
//                    int j = 1;
//                    while (reader.Read())
//                    {
//                        string enTitle = reader.GetString(1);
//                        if (enTitle != reportModel.Field1 && enTitle != reportModel.Field2)
//                        {
//                            sheet1.AutoSizeColumn(j);
//                            headerCell = row.CreateCell(j);
//                            headerCell.SetCellValue(reader.GetString(0));
//                            listEn.Add(enTitle);
//                            headerCell.CellStyle = style1;
//                            j++;
//                        }
//                    }
//                    colCount = j;
//                    if (reader != null)
//                        reader.Close();
//                    if (con1 != null)
//                        con1.Close();
//                }
//                IList<object> objectList = new List<object>();
//                if (reportModel.Field1.HasValue())
//                    if (!strSelect.Contains(reportModel.Field1))
//                    {
//                        var index = strSelect.IndexOf('(') + 1;
//                        strSelect = strSelect.Insert(index, reportModel.Field1 + ",");
//                    }
//                if (reportModel.Field2.HasValue())
//                    if (!strSelect.Contains(reportModel.Field2))
//                    {
//                        var index = strSelect.IndexOf('(') + 1;
//                        strSelect = strSelect.Insert(index, reportModel.Field2 + ",");
//                    }
//                var type = reportModel.Data.AsQueryable().ElementType;
//                if (string.IsNullOrEmpty(group1Expr))
//                    reportModel.Data = reportModel.Data.AsQueryable().Select_(strSelect);
//                else
//                    reportModel.Data = reportModel.Data.AsQueryable().GroupBy_(group1Expr, group2Expr, null).Select_(strSelect);
//                int i = 1;
//                IList<Type> types = new List<Type>();
//                if (reportModel.Field1.HasValue())
//                {
//                    var attr = type.GetMyProperty(reportModel.Field1).GetCustomAttributes(typeof(DisplayNameAttribute), false);
//                    if (attr == null || attr.Count() == 0)
//                        throw new MyException("خطا:" + "DisplayName Attribute not set", 8);
//                    var title1 = (attr[0] as DisplayNameAttribute).DisplayName;
//                    string title2 = null;
//                    if (reportModel.Field2.HasValue())
//                    {
//                        attr = type.GetMyProperty(reportModel.Field2).GetCustomAttributes(typeof(DisplayNameAttribute), false);
//                        if (attr == null || attr.Count() == 0)
//                            throw new MyException("خطا:" + "DisplayName Attribute not set", 9);
//                        title2 = (attr[0] as DisplayNameAttribute).DisplayName;
//                    }
//                    OfficeReport officerReport = new OfficeReport(reportModel, workBook);
//                    officerReport.Grouping2(title1, title2, colCount - 1, listEn);
//                }
//                else
//                {
//                    foreach (var item in reportModel.Data)
//                    {
//                       IRow row = sheet1.CreateRow(i);
//                        row.CreateCell(0).SetCellValue(i);
//                        int j = 1; 
//                        foreach (var str in listEn)
//                        {
//                            var pro = item.GetType().GetMyProperty(str);
//                            var tempType = pro.PropertyType;
//                            if (tempType.IsNullable())
//                                tempType = Nullable.GetUnderlyingType(tempType);
//                            var value = item.GetMyValue(str);
//                            if (tempType.IsEnum)
//                            {
//                                value = value.GetName();
//                                tempType = typeof(string);
//                            }
//                            if (tempType == typeof(string))
//                                row.CreateCell(j, CellType.STRING).SetCellValue(Convert.ToString(value));
//                            else
//                                if (tempType == typeof(DateTime))
//                                {
//                                    DateTime? date = (DateTime?)value;
//                                    row.CreateCell(j, CellType.STRING).SetCellValue(date.ToPersianDate());
//                                }
//                                else
//                                    row.CreateCell(j, CellType.NUMERIC).SetCellValue(Convert.ToDouble(value));
//                            j++;
//                            if (i == 1)
//                                types.Add(tempType);
//                        }
//                        i++;
//                    }
//                    style1 = workBook.CreateCellStyle();
//                    style1.BorderTop = BorderStyle.THIN;
//                    style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
//                    style1.FillPattern = FillPatternType.SOLID_FOREGROUND;
//                    IRow final = sheet1.CreateRow(i);
//                    for (int j = 0; j < colCount; j++)
//                    {
//                        var finalCell = final.CreateCell(j);
//                        finalCell.CellStyle = style1;
//                        if (j == 0)
//                            finalCell.SetCellValue("جمع");
//                        else
//                            if (types.Any() && types.ElementAt(j - 1) != typeof(string) && types.ElementAt(j - 1) != typeof(DateTime))
//                                finalCell.CellFormula = "Sum(" + (char)(65 + j) + "2:" + (char)(65 + j) + i.ToString() + ')';
//                    }
//                }
//            }
//            return workBook;
//        }    
//    }
//}
