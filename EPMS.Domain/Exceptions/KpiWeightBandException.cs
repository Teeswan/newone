using System;

namespace EPMS.Domain.Exceptions;

public class KpiWeightBandException : Exception
{
    public KpiWeightBandException(string message) : base(message) { }
}
