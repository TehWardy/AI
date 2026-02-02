namespace TehWardy.AI.Tools.Variable.Processings;

internal interface ISetVariableToolProcessingService
{
    void SetBool(string key, bool value);
    void SetInt(string key, int value);
    void SetString(string key, string value);
}