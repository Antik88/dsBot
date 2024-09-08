namespace dsBot.DataContext.Entities;

public class Tracks
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PlayListId { get; set; }
    public PlayList PlayList { get; set; }
}
