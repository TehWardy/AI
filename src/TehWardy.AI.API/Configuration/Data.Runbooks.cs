using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.API.Configuration;

internal static partial class Data
{
    public static Runbook[] Runbooks() =>
    [
        DefaultRunbook,
        ArchitectRunbook
    ];
}