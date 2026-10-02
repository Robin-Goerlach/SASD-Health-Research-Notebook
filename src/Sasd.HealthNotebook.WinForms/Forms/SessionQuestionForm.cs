using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Adds a question or records an existing question's answer, without generating suggestions.</summary>
public sealed class SessionQuestionForm : SourceRecordForm
{
    private readonly SessionService _service;
    private readonly Guid _sessionId;
    private readonly SessionQuestion? _question;
    private readonly TextBox _questionTextBox = TextEditor(Session.MaximumTextLength);
    private readonly NumericUpDown _orderEditor = new() { Minimum = 0, Maximum = int.MaxValue };
    private readonly CheckBox _answeredCheckBox = new() { AutoSize = true };
    private readonly TextBox _answerTextBox = TextEditor(Session.MaximumTextLength);
    /// <summary>Existing questions keep identity, text and order while their answer is updated.</summary>
    public SessionQuestionForm(SessionService service, Guid sessionId, SessionQuestion? question = null)
        : base(question is null ? AppStrings.NewSessionQuestion : AppStrings.EditSessionAnswer, AppStrings.SessionSemantics, new Size(800, 620))
    {
        _service = service; _sessionId = sessionId; _question = question;
        _questionTextBox.Text = question?.Text ?? string.Empty; _questionTextBox.ReadOnly = question is not null;
        _orderEditor.Value = question?.SortOrder ?? 0; _orderEditor.Enabled = question is null;
        _answeredCheckBox.Checked = question?.IsAnswered ?? false; _answerTextBox.Text = question?.AnswerNote ?? string.Empty;
        AddField(AppStrings.SessionQuestionText, _questionTextBox, AppStrings.SessionSemantics, true);
        AddField(AppStrings.SessionOrder, _orderEditor, AppStrings.SessionOrder);
        AddField(AppStrings.SessionAnswered, _answeredCheckBox, AppStrings.SessionAnswered);
        AddField(AppStrings.SessionAnswerNote, _answerTextBox, AppStrings.SessionSemantics, true);
    }
    protected override string ValidationMessage => AppStrings.SessionValidation;
    protected override string SaveOperation => AppStrings.SaveSessionOperation;
    protected override async Task SaveRecordAsync()
    {
        if (_question is null) await _service.CreateQuestionAsync(new(_sessionId, _questionTextBox.Text, (int)_orderEditor.Value, _answeredCheckBox.Checked, _answerTextBox.Text));
        else await _service.SetQuestionAnswerAsync(_question.Id, _answeredCheckBox.Checked, _answerTextBox.Text);
    }
}
