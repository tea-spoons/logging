
namespace TeaSpoons.Logging
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for additional log data, beyond the central message or exception.
    /// </summary>
    /// <remarks>
    /// Implement this with a <c>readonly struct</c> to optimize performance.
    /// </remarks>
    public interface ILogData : IEnumerable<(string Key, string Value)>
    {

    }
}
