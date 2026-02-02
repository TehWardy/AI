using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Processings;
public interface IManifestGenerationProcessingService
{
    ConformanceManifest Generate(ArchitectureSpec spec);
}