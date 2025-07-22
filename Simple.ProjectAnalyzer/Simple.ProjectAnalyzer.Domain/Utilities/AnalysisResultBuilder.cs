using Simple.ProjectAnalyzer.Domain.Models;

namespace Simple.ProjectAnalyzer.Domain.Utilities;

public class AnalysisResultBuilder
{
    private string? _source;
    private IAnalyzable? _parent;
    private AnalysisResultType _type;
    private AnalysisResultCode _code;
    private string? _title;
    private string? _message;
    private string? _details;
    private string? _recommendation;

    public AnalysisResultBuilder Source(string? source)
    {
        _source = source;
        return this;
    }

    public AnalysisResultBuilder Parent(IAnalyzable parent)
    {
        _parent = parent;
        return this;
    }

    public AnalysisResultBuilder Type(AnalysisResultType type)
    {
        _type = type;
        return this;
    }

    public AnalysisResultBuilder Code(AnalysisResultCode code)
    {
        _code = code;
        return this;
    }

    public AnalysisResultBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public AnalysisResultBuilder Message(string message)
    {
        _message = message;
        return this;
    }

    public AnalysisResultBuilder Details(string details)
    {
        _details = details;
        return this;
    }

    public AnalysisResultBuilder Recommendation(string reccomendation)
    {
        _recommendation = reccomendation;
        return this;
    }

    public AnalysisResult Build() => new()
    {
        Source = _source ?? throw new ArgumentNullException(_source),
        Type = _type,
        Code = _code,
        Title = _title ?? throw new ArgumentNullException(_title),
        Message = _message ?? throw new ArgumentNullException(_message),
        Details = _details ?? throw new ArgumentNullException(_details),
        Parent = _parent ?? throw new ArgumentNullException(nameof(_parent)),
        Recommendation = _recommendation
    };
}