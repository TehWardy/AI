namespace TehWardy.AI.Brokers;

internal interface IParameterParsingBroker
{
    bool GetBool(IDictionary<string, object> dictionary, string key, bool defaultValue);
    int GetInt(IDictionary<string, object> dictionary, string key, int defaultValue);
    string GetString(IDictionary<string, object> dictionary, string key);
}