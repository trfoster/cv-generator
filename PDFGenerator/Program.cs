using System.Text.Json;
using PDFGenerator;
using QuestPDF.Companion;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

QuestPDF.Settings.License = LicenseType.Community;
const int defaultFontSize = 11;

using var fontStream = File.OpenRead("../../../Garamond.ttf");
FontManager.RegisterFont(fontStream);

string json = File.ReadAllText("../../../chef.json");
var cv = JsonSerializer.Deserialize<Root>(json)!;

//CreateCv(cv).ShowInCompanion();
CreateCv(cv).GeneratePdf("/home/foamtoaster/Downloads/chef-cv.pdf");
return;

Document CreateCv(Root document) {
    return Document.Create(container => {
        container.Page(page => {
            page.Size(PageSizes.A4);
            page.MarginHorizontal(1f, Unit.Centimetre);
            page.MarginVertical(0.7f, Unit.Centimetre);
            page.DefaultTextStyle(t => t.FontSize(defaultFontSize).FontFamily("EB Garamond"));

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
                                .FontColor(Colors.Blue.Darken4);
                        });
                    }
                    if (document.LinkedInUrl is not null) {
                        row.AutoItem()
                            .PaddingHorizontal(10)
                            .LineVertical(1);
                        row.AutoItem().Text(t => {
                            t.Hyperlink("LinkedIn", document.LinkedInUrl).Underline()
                                .FontColor(Colors.Blue.Darken4);
                        });
                    }
                });
            });

            page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(column => {
                if (document.Summary is not null) {
                    column.Title("Summary");
                    column.Item().Text(document.Summary);
                    column.Item().PaddingVertical(3);
                }
                foreach (var documentSection in document.Sections) {
                    column.Title(documentSection.Title);
                    foreach (var datedItem in documentSection.Items) {
                        column.DatedItem(datedItem);
                        column.Item().PaddingVertical(3);
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

    private static void LinkSpan(this TextDescriptor textDescriptor, string? text, bool isItalic = false) {
        ArgumentNullException.ThrowIfNull(text);
        int firstIndex = text.IndexOf('[');
        if (firstIndex == -1) {
            if (isItalic) textDescriptor.Span(text).Italic();
            else textDescriptor.Span(text);
            return;
        }
        
        int endIndex = -1;
        for (int index = firstIndex; index > -1; index = text.IndexOf('[', index + 1)) {
            //Checks if hyperlink is at the start, if not, draws text
            if (index != 0) {
                string startText = text.Substring(endIndex + 1, index - endIndex - 1);
                if (isItalic) textDescriptor.Span(startText).Italic();
                else  textDescriptor.Span(startText);
            }

            endIndex = text.IndexOf(']', index + 1);
            int commaIndex = text.IndexOf(',', index + 1, endIndex - index);
            
            string displayText = text[(commaIndex + 1)..endIndex];
            string urlText = text[(index + 1)..commaIndex];
            textDescriptor.Hyperlink(displayText, urlText).Underline().FontColor(Colors.Blue.Darken4);
        }

        if (endIndex != text.Length - 1) {
            string restOfText = text[(endIndex + 1)..];
            if (isItalic) textDescriptor.Span(restOfText).Italic();
            else textDescriptor.Span(restOfText);
        }
    }

    public static void DatedItem(this ColumnDescriptor column, DatedItem item) {
        column.Item().Row(row => {
            if (item.Title is null) return;
            row.RelativeItem().Text(item.Title).Bold().FontSize(13);
            if (item.Date is not null) {
                row.AutoItem().Text(item.Date).Bold();
            }
        });
        if (item.SubTitle is not null || item.Location is not null) {
            column.Item().Row(row => {
                if (item.SubTitle is not null) {
                    row.RelativeItem().Text(t => {
                        t.LinkSpan(item.SubTitle, true);
                    });
                }
                if (item.Location is not null) {
                    row.AutoItem().Text(item.Location).Italic();
                }
            });
        }

        if (item.Points is not null && item.Points.Length > 0) {
            for (int i = 0; i < item.Points.Length; i++) {
                column.Item().Row(row => {
                    row.ConstantItem(11).Text("–");
                    row.RelativeItem().Text(t => {
                        if (item.Points[i].Bolded is not null) {
                            t.Span(item.Points[i].Bolded).Bold();
                        }
                        t.LinkSpan(item.Points[i].Content);
                    });
                });
                    
            }
        }
    }
}