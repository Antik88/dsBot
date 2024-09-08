namespace dsBot.DataContext.Entities;

public class PlayList
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Tracks> Tracks { get; set; } = new List<Tracks>();
}
