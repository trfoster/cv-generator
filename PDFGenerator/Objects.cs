namespace PDFGenerator;

public record Root(string Title, string Email, Section[] Sections, string? PhoneNumber = null, string? GithubUrl = null, string? LinkedInUrl = null);
public record Section(string Title, DatedItem[] Items);
public record DatedItem(string? Title, string? Date = null, string? SubTitle = null, string? Location = null, BoldedBulletPoint[]? Points = null);
public record BoldedBulletPoint(string? Bolded, string Content);