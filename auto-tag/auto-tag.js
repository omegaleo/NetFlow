const { context} = require('@actions/github');

const { Octokit } = require('@octokit/core');
const { addLabels } = require('@octokit/rest');

const octokit = new Octokit({
  auth: process.env.UNITYFLOW_SECRET
});
const issueNumber = context.payload.issue.number;
const issueTitle = context.payload.issue.title;
const issueBody = context.payload.issue.body;
const isFeature = /@feature/gi.test(issueBody) || /@feature/gi.test(issueTitle);
const isBug = /@bug/gi.test(issueBody) || /@bug/gi.test(issueTitle);
const isEnhancement = /@enhancement/gi.test(issueBody) || /@enhancement/gi.test(issueTitle);

if (isFeature) {
  addLabels({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    labels: ['feature']
  });
}
if (isBug) {
  addLabels({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    labels: ['bug']
  });
}
if (isEnhancement) {
  addLabels({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    labels: ['enhancement']
  });
}
