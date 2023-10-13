using Newtonsoft.Json.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NameAndShame.Models;
using NameAndShame.Repositories;

namespace NameAndShame.Services;

public class Handler
{
    private Filters _filters;
    private IBitBucketRepo _bitBucket;
    private AppSettings _appSettings;

    public Handler(Filters filters, IBitBucketRepo bitBucket, IOptions<AppSettings> appSettings)
    {
        _filters = filters;
        _bitBucket = bitBucket;
        _appSettings = appSettings.Value;
    }

    public async ValueTask<int> Run()
    {
        var x = new Dictionary<string, List<string>>();
        //convert [] to kvp.

        var listOfProjects = _appSettings.Projects.Split('|');
        foreach (var listOfProject in listOfProjects)
        {
            var split = listOfProject.Split(':');
            var repos = split[1].Split(',').ToList();
            x.Add(split[0], repos);
            //loop projects
            foreach (var (project, repositories) in x)
            {
                foreach (var repo in repositories)
                {
                    //get branches
                    var branches = new List<Branch>();

                    var finalPage = false;
                    var startPosition = 0;
                    while (finalPage != true)
                    {
                        var result = await _bitBucket.GetBranchList(project, repo, startPosition, 10);

                        if (result.Success == false)
                        {
                            throw new Exception("Error Connecting to BitBucket", result.Error);
                        }
                        
                        var json = JObject.Parse(result.Data);

                        if ((bool)json["isLastPage"])
                        {
                            finalPage = true;
                        }
                        else
                        {
                            startPosition = (int)json["nextPageStart"];
                        }

                        branches.AddRange(await Parse(json));
                    }

                    //filter branches based on author
                    var userBranches = await _filters.GroupBranches(branches);
                    var branchStats = new BranchStats()
                    {
                        ZeroAheadCount = (from branch in branches where branch.Ahead == 0 select branch).Count(),
                        EmailList = (from branch in branches select branch.Author).Distinct().ToArray(),
                        BranchUsers = userBranches
                    };
                    Console.WriteLine(JsonSerializer.Serialize(branchStats));
                    return 0;
                }
            }
        }

        return 1;
    }

    private async ValueTask<List<Branch>> Parse(JObject result)
        {
        var branches = new List<Branch>();
        foreach (var branch in (JArray)result["values"])
        {
            if (branch["displayId"].ToString().EndsWith("develop") || branch["displayId"].ToString().EndsWith("master") 
                                                                   || branch["displayId"].ToString().StartsWith("release/"))
            {
                continue;
            }
            branches.Add(await _filters.BranchFilter(branch));
        }
        return branches;
    }
}