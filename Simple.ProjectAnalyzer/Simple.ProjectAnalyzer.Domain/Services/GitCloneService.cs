using LibGit2Sharp;
using Simple.ProjectAnalyzer.Domain.CommandLine;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class GitCloneService
{
    private const string GitCloneTargetBaseFolder = @"tmp\";

    public string CloneRepository(Uri path)
    {
        var repositoryName = Path.GetFileNameWithoutExtension(path.AbsolutePath);
        var repositoryRoot = Path.Combine(Path.GetTempPath(), "Simple.ProjectAnalyzer.Repos");
        var targetFolder = Path.Combine(repositoryRoot, repositoryName, Guid.NewGuid().ToString());

        if (!Directory.Exists(repositoryRoot))
        {
            Writer.WriteVerbose($"Creating directory {repositoryRoot}");
            Directory.CreateDirectory(repositoryRoot);
        }
        
        Writer.WriteVerbose($"Cloning repository {repositoryName} to {targetFolder} from  {path}");
        var result = Repository.Clone(path.ToString(), targetFolder);
        Writer.WriteVerbose($"Cloned repository {repositoryName} to {result}");

        return targetFolder;
    }
}