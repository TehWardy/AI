using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Processings;
public interface IArchitectureNormalizerProcessingService
{
    ArchitectureSpec Normalize(ArchitectureSpec spec);
}