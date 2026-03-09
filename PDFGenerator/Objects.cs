namespace PDFGenerator;

public record Document(Section[] sections);
public record Section(string Title, DatedItem[] items);
public record DatedItem(string Title, string? Date = null, string? SubTitle = null, string? Location = null, BulletPointItem[]? Points = null);
public record BulletPointItem(string? Title, string Content);