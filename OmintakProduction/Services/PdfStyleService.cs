using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace OmintakProduction.Services
{
    public class PdfStyleService
    {
        // Font properties
        private static readonly PdfFont BOLD_FONT = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        private static readonly PdfFont REGULAR_FONT = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        // Colors
        private static readonly Color PRIMARY_COLOR = ColorConstants.BLUE;
        private static readonly Color SECONDARY_COLOR = ColorConstants.GRAY;
        private static readonly Color SUCCESS_COLOR = ColorConstants.GREEN;
        private static readonly Color WARNING_COLOR = ColorConstants.ORANGE;
        private static readonly Color DANGER_COLOR = ColorConstants.RED;

        public static void ApplyHeaderStyle(Paragraph header)
        {
            header.SetFontSize(20)
                  .SetFont(BOLD_FONT)
                  .SetFontColor(PRIMARY_COLOR)
                  .SetTextAlignment(TextAlignment.CENTER)
                  .SetMarginBottom(20);
        }

        public static void ApplySubHeaderStyle(Paragraph subheader)
        {
            subheader.SetFontSize(16)
                    .SetFont(BOLD_FONT)
                    .SetFontColor(SECONDARY_COLOR)
                    .SetMarginBottom(10);
        }

        public static void ApplyTableHeaderStyle(Table table)
        {
            table.SetWidth(UnitValue.CreatePercentValue(100))
                 .SetMarginBottom(20)
                 .SetBorder(new SolidBorder(1));

            foreach (Cell cell in table.GetHeader().GetChildren())
            {
                cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetFont(BOLD_FONT)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(8);
            }
        }

        public static Cell CreateStyledCell(string content, TextAlignment alignment = TextAlignment.LEFT)
        {
            return new Cell()
                .Add(new Paragraph(content).SetFont(REGULAR_FONT))
                .SetTextAlignment(alignment)
                .SetPadding(5);
        }

        public static void ApplyStatusStyle(Cell cell, string status)
        {
            status = status.ToLower();
            Color color = status switch
            {
                "open" or "todo" => DANGER_COLOR,
                "in progress" => WARNING_COLOR,
                "resolved" or "done" => SUCCESS_COLOR,
                _ => SECONDARY_COLOR
            };
            cell.SetFontColor(color);
        }

        public static void CreateStatisticBlock(Document document, string label, string value)
        {
            var statBlock = new Div()
                .SetBorder(new SolidBorder(1))
                .SetPadding(10)
                .SetMargin(5);

            statBlock.Add(new Paragraph(value)
                .SetFontSize(18)
                .SetFont(BOLD_FONT)
                .SetTextAlignment(TextAlignment.CENTER));

            statBlock.Add(new Paragraph(label)
                .SetFontSize(12)
                .SetFontColor(SECONDARY_COLOR)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(statBlock);
        }

        public static void ApplyDocumentSettings(Document document)
        {
            document.SetMargins(50, 50, 50, 50);
            document.SetFont(REGULAR_FONT);
        }

        public static Table CreateStandardTable(int numberOfColumns)
        {
            var table = new Table(numberOfColumns)
                .UseAllAvailableWidth()
                .SetMarginTop(15)
                .SetMarginBottom(15)
                .SetBorder(new SolidBorder(1));

            return table;
        }

        public static void AddDateTimeFooter(Document document)
        {
            document.Add(new Paragraph($"Generated on: {DateTime.Now:MMMM dd, yyyy HH:mm:ss}")
                .SetFontSize(8)
                .SetFontColor(SECONDARY_COLOR)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginTop(20));
        }
    }
}
