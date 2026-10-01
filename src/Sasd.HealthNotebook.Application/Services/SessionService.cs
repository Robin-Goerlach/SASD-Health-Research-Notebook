using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Conversation preparation/follow-up use cases without medical interpretation.</summary>
public sealed class SessionService
{
    private readonly ISessionRepository _sessions;
    private readonly IHealthTopicRepository _topics;
    /// <summary>Uses UI-independent persistence contracts.</summary>
    public SessionService(ISessionRepository sessions, IHealthTopicRepository topics)
    { _sessions = sessions ?? throw new ArgumentNullException(nameof(sessions)); _topics = topics ?? throw new ArgumentNullException(nameof(topics)); }
    /// <summary>Creates a session with an optional existing topic reference.</summary>
    public async Task<Session> CreateSessionAsync(CreateSessionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = Session.Create(request.ScheduledAt, request.Title, request.SessionType, request.Status,
            request.HealthTopicId, request.ContactText, request.Notes);
        if (session.HealthTopicId.HasValue && !(await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).Any(topic => topic.Id == session.HealthTopicId))
            throw new ArgumentException("The selected topic does not exist.");
        await _sessions.AddSessionAsync(session, cancellationToken).ConfigureAwait(false); return session;
    }
    /// <summary>Lists newest scheduled instants first, optionally filtered by topic.</summary>
    public async Task<IReadOnlyList<SessionSummary>> GetSessionsAsync(Guid? healthTopicId = null, CancellationToken cancellationToken = default)
    {
        var store = await _sessions.LoadAsync(cancellationToken).ConfigureAwait(false);
        var topics = (await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(topic => topic.Id);
        return store.Sessions.Where(session => !healthTopicId.HasValue || session.HealthTopicId == healthTopicId)
            .OrderByDescending(session => session.ScheduledAt).ThenByDescending(session => session.CreatedAt).ThenBy(session => session.Id)
            .Select(session => new SessionSummary(session, session.HealthTopicId.HasValue && topics.TryGetValue(session.HealthTopicId.Value, out var topic) ? topic.Title : null)).ToList();
    }
    /// <summary>Loads one coherent selected-session snapshot, with questions in explicit order.</summary>
    public async Task<SessionNotebook> GetSessionDetailsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var store = await _sessions.LoadAsync(cancellationToken).ConfigureAwait(false);
        return new SessionNotebook(store.Sessions.Where(session => session.Id == sessionId).ToList(),
            store.Questions.Where(question => question.SessionId == sessionId).OrderBy(question => question.SortOrder).ThenBy(question => question.CreatedAt).ThenBy(question => question.Id).ToList(),
            store.FollowUps.Where(followUp => followUp.SessionId == sessionId).OrderBy(followUp => followUp.CreatedAt).ThenBy(followUp => followUp.Id).ToList());
    }
    /// <summary>Adds a user's question only to an existing session.</summary>
    public async Task<SessionQuestion> CreateQuestionAsync(CreateSessionQuestionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var question = SessionQuestion.Create(request.SessionId, request.Text, request.SortOrder, request.IsAnswered, request.AnswerNote);
        await RequireSessionAsync(question.SessionId, cancellationToken).ConfigureAwait(false);
        await _sessions.AddQuestionAsync(question, cancellationToken).ConfigureAwait(false); return question;
    }
    /// <summary>Adds a documented next step only to an existing session.</summary>
    public async Task<SessionFollowUp> CreateFollowUpAsync(CreateSessionFollowUpRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var followUp = SessionFollowUp.Create(request.SessionId, request.Text, request.Status);
        await RequireSessionAsync(followUp.SessionId, cancellationToken).ConfigureAwait(false);
        await _sessions.AddFollowUpAsync(followUp, cancellationToken).ConfigureAwait(false); return followUp;
    }
    /// <summary>Records the user's answer state and note; never interprets their contents.</summary>
    public async Task SetQuestionAnswerAsync(Guid id, bool answered, string? note, CancellationToken cancellationToken = default)
    {
        var question = (await _sessions.LoadAsync(cancellationToken).ConfigureAwait(false)).Questions.SingleOrDefault(item => item.Id == id)
            ?? throw new ArgumentException("The selected question does not exist.");
        question.WithAnswer(answered, note).Validate();
        await _sessions.SetQuestionAnswerAsync(id, answered, note, cancellationToken).ConfigureAwait(false);
    }
    /// <summary>Records only an explicitly selected next-step status.</summary>
    public async Task SetFollowUpStatusAsync(Guid id, SessionFollowUpStatus status, CancellationToken cancellationToken = default)
    {
        var followUp = (await _sessions.LoadAsync(cancellationToken).ConfigureAwait(false)).FollowUps.SingleOrDefault(item => item.Id == id)
            ?? throw new ArgumentException("The selected follow-up does not exist.");
        followUp.WithStatus(status).Validate();
        await _sessions.SetFollowUpStatusAsync(id, status, cancellationToken).ConfigureAwait(false);
    }
    private async Task RequireSessionAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!(await _sessions.LoadAsync(cancellationToken).ConfigureAwait(false)).Sessions.Any(session => session.Id == id))
            throw new ArgumentException("The selected session does not exist.");
    }
}
