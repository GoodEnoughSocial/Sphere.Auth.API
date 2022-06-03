namespace Sphere.Auth.API.Pages.Ciba;

public class InputModel
{
    public string Button { get; set; }
    public IEnumerable<string> ScopesConsented { get; set; } = Enumerable.Empty<string>();
    public string Id { get; set; }
    public string Description { get; set; }
}
