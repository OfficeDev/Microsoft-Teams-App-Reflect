// -----------------------------------------------------------------------
// <copyright file="UserDataEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.UserData
{
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// UserData Entity.
    /// </summary>
    public class UserDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets UserID.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets UPN.
        /// </summary>
        public string UPN { get; set; }

        /// <summary>
        /// Gets or sets Role.
        /// </summary>
        public string Role { get; set; }
    }
}
