using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Atomic session/child storage; updates preserve unrelated records.</summary>
public interface ISessionRepository
{
    /// <summary>Loads a coherent snapshot.</summary>
    Task<SessionNotebook> LoadAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds a session.</summary>
    Task AddSessionAsync(Session session, CancellationToken cancellationToken = default);
    /// <summary>Adds a question to an existing session.</summary>
    Task AddQuestionAsync(SessionQuestion question, CancellationToken cancellationToken = default);
    /// <summary>Adds a next step to an existing session.</summary>
    Task AddFollowUpAsync(SessionFollowUp followUp, CancellationToken cancellationToken = default);
    /// <summary>Updates an existing question's answer only, under the writer lock.</summary>
    Task SetQuestionAnswerAsync(Guid id, bool answered, string? note, CancellationToken cancellationToken = default);
    /// <summary>Updates an existing next step's status only, under the writer lock.</summary>
    Task SetFollowUpStatusAsync(Guid id, SessionFollowUpStatus status, CancellationToken cancellationToken = default);
}
