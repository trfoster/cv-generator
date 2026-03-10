using System.Collections.Concurrent;
using System.Net.Mime;
using System.Text.Json;
using PDFGenerator;
using QuestPDF.Companion;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

QuestPDF.Settings.License = LicenseType.Community;
const int defaultFontSize = 12;

using var fontStream = File.OpenRead("../../../Times New Roman.ttf");
FontManager.RegisterFont(fontStream);

string json = File.ReadAllText("../../../document.json");
var cv = JsonSerializer.Deserialize<Root>(json)!;
    
CreateCv(cv).ShowInCompanion();
return;

Document CreateCv(Root document) {
    return Document.Create(container => {
        container.Page(page => {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);
            page.DefaultTextStyle(t => t.FontSize(defaultFontSize).FontFamily("Times New Roman"));

            page.Header().Column(column => {
                column.Item().Text(document.Title).Bold().FontSize(30).AlignCenter();
                column.Item().AlignCenter().Row(row => {
                    row.AutoItem().Text(document.Email);
                    if (document.PhoneNumber is not null) {
                        row.AutoItem()
                            .PaddingHorizontal(10)
                            .LineVertical(1);
                        row.AutoItem().Text(document.PhoneNumber);
                    }
                    if (document.GithubUrl is not null) {
                        row.AutoItem()
                            .PaddingHorizontal(10)
                            .LineVertical(1);
                        row.AutoItem().Text(t => {
                            t.Hyperlink("Github", document.GithubUrl).Underline()
                                .FontColor(Colors.Blue.Medium);
                        });
                    }
                    if (document.LinkedInUrl is not null) {
                        row.AutoItem()
                            .PaddingHorizontal(10)
                            .LineVertical(1);
                        row.AutoItem().Text(t => {
                            t.Hyperlink("LinkedIn", document.LinkedInUrl).Underline()
                                .FontColor(Colors.Blue.Medium);
                        });
                    }
                });
            });

            page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(column => {
                foreach (var documentSection in document.Sections) {
                    column.Title(documentSection.Title);
                    foreach (var datedItem in documentSection.Items) {
                        column.DatedItem(datedItem);
                        column.Item().PaddingVertical(5);
                    }
                }
            });
        });
    });
}

public static class Extensions {
    public static void Title(this ColumnDescriptor column, string title) {
        column.Item().Text(title.ToUpper()).FontSize(15).Bold();
        column.Item().PaddingVertical(5).LineHorizontal(1);
    }

    public static void DatedItem(this ColumnDescriptor column, DatedItem item) {
        column.Item().Row(row => {
            row.RelativeItem().Text(item.Title).Bold().FontSize(13);
            if (item.Date is not null) {
                row.AutoItem().Text(item.Date).Bold();
            }
        });
        if (item.SubTitle is not null || item.Location is not null) {
            column.Item().Row(row => {
                if (item.SubTitle is not null) {
                    row.RelativeItem().Text(item.SubTitle);
                }
                if (item.Location is not null) {
                    row.AutoItem().Text(item.Location).Italic();
                }
            });
        }
        column.Item().PaddingVertical(5);

        if (item.Points is not null && item.Points.Length > 0) {
            for (int i = 0; i < item.Points.Length; i++) {
                column.Item().Text(t => {
                    t.Span("- ");
                    if (item.Points[i].Bolded is not null) {
                        t.Span(item.Points[i].Bolded).Bold();
                    }
                    t.Span(item.Points[i].Content);
                });
            }
        }
    }
}