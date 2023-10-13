using NameAndShame.Models;
using Newtonsoft.Json.Linq;

namespace NameAndShame;

public class Filters
{
    public async ValueTask<Branch> BranchFilter(JToken json)
    {
        return new Branch()
        {
            Author = (string)json["metadata"]
                ["com.atlassian.bitbucket.server.bitbucket-branch:latest-commit-metadata"]["author"]["emailAddress"],
            BranchName = (string)json["displayId"],
            LastCommit = DateTimeOffset.FromUnixTimeMilliseconds((long)json["metadata"]
                ["com.atlassian.bitbucket.server.bitbucket-branch:latest-commit-metadata"]["committerTimestamp"]).DateTime,
            PRStatus = (string)json["metadata"]?
                ["com.atlassian.bitbucket.server.bitbucket-ref-metadata:outgoing-pull-request-metadata"]?
                ["pullRequest"]?["state"] ?? "NO-PR",
            Ahead = (int)json["metadata"][
                "com.atlassian.bitbucket.server.bitbucket-branch:ahead-behind-metadata-provider"][
                "ahead"],
            Behind = (int)json["metadata"][
            "com.atlassian.bitbucket.server.bitbucket-branch:ahead-behind-metadata-provider"][
            "behind"]
        };
    }

    public async ValueTask<List<BranchUser>> GroupBranches(List<Branch> branches)
    {
        var users = (from branch in branches select branch.Author).Distinct();

        return users.Select(user => new BranchUser()
        {
            Author = user,
            BranchCount = (from branch in branches where branch.Author == user select branch).Count(),
            Branches = from branch in branches where branch.Author == user select branch
        }).ToList();
    }
}