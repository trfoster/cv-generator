using System.Collections.Concurrent;
using System.Net.Mime;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

QuestPDF.Settings.License = LicenseType.Community;
const int defaultFontSize = 12;

Document.Create(container => {
    container.Page(page => {
        page.Size(PageSizes.A4);
        page.Margin(1.5f, Unit.Centimetre);
        page.DefaultTextStyle(t => t.FontSize(defaultFontSize).FontFamily("Times New Roman"));
        
        page.Header().Column(column => {
                column.Item().Text("Tom Foster").Bold().FontSize(30).AlignCenter();
                column.Item().AlignCenter().Row(row => {
                    row.AutoItem().Text("trfoster1794@gmail.com");
                    row.AutoItem()
                        .PaddingHorizontal(10)
                        .LineVertical(1);
                    row.AutoItem().Text("+44 7378 343 998");
                    row.AutoItem()
                        .PaddingHorizontal(10)
                        .LineVertical(1);
                    row.AutoItem().Text(t => {
                        t.Hyperlink("github.com/trfoster", "https://github.com/trfoster").Underline()
                            .FontColor(Colors.Blue.Medium);
                    });
                });
        });
            

        page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(column => {
            column.Title("Education");
            column.DatedItem("University of Sheffield", "2023 - 2026", 
                "BEng Software Engineering",
                points: [("Relevant modules: ","Cybersecurity, Cryptography, Software Reengineering, Software Testing"),
                ("Dissertation: ", "")]);
            column.Title("skills");
            column.Item().PaddingBottom(15).Text("C#, Java, MVC, HTML, CSS, Ruby, Python, Haskell, JS, .NET, SQL, Fluent Spanish");
            column.Title("Experience");
            
        });

        page.Footer().AlignCenter().Text("Really nice footer");
    });
}).ShowInCompanion();

public static class Extensions {
    public static void Title(this ColumnDescriptor column, string title) {
        column.Item().Text(title.ToUpper()).FontSize(15).Bold();
        column.Item().PaddingVertical(5).LineHorizontal(1);
    }

    public static void DatedItem(this ColumnDescriptor column, string company, string? date = null, string? role = null, string? location = null,
        (string?, string)[]? points = null) {
        column.Item().Row(row => {
            row.RelativeItem().Text(company).Bold();
            if (date is not null) {
                row.AutoItem().Text(date).Bold();
            }
        });
        if (role is not null ||  location is not null) {
            column.Item().Row(row => {
                if (role is not null) {
                    row.RelativeItem().Text(role);
                }
                if (location is not null) {
                    row.AutoItem().Text(location).Italic();
                }
            });
        }

        if (points is not null && points.Length > 0) {
            for (int i = 0; i < points.Length; i++) {
                if (points[i].Item1 is not null) {
                    column.Item().Text("• " + points[i]);
                }
                column.Item().Text("• " + points[i]);
            }
        }
    }
}