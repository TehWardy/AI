using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Processings;
public interface IArchitectureValidatorProcessingService
{
    ValidationResult Validate(ArchitectureSpec spec);
}