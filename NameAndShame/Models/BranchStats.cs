namespace NameAndShame.Models;

public class BranchStats
{
    public int ZeroAheadCount { get; set; }
    public string[] EmailList { get; set; }
    public List<BranchUser> BranchUsers { get; set; }
}