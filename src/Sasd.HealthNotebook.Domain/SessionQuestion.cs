namespace Sasd.HealthNotebook.Domain;

/// <summary>A user's question and separately recorded answer; no truth judgement.</summary>
public sealed record SessionQuestion
{
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>Owning session.</summary>
    public required Guid SessionId { get; init; }
    /// <summary>Required user question, at most 4000 characters.</summary>
    public required string Text { get; init; }
    /// <summary>Explicit display order; no automatic priority.</summary>
    public required int SortOrder { get; init; }
    /// <summary>User marks the question answered.</summary>
    public required bool IsAnswered { get; init; }
    /// <summary>Optional documented answer, at most 4000 characters.</summary>
    public string? AnswerNote { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical last update time.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Creates a question belonging to exactly one session.</summary>
    public static SessionQuestion Create(Guid sessionId, string text, int sortOrder = 0, bool isAnswered = false, string? answerNote = null)
    {
        var now = DateTimeOffset.Now;
        var question = new SessionQuestion { Id = Guid.NewGuid(), SessionId = sessionId, Text = text?.Trim() ?? string.Empty,
            SortOrder = sortOrder, IsAnswered = isAnswered, AnswerNote = answerNote, CreatedAt = now, ModifiedAt = now };
        question.Validate(); return question;
    }
    /// <summary>Updates only the user-recorded answer state and note.</summary>
    public SessionQuestion WithAnswer(bool answered, string? note)
    {
        var now = DateTimeOffset.Now;
        var updated = this with { IsAnswered = answered, AnswerNote = note, ModifiedAt = now > ModifiedAt ? now : ModifiedAt.AddTicks(1) };
        updated.Validate(); return updated;
    }
    /// <summary>Validates structure; owning session existence is checked by Application/storage.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || SessionId == Guid.Empty || CreatedAt == default || ModifiedAt < CreatedAt || SortOrder < 0
            || string.IsNullOrWhiteSpace(Text) || Text.Length > Session.MaximumTextLength || AnswerNote?.Length > Session.MaximumTextLength)
            throw new ArgumentException("Invalid session question metadata or text length.");
    }
}
