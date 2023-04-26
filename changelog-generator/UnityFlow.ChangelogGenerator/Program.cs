using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Octokit;

class Program {
    static async Task Main(string[] args) {
        var client = new GitHubClient(new ProductHeaderValue("unityflow"));
        var tokenAuth = new Credentials(Environment.GetEnvironmentVariable("UNITYFLOW_SECRET"));
        client.Credentials = tokenAuth;

        var releaseTag = args[0]; // e.g. "v1.0.0"
        var owner = args[1]; // e.g. "my-org"
        var repo = args[2]; // e.g. "my-repo"

        // Get the release and its associated commits
        var release = await client.Repository.Release.Get(owner, repo, releaseTag);
        var commits = await client.Repository.Commit.GetAll(owner, repo, new CommitRequest { Sha = release.TargetCommitish });

        // Get the previous release to compare with
        var previousRelease = await client.Repository.Release.GetAll(owner, repo);
        // var previousReleaseTag = previousRelease.FirstOrDefault()?.TagName;
        var previousReleaseDate = previousRelease.FirstOrDefault()?.CreatedAt.UtcDateTime ?? DateTime.Parse("2000-01-01 00:00:00");

        // Filter the commits to only include those since the last release
        var filteredCommits = commits.Where(commit => commit.Commit.Author.Date >= previousReleaseDate)
                                     .ToList();

        // Generate the changelog
        var changelog = "# Changelog\n\n";
        foreach (var commit in filteredCommits) {
            var commitMessage = commit.Commit.Message.Trim();
            if (commitMessage.StartsWith("feat") || commitMessage.StartsWith("fix") || commitMessage.StartsWith("perf") || commitMessage.StartsWith("docs") || commitMessage.StartsWith("refactor") || commitMessage.StartsWith("test") || commitMessage.StartsWith("build") || commitMessage.StartsWith("ci") || commitMessage.StartsWith("chore")) {
                var match = Regex.Match(commitMessage, @"(\bfeat\b|\bfix\b|\bperf\b|\bdocs\b|\brefactor\b|\btest\b|\bbuild\b|\bci\b|\bchore\b)(\([^\)]+\))?:\s*(.+)");
                var type = match.Groups[1].Value;
                var scope = match.Groups[2].Value.Trim('(', ')');
                var message = match.Groups[3].Value;
                changelog += $"- **{type}**({scope}): {message}\n";
            }
        }

        // Update the Changelog.md file
        var changelogFilePath = "Changelog.md";
        var currentChangelog = File.ReadAllText(changelogFilePath);
        currentChangelog = currentChangelog.Replace("# Changelog", $"# Changelog\n\n## {release.Name} ({release.CreatedAt.ToLocalTime():yyyy-MM-dd})");
        currentChangelog = currentChangelog.Insert(currentChangelog.IndexOf("## [Unreleased]"), changelog);
        File.WriteAllText(changelogFilePath, currentChangelog);

        // Update the release notes
        var update = new ReleaseUpdate {
            Body = currentChangelog
        };
        var updatedRelease = await client.Repository.Release.Edit(owner, repo, release.Id, update);
    }
}
