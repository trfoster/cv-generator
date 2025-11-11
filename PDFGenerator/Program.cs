using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

QuestPDF.Settings.License = LicenseType.Community;

Document.Create(container => {
    container.Page(page => {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.DefaultTextStyle(t => t.FontSize(12).FontFamily("Courier"));
        
        page.Header().Text("Tom Foster").Bold().FontSize(30).FontFamily("Courier").AlignCenter();

        page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(c => {
            c.Item().Text("This is a really amazing CV");
            c.Item().Image(Placeholders.Image(100, 100));
        });

        page.Footer().AlignCenter().Text("Really nice footer");
    });
}).ShowInCompanion();