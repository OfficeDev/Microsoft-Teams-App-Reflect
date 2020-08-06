// -----------------------------------------------------------------------
// <copyright file="user.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Model
{
    using Newtonsoft.Json;

     /// <summary>
     /// User class
     /// </summary>
    public class User 
        : DatabaseItem
    {
        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [JsonProperty("type")]
        public override string Type { get; set; } = nameof(User);

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets AAD ObjectId.
        /// </summary>
        [JsonProperty("aadObjectId")]
        public string AadObjectId { get; set; }

        /// <summary>
        /// Gets or sets BotConversationId.
        /// </summary>
        public string BotConversationId { get; set; }

        /// <summary>
        /// Gets or sets PersonalConversationId.
        /// </summary>
        public string PersonalConversationId { get; set; }
    }
}
