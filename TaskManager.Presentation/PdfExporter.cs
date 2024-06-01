using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using TaskManager.Data;

namespace TaskManager.Presentation
{
    public static class PdfExporter
    {

        public static byte[] ExportTasksToPdf(List<ToDoTask> tasks)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Tasks Report"));

                var table = new Table(5, true);

                table.AddCell("Başlık");
                table.AddCell("Açıklama");
                table.AddCell("Tarih");
                table.AddCell("Hatırlatma(dk)");
                table.AddCell("Durum");

                foreach (var task in tasks)
                {
                    table.AddCell(task.TITLE);
                    table.AddCell(task.DESCRIPTION);
                    table.AddCell(task.DUE_DATE.ToString("dd.MM.yyyy HH:mm"));
                    table.AddCell(task.REMINDER_TIME.ToString());
                    table.AddCell(task.IS_COMPLETED ? "Yes" : "No");
                }

                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }
        }
    }
}
