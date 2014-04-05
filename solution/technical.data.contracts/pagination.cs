using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.technical.data.contracts
{
    /// <summary>
    /// Specifies the interface for a page size
    /// </summary>
    /// <typeparam name="TSize">Represents the data type of the size</typeparam>
    public interface IPaginated<TSize> where TSize : struct
    {
        /// <summary>
        /// Gets or sets page capacity (number of results taken per page)
        /// </summary>
        TSize? Take { get; set; }

    }
}
