using LibGit2Sharp;
using Simple.ProjectAnalyzer.Domain.CommandLine;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class GitService
{
    private const string RepositoryRootDirectoryName = "Simple.ProjectAnalyzer.Repos";
    
    public string Clone(Uri path)
    {
        Output.Verbose($"{nameof(GitService)}.{nameof(Clone)} started");
        Output.Verbose($"Repository root directory name: {RepositoryRootDirectoryName}");
        
        var repositoryName = Path.GetFileNameWithoutExtension(path.AbsolutePath);
        var repositoryRoot = Path.Combine(Path.GetTempPath(), RepositoryRootDirectoryName);
        var targetFolder = Path.Combine(repositoryRoot, repositoryName, Guid.NewGuid().ToString());

        if (!Directory.Exists(repositoryRoot))
        {
            Output.Verbose($"Creating directory {repositoryRoot}");
            Directory.CreateDirectory(repositoryRoot);
        }
        
        Output.Verbose($"Cloning repository {repositoryName} to {targetFolder} from  {path}");
        Repository.Clone(path.ToString(), targetFolder);

        return targetFolder;
    }
}