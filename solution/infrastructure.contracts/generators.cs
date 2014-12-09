using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.infrastructure.operations.contracts
{
    /// <summary>
    /// Specifies a contract for providing unique keys
    /// </summary>
    /// <typeparam name="Tkey">The type of key</typeparam>
    public interface IKeyGenerator<Tkey>
        where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// Produces the next key
        /// </summary>
        /// <returns>The next available key</returns>
        Tkey GetNextKey();
    }

    public interface IIntegralKeyGenerator : IKeyGenerator<int> { }

    public interface ILongKeyGenerator : IKeyGenerator<long> { }

    public interface IGuidKeyGenerator : IKeyGenerator<string> { }

    public interface IFPIKeyGenerator : IKeyGenerator<string>
    {
        string ISO { get; set; }
        string Owner { get; set; }
        string Description { get; set; }
        string LanguageId { get; set; }
        Authority Authority { get; set; }
    }
}
