const { context, getOctokit } = require('@actions/github');
const octokit = getOctokit(process.env.UNITYFLOW_SECRET);
const issueNumber = context.payload.issue.number;
const issueTitle = context.payload.issue.title;
const issueBody = context.payload.issue.body;
const isFeature = /@feature/gi.test(issueBody) || /@feature/gi.test(issueTitle);
const isBug = /@bug/gi.test(issueBody) || /@bug/gi.test(issueTitle);
const isEnhancement = /@enhancement/gi.test(issueBody) || /@enhancement/gi.test(issueTitle);

console.log(JSON.stringify(octokit.issues));

if (isFeature) {
  octokit.issues.addLabels({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    labels: ['feature']
  });
}
if (isBug) {
  octokit.issues.addLabels({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    labels: ['bug']
  });
}
if (isEnhancement) {
  octokit.issues.addLabels({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    labels: ['enhancement']
  });
}
