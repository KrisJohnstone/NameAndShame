namespace NameAndShame.Models;

public class BranchUser
{
    public string Author { get; set; }
    public int BranchCount { get; set; }
    public IEnumerable<Branch> Branches { get; set; }
}