// -----------------------------------------------------------------------
// <copyright file="DatabaseItem.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Model
{
    using Newtonsoft.Json;

    /// <summary>
    /// DatabaseItem.
    /// </summary>
    public class DatabaseItem
    {
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
