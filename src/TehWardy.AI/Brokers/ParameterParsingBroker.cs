namespace TehWardy.AI.Brokers;

internal class ParameterParsingBroker : IParameterParsingBroker
{
    public string GetString(IDictionary<string, object> dictionary, string key)
    {
        if (dictionary is null || !dictionary.TryGetValue(key, out var value) || value is null)
            return null;

        return value switch
        {
            string s => s,
            _ => value.ToString()
        };
    }

    public bool GetBool(IDictionary<string, object> dictionary, string key, bool defaultValue)
    {
        if (dictionary is null || !dictionary.TryGetValue(key, out var value) || value is null)
            return defaultValue;

        if (value is bool b) return b;

        if (value is string s && bool.TryParse(s, out var parsed))
            return parsed;

        return defaultValue;
    }

    public int GetInt(IDictionary<string, object> dictionary, string key, int defaultValue)
    {
        if (dictionary is null || !dictionary.TryGetValue(key, out var value) || value is null)
            return defaultValue;

        if (value is int i) return i;

        if (value is long l && l >= int.MinValue && l <= int.MaxValue)
            return (int)l;

        if (value is string s && int.TryParse(s, out var parsed))
            return parsed;

        return defaultValue;
    }
}