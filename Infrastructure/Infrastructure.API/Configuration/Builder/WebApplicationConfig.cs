using Microsoft.AspNetCore.Builder;

namespace Infrastructure.API.Configuration.Builder;

/// <summary>
/// Помогает билдить кофингурацию приложения. Предотвращает неконсистентный конфиг
/// </summary>
internal class WebApplicationConfig
{
    private readonly List<Action<WebApplication>> configs;
    private readonly Dictionary<string, int> configIndexes;

    public WebApplicationConfig()
    {
        configs = new();
        configIndexes = new();
    }

    public void Add(Action<WebApplication> toAdd)
    {
        var methodName = toAdd.Method.Name;
        // To prevent inconsistent configs. Ordering is important
        if (configIndexes.TryGetValue(methodName, out var index))
        {
            configs[index] = toAdd;
        }
        else
        {
            configIndexes.Add(methodName, configs.Count);
            configs.Add(toAdd);
        }
    }

    public void Apply(WebApplication app)
    {
        foreach (var config in configs)
        {
            config(app);
        }
    }
}