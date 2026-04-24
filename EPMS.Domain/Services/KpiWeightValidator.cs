using System.Collections.Generic;
using System.Linq;

namespace EPMS.Domain.Services;

public class KpiWeightValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    private KpiWeightValidationResult(bool isValid, string errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static KpiWeightValidationResult Success() => new(true, string.Empty);
    public static KpiWeightValidationResult Failure(string message) => new(false, message);
}

public class KpiWeightValidator
{
    public KpiWeightValidationResult ValidateTotalWeight(IEnumerable<decimal> weights)
    {
        var total = weights.Sum();
        if (total != 100.00m)
        {
            return KpiWeightValidationResult.Failure($"Total KPI weight must equal exactly 100%. Current total: {total}%.");
        }
        return KpiWeightValidationResult.Success();
    }
}
