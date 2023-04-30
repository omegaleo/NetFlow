
using Octokit;
using UnityFlow.DocumentationHelper.Library.Helpers;

class Program
{
    static async Task Main(string[] args)
    {
        var docs = DocumentationHelperTool.GenerateDocumentation(true);
        
        var client = new GitHubClient(new ProductHeaderValue("unityflow"));
        var tokenAuth = new Credentials(Environment.GetEnvironmentVariable("UNITYFLOW_SECRET"));
        client.Credentials = tokenAuth;

        var filePath = "documentation.md";
        
        if (!File.Exists(filePath))
        {
            await File.WriteAllTextAsync(filePath, "");
        }

        var documentation = "";

        foreach (var doc in docs)
        {
            documentation += $"# {doc.AssemblyName}{Environment.NewLine}  ";
            documentation += $"## {doc.ClassName}{Environment.NewLine}  ";

            foreach (var desc in doc.Descriptions)
            {
                documentation += $"### {desc.Title}{Environment.NewLine}  ";
                documentation += $"{desc.Description}{Environment.NewLine}  ";
            }
        }
        
        File.WriteAllText(filePath, documentation);
    }
}
