using LibGit2Sharp;
using Simple.ProjectAnalyzer.Abstractions.CommandLine;

namespace Simple.ProjectAnalyzer.Domain.Services;

public class GitService(IConsoleOutput console)
{
    private const string RepositoryRootDirectoryName = "Simple.ProjectAnalyzer.Repos";
    
    public string Clone(Uri path)
    {
        console.Verbose($"{nameof(GitService)}.{nameof(Clone)} started");
        console.Verbose($"Repository root directory name: {RepositoryRootDirectoryName}");
        
        var repositoryName = Path.GetFileNameWithoutExtension(path.AbsolutePath);
        var repositoryRoot = Path.Combine(Path.GetTempPath(), RepositoryRootDirectoryName);
        var targetFolder = Path.Combine(repositoryRoot, repositoryName, Guid.NewGuid().ToString());

        if (!Directory.Exists(repositoryRoot))
        {
            console.Verbose($"Creating directory {repositoryRoot}");
            Directory.CreateDirectory(repositoryRoot);
        }
        
        console.Verbose($"Cloning repository {repositoryName} to {targetFolder} from  {path}");
        Repository.Clone(path.ToString(), targetFolder);

        return targetFolder;
    }
}