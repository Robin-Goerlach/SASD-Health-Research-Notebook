using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>Coherent session snapshot with no redundant parent metadata.</summary>
public sealed record SessionNotebook(IReadOnlyList<Session> Sessions, IReadOnlyList<SessionQuestion> Questions,
    IReadOnlyList<SessionFollowUp> FollowUps);
/// <summary>Current topic title resolved only for display.</summary>
public sealed record SessionSummary(Session Session, string? HealthTopicTitle);
/// <summary>User-entered appointment metadata.</summary>
public sealed record CreateSessionRequest(DateTimeOffset ScheduledAt, string Title, SessionType SessionType = SessionType.DoctorVisit,
    SessionStatus Status = SessionStatus.Planned, Guid? HealthTopicId = null, string? ContactText = null, string? Notes = null);
/// <summary>User-entered question and separately documented answer.</summary>
public sealed record CreateSessionQuestionRequest(Guid SessionId, string Text, int SortOrder = 0, bool IsAnswered = false, string? AnswerNote = null);
/// <summary>User-entered next step, without medical recommendation.</summary>
public sealed record CreateSessionFollowUpRequest(Guid SessionId, string Text, SessionFollowUpStatus Status = SessionFollowUpStatus.Open, DateOnly? DueDate = null);
