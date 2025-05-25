using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Exceptions
{
    /// <summary>
    /// Prilagođena iznimka za domenske validacijske pogreške.
    /// </summary>
    public class DomainValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public DomainValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public DomainValidationException(string message, IDictionary<string, string[]> errors) : base(message)
        {
            Errors = new Dictionary<string, string[]>(errors);
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public DomainValidationException(string message, string propertyName, string errorMessage)
            : base(message)
        {
            Errors = new Dictionary<string, string[]> { { propertyName, new[] { errorMessage } } };
        }
    }
}
