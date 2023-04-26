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
        var previousReleaseDate = previousRelease.FirstOrDefault(x => x.Id != release.Id)?.CreatedAt.UtcDateTime ?? DateTime.Parse("2000-01-01 00:00:00");

        // Filter the commits to only include those since the last release
        var filteredCommits = commits.Where(commit => commit.Commit.Author.Date >= previousReleaseDate)
                                     .ToList();

        // Generate the changelog
        var changelog = $"## ChangeLog{Environment.NewLine}";
        foreach (var commit in filteredCommits) {
            var commitMessage = commit.Commit.Message.Trim();
            if (Regex.IsMatch(commitMessage, "^(Fix|Implemented|Added|Removed|Changed|Modified)"))
            {
                changelog += commitMessage + Environment.NewLine;
            }
        }

        // Update the Changelog.md file
        var changelogFilePath = "Changelog.md";

        if (!File.Exists(changelogFilePath))
        {
            await File.WriteAllTextAsync(changelogFilePath, "");
        }
        
        // Update the release notes
        var update = new ReleaseUpdate {
            Body = changelog
        };
        var updatedRelease = await client.Repository.Release.Edit(owner, repo, release.Id, update);


        changelog = changelog.Replace("## ChangeLog", "## " + releaseTag);
        
        var currentChangelog = File.ReadAllText(changelogFilePath);

        currentChangelog = $"{changelog}{Environment.NewLine}{currentChangelog}";
        File.WriteAllText(changelogFilePath, currentChangelog);

    }
}
