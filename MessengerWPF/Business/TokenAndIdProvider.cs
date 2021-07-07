using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    /// <summary>
    /// holds token.
    /// </summary>
    public class TokenAndIdProvider
    {
        /// <summary>
        /// Gets or sets the Id of the current user.
        /// </summary>
        /// <value>
        /// The Id.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }
    }
}
