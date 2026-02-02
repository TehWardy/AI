using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.DemoConsole;

internal static partial class Data
{
    public static Runbook[] Runbooks() =>
    [
        DefaultRunbook,
        ArchitectRunbook
    ];
}