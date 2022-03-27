using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _01electronics_crm
{
    class ExcelExport
    {
        protected Grid gridToExport;

        Microsoft.Office.Interop.Excel.Application excelApplication;
        Microsoft.Office.Interop.Excel.Worksheet workSheet;
        Microsoft.Office.Interop.Excel.Workbook workBook;

        FrameworkElement currentStyle;

        BrushConverter brush;
        public ExcelExport()
        {

        }

        public ExcelExport(Grid mGridToExport)
        {
            gridToExport = mGridToExport;

            excelApplication = new Microsoft.Office.Interop.Excel.Application();
            workBook = excelApplication.Workbooks.Add();
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;

            currentStyle = new FrameworkElement();

            brush = new BrushConverter();

            workSheet.Columns.EntireColumn.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            workSheet.Columns.EntireColumn.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            workSheet.Columns.EntireColumn.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbMidnightBlue;

            int rowCount = 0;
            int currentRow = 0;
            int columnCount = gridToExport.ColumnDefinitions.Count;
            int columnsAdded = 0;
            int childGridRows = 0;

            for (int i = 0; i < gridToExport.RowDefinitions.Count; i++, rowCount++)
            {
                bool gridInserted = false;

                for (int j = 0; j < gridToExport.ColumnDefinitions.Count; j++)
                {

                    if (gridToExport.Children[i * gridToExport.ColumnDefinitions.Count + j].GetType().Equals(typeof(Label)))
                    {
                        Label currentLabel = (Label)gridToExport.Children[i * gridToExport.ColumnDefinitions.Count + j];

                        if (currentLabel.Content != null && gridInserted == true)
                        {
                            workSheet.Range["A1"].Offset[rowCount - childGridRows, j + columnsAdded].Value = currentLabel.Content.ToString();
                            SetCellStyle(currentLabel, rowCount - childGridRows, j + columnsAdded);
                        }
                        else if (currentLabel.Content != null)
                        {
                            workSheet.Range["A1"].Offset[rowCount, j].Value = currentLabel.Content.ToString();
                            SetCellStyle(currentLabel, rowCount, j);
                        }

                    }
                    else if (gridToExport.Children[i * gridToExport.ColumnDefinitions.Count + j].GetType().Equals(typeof(Grid)) && gridInserted == false)
                    {
                        Grid childGrid = (Grid)gridToExport.Children[i * gridToExport.ColumnDefinitions.Count + j];

                        childGridRows = childGrid.RowDefinitions.Count - 1;

                        if (i == 1)
                        {
                            for (int l = 0; l < childGrid.ColumnDefinitions.Count - 1; l++)
                            {
                                workSheet.Range["A1"].Offset[rowCount, j + l + 1].EntireColumn.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, Microsoft.Office.Interop.Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                            }
                        }

                        for (int k = 0; k < childGrid.RowDefinitions.Count; k++, rowCount++)
                        {
                            for (int m = 0; m < childGrid.ColumnDefinitions.Count; m++)
                            {
                                Label currentChildGridLabel = (Label)childGrid.Children[k * childGrid.ColumnDefinitions.Count + m];

                                if (currentChildGridLabel.Content != null)
                                    workSheet.Range["A1"].Offset[rowCount, j + m].Value = currentChildGridLabel.Content.ToString();
                                else
                                {
                                    currentChildGridLabel.Content = "  ";
                                    workSheet.Range["A1"].Offset[rowCount, j + m].Value = currentChildGridLabel.Content.ToString();
                                }
                                SetCellStyle(currentChildGridLabel, rowCount, j + m);
                            }

                        }

                        gridInserted = true;

                        rowCount--;

                        if (i == 1)
                        {
                            columnsAdded = childGrid.ColumnDefinitions.Count - 1;
                            columnCount += columnsAdded;

                            workSheet.Range[workSheet.Range["A1"].Offset[0, j], workSheet.Range["A1"].Offset[0, j + columnsAdded]].Merge();

                        }

                        for (int m = 0; m < gridToExport.ColumnDefinitions.Count; m++)
                        {
                            if (m == j)
                                continue;

                            else if (m < j)
                            {
                                if (gridToExport.Children[i * gridToExport.ColumnDefinitions.Count + m].GetType().Equals(typeof(Label)))
                                    workSheet.Range[workSheet.Range["A1"].Offset[currentRow, m], workSheet.Range["A1"].Offset[currentRow + childGridRows, m]].Merge();
                            }
                            else
                            {
                                if (gridToExport.Children[i * gridToExport.ColumnDefinitions.Count + m].GetType().Equals(typeof(Label)))
                                    workSheet.Range[workSheet.Range["A1"].Offset[currentRow, m + columnsAdded], workSheet.Range["A1"].Offset[currentRow + childGridRows, m + columnsAdded]].Merge();
                            }
                        }
                        currentRow += childGridRows;
                    }
                }

                currentRow++;
            }

            excelApplication.Visible = true;
            workSheet.Columns.AutoFit();
            workBook.Activate();
        }

        private void SetCellStyle(Label currentLabel, int row, int column)
        {
            if (currentLabel.Style == (Style)currentStyle.FindResource("tableSubItemLabel"))
                SetTableSubItemLabelStyle(row, column);
            else if (currentLabel.Style == (Style)currentStyle.FindResource("tableHeaderItem"))
                SetTableHeaderItemStyle(row, column);
            else if (currentLabel.Style == (Style)currentStyle.FindResource("tableSubHeaderItem"))
                SetTableSubHeaderItemStyle(row, column);
        }
        private void SetTableSubItemLabelStyle(int row, int column)
        {
            workSheet.Range["A1"].Offset[row, column].Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
            workSheet.Range["A1"].Offset[row, column].Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbMidnightBlue;
            workSheet.Range["A1"].Offset[row, column].Font.Bold = true;
            workSheet.Range["A1"].Offset[row, column].Font.Size = 16;
        }

        private void SetTableSubHeaderItemStyle(int row, int column)
        {
            workSheet.Range["A1"].Offset[row, column].Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbMidnightBlue;
            workSheet.Range["A1"].Offset[row, column].Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
            workSheet.Range["A1"].Offset[row, column].Font.Bold = true;
            workSheet.Range["A1"].Offset[row, column].Font.Size = 16;
        }

        private void SetTableHeaderItemStyle(int row, int column)
        {
            workSheet.Range["A1"].Offset[row, column].Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbMidnightBlue;
            workSheet.Range["A1"].Offset[row, column].Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
            workSheet.Range["A1"].Offset[row, column].Font.Bold = true;
            workSheet.Range["A1"].Offset[row, column].Font.Size = 16;
            workSheet.Range["A1"].Offset[row, column].RowHeight = 40;
        }
    }
}
