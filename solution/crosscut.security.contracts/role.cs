using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosscut.security.contracts
{
    /// <summary>
    /// Specifies an interface for  a security role
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// Gets or sets the name of the role
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a brief decsription of the role
        /// </summary>
        string Description { get; set; }
    }
}