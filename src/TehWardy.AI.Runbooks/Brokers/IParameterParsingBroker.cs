namespace TehWardy.AI.Agents.Runbooks.Brokers;

internal interface IParameterParsingBroker
{
    bool GetBool(IDictionary<string, object> dictionary, string key, bool defaultValue);
    int GetInt(IDictionary<string, object> dictionary, string key, int defaultValue);
    string GetString(IDictionary<string, object> dictionary, string key, string defaultValue = null);
}