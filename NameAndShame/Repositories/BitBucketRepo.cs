using Microsoft.Extensions.Options;
using NameAndShame.Models;
using StackExchange.Utils;

namespace NameAndShame.Repositories;

public class BitBucketRepo : IBitBucketRepo
{
    private AppSettings _appSettings;

    public BitBucketRepo(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public async ValueTask<HttpCallResponse<string>> GetBranchList(string project, string repository, int start, int fetch) =>
        await Http.Request(
                $"https://{_appSettings.Uri}/rest/api/latest/projects/{project}/repos/{repository}/branches?details=true&start={start}&limit={fetch}&orderBy=MODIFICATION")
            .AddHeader("Authorization", $"Basic {_appSettings.Auth}")
            .ExpectString()
            .GetAsync();
}