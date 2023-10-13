namespace NameAndShame.Models;

public class Branch
{
    public string BranchName { get; set; }
    public string Author { get; set; }
    public DateTime LastCommit { get; set; }
    public string PRStatus { get; set; }
    public int Ahead { get; set; }
    public int Behind { get; set; }
}