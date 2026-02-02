using TehWardy.AI.Agents.Runbooks.Models;

namespace TehWardy.AI.Tools.Variable.Processings;

internal class SetVariableToolProcessingService(RunbookState runbookState) 
    : ISetVariableToolProcessingService
{
    public void SetBool(string key, bool value) =>
        runbookState.Variables[key] = value;

    public void SetString(string key, string value) =>
        runbookState.Variables[key] = value;

    public void SetInt(string key, int value) =>
        runbookState.Variables[key] = value;
}