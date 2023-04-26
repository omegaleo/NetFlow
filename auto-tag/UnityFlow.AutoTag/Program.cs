using Octokit;

class Program {
    static async Task Main(string[] args) {
        var client = new GitHubClient(new ProductHeaderValue("unityflow"));
        var tokenAuth = new Credentials(Environment.GetEnvironmentVariable("UNITYFLOW_SECRET"));
        client.Credentials = tokenAuth;

        var issueNumber = int.Parse(args[0]);
        var issue = await client.Issue.Get("OWNER", "REPO", issueNumber);
        var issueUpdate = new IssueUpdate();

        if (issue.Title.Contains("@issue") || issue.Body.Contains("@issue")) {
            issueUpdate.AddLabel("issue");
        }

        if (issue.Title.Contains("@feature") || issue.Body.Contains("@feature")) {
            issueUpdate.AddLabel("feature");
        }

        if (issue.Title.Contains("@enhancement") || issue.Body.Contains("@enhancement")) {
            issueUpdate.AddLabel("enhancement");
        }

        var updatedIssue = await client.Issue.Update("OWNER", "REPO", issueNumber, issueUpdate);
        Console.WriteLine(updatedIssue.Number);
    }
}