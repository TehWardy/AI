using TehWardy.AI.Tools.Variable.Processings;

namespace TehWardy.AI.Tools.Variable;

internal class VariableTool(ISetVariableToolProcessingService setVariableToolProcessingService)
{
    public void SetBool(string key, bool value) =>
        setVariableToolProcessingService.SetBool(key, value);

    public void SetString(string key, string value) =>
        setVariableToolProcessingService.SetString(key, value);

    public void SetInt(string key, int value) =>
        setVariableToolProcessingService.SetInt(key, value);
}