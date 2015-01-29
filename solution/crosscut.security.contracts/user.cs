using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosscut.security.contracts
{
    /// <summary>
    ///
    /// </summary>
    public interface IUser
    {
        string Username { get; set; }

        string Password { get; set; }

        string Alias { get; set; }

        string Email { get; set; }
    }
}