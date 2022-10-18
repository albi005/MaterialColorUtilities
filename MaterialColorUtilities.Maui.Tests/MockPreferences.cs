using System.Text.Json;

namespace MaterialColorUtilities.Maui.Tests;

public class MockPreferences : IPreferences
{
    private readonly Dictionary<string, string> _container = new(); 

    public bool ContainsKey(string key, string? sharedName = null)
    {
        return _container.ContainsKey(key);
    }

    public void Remove(string key, string? sharedName = null)
    {
        _container.Remove(key);
    }

    public void Clear(string? sharedName = null)
    {
        _container.Clear();
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        _container[key] = JsonSerializer.Serialize(value);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        return JsonSerializer.Deserialize<T>(_container[key]) ?? throw new InvalidOperationException();
    }
}