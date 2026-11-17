using KlikajZGlowa.Model;

public class AuthorApplication
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string? Motivation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string? RejectionReason { get; set; }
}