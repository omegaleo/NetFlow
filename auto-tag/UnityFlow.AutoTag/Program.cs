using Newtonsoft.Json;
using Octokit;
using UnityFlow.AutoTag.Models;

class Program {
    static async Task Main(string[] args) {
        var client = new GitHubClient(new ProductHeaderValue("unityflow"));
        var tokenAuth = new Credentials(Environment.GetEnvironmentVariable("UNITYFLOW_SECRET"));
        client.Credentials = tokenAuth;

        var owner = args[0];
        var repo = args[1];
        var issueNumber = int.Parse(args[2]);
        var issue = await client.Issue.Get(owner, repo, issueNumber);
        var issueUpdate = new IssueUpdate();

        var filterPath = Path.Join(AppContext.BaseDirectory, "filters.json");
        var filters = JsonConvert.DeserializeObject<List<Filter>>(File.ReadAllText(filterPath));

        foreach (var filter in filters)
        {
            if (issue.Title.Contains(filter.LookingFor, StringComparison.OrdinalIgnoreCase) || issue.Body.Contains(filter.LookingFor, StringComparison.OrdinalIgnoreCase)) {
                issueUpdate.AddLabel(filter.Label);
            }
        }

        var updatedIssue = await client.Issue.Update(owner, repo, issueNumber, issueUpdate);
        Console.WriteLine($"Successfully tagged Issue #{updatedIssue.Number}");
    }
}