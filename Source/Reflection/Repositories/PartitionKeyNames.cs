﻿// -----------------------------------------------------------------------
// <copyright file="PartitionKeyNames.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories
{
    /// <summary>
    /// Partition key names used in the table storage.
    /// </summary>
    public class PartitionKeyNames
    {
        /// <summary>
        /// Users data table partition key names.
        /// </summary>
        public static class UserDataTable
        {
            /// <summary>
            /// Table name for user data table.
            /// </summary>
            public static readonly string TableName = "UserData";

            /// <summary>
            /// Users data partition key name.
            /// </summary>
            public static readonly string UserDataPartition = "UserData";
        }

        /// <summary>
        /// Reflection Data Table.
        /// </summary>
        public static class ReflectionDataTable
        {
            /// <summary>
            /// Table name for Reflection data table.
            /// </summary>
            public static readonly string TableName = "ReflectionData";

            /// <summary>
            /// Reflection data partition key name.
            /// </summary>
            public static readonly string ReflectionDataPartition = "ReflectionData";
        }

        /// <summary>
        /// Questions Data Table.
        /// </summary>
        public static class QuestionsDataTable
        {
            /// <summary>
            /// Table name for Questions data table.
            /// </summary>
            public static readonly string TableName = "QuestionsData";

            /// <summary>
            /// Questions data partition key name.
            /// </summary>
            public static readonly string QuestionsDataPartition = "QuestionsData";
        }

        /// <summary>
        /// Recursion Data Table.
        /// </summary>
        public static class RecursionDataTable
        {
            /// <summary>
            /// Table name for Recursion data table.
            /// </summary>
            public static readonly string TableName = "RecursionData";

            /// <summary>
            /// Recursion data partition key name.
            /// </summary>
            public static readonly string RecursionDataPartition = "RecursionData";
        }

        /// <summary>
        /// Feedback Data Table.
        /// </summary>
        public static class FeedbackDataTable
        {
            /// <summary>
            /// Table name for Recursion data table.
            /// </summary>
            public static readonly string TableName = "FeedbackData";

            /// <summary>
            /// Recursion data partition key name.
            /// </summary>
            public static readonly string FeedbackDataPartition = "FeedbackData";
        }
    }
}