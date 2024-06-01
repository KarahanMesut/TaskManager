using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using TaskManager.Data;

namespace TaskManager.Presentation
{
    public static class ExcelExporter
    {
        public static byte[] ExportTasksToExcel(List<ToDoTask> tasks)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                worksheet.Cells[1, 1].Value = "Başlık";
                worksheet.Cells[1, 2].Value = "Açıklama";
                worksheet.Cells[1, 3].Value = "Tarih";
                worksheet.Cells[1, 4].Value = "Hatırlatma(dk)";
                worksheet.Cells[1, 5].Value = "Durum";

                for (int i = 0; i < tasks.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = tasks[i].TITLE;
                    worksheet.Cells[i + 2, 2].Value = tasks[i].DESCRIPTION;
                    worksheet.Cells[i + 2, 3].Value = tasks[i].DUE_DATE.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cells[i + 2, 4].Value = tasks[i].REMINDER_TIME;
                    worksheet.Cells[i + 2, 5].Value = tasks[i].IS_COMPLETED ? "Yes" : "No";
                }

                return package.GetAsByteArray();
            }
        }
    }
}
