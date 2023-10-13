using StackExchange.Utils;

namespace NameAndShame.Repositories;

public interface IBitBucketRepo
{
    ValueTask<HttpCallResponse<string>> GetBranchList(string project, 
        string repository, int start, int fetch);
}