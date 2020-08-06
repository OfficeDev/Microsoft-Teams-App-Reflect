// -----------------------------------------------------------------------
// <copyright file="ProactiveMessageHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Helper
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Reflection.Model;

    /// <summary>
    /// ProactiveMessage Helper.
    /// </summary>
    public class ProactiveMessageHelper
    {
        private IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProactiveMessageHelper"/> class.
        /// ProactiveMessage Helper.
        /// </summary>
        public ProactiveMessageHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Send Personal Notification.
        /// </summary>
        /// <param name="serviceUrl">serviceUrl.</param>
        /// <param name="tenantId">tenantId.</param>
        /// <param name="userDetails">userDetails.</param>
        /// <param name="messageText">messageText.</param>
        /// <param name="attachment">attachment.</param>
        /// <returns>Null.</returns>
        public static async Task<NotificationSendStatus> SendPersonalNotification(string serviceUrl, string tenantId, User userDetails, string messageText, Attachment attachment)
        {
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.MaxValue);
            var connectorClient = new ConnectorClient(new Uri(serviceUrl));
            if (string.IsNullOrEmpty(userDetails.PersonalConversationId))
            {
                var createConversationResult = await GetConversationId(connectorClient, tenantId, userDetails.BotConversationId);
                if (createConversationResult.IsSuccessful)
                {
                    userDetails.PersonalConversationId = createConversationResult.MessageId;
                }
                else
                {
                    return createConversationResult;
                }
            }

            return await SendNotificationToConversationId(connectorClient, tenantId, userDetails.PersonalConversationId, messageText, attachment);
        }

        /// <summary>
        /// Send Channel Notification.
        /// </summary>
        /// <param name="botAccount">connectorClient.</param>
        /// <param name="serviceUrl">tenantId.</param>
        /// <param name="channelId">userId.</param>
        /// <param name="messageText">messageText.</param>
        /// <param name="attachment">attachment.</param>
        /// <returns>.</returns>
        public async Task<NotificationSendStatus> SendChannelNotification(ChannelAccount botAccount, string serviceUrl, string channelId, string messageText, Attachment attachment, string ReflectMessageId="")
        {
            try
            {
                var replyMessage = Activity.CreateMessageActivity();
                replyMessage.Text = messageText;

                if (attachment != null)
                {
                    replyMessage.Attachments.Add(attachment);
                }
                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.MaxValue);
                using (var connectorClient = new ConnectorClient(new Uri(serviceUrl), _configuration["MicrosoftAppId"], _configuration["MicrosoftAppPassword"]))
                {
                    var parameters = new ConversationParameters
                    {
                        Bot = botAccount,
                        ChannelData = new TeamsChannelData
                        {
                            Channel = new ChannelInfo(channelId),
                            Notification = new NotificationInfo() { Alert = true }
                        },
                        IsGroup = true,
                        Activity = (Activity)replyMessage
                    };

                    var exponentialBackoffRetryStrategy = new ExponentialBackoff(
                        3,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(1));

                    // Define the Retry Policy
                    var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                    if (ReflectMessageId == "")
                    {
                        var conversationResource = await
                                                connectorClient.Conversations.CreateConversationAsync(parameters);
                        return new NotificationSendStatus() { MessageId = conversationResource.Id, IsSuccessful = true };
                    }
                    else
                    {
                        var conversationId = $"{channelId};messageid={ReflectMessageId}";
                        var replyActivity = MessageFactory.Attachment(attachment);
                        replyActivity.Conversation = new ConversationAccount(id: conversationId);
                        var resultfeedback = await connectorClient.Conversations.SendToConversationAsync((Activity)replyActivity);
                        return new NotificationSendStatus() { MessageId = resultfeedback.Id, IsSuccessful = true };
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }
        }

        /// <summary>
        /// Send Notification To Conversation Id.
        /// </summary>
        /// <param name="connectorClient">connectorClient.</param>
        /// <param name="tenantId">tenantId.</param>
        /// <param name="conversationId">conversationId.</param>
        /// <param name="messageText">messageText.</param>
        /// <param name="attachment">attachment.</param>
        /// <returns>.</returns>
        private static async Task<NotificationSendStatus> SendNotificationToConversationId(ConnectorClient connectorClient, string tenantId, string conversationId, string messageText, Attachment attachment)
        {
            try
            {
                var replyMessage = Activity.CreateMessageActivity();

                replyMessage.Conversation = new ConversationAccount(id: conversationId);
                replyMessage.ChannelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
                replyMessage.Text = messageText;
                if (attachment != null)
                {
                    replyMessage.Attachments.Add(attachment);
                }

                var exponentialBackoffRetryStrategy = new ExponentialBackoff(
                    5,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(1));

                // Define the Retry Policy
                var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                var resourceResponse = await retryPolicy.ExecuteAsync(() =>
                                        connectorClient.Conversations.SendToConversationAsync(conversationId, (Activity)replyMessage))
                                        .ConfigureAwait(false);
                return new NotificationSendStatus() { MessageId = resourceResponse.Id, IsSuccessful = true };
            }
            catch (Exception ex)
            {
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }
        }

        /// <summary>
        /// Get Conversation Id.
        /// </summary>
        /// <param name="connectorClient">connectorClient.</param>
        /// <param name="tenantId">tenantId.</param>
        /// <param name="userId">userId.</param>
        /// <returns>.</returns>
        private static async Task<NotificationSendStatus> GetConversationId(ConnectorClient connectorClient, string tenantId, string userId)
        {
            var parameters = new ConversationParameters
            {
                Members = new ChannelAccount[] { new ChannelAccount(userId) },
                ChannelData = new TeamsChannelData
                {
                    Tenant = new TenantInfo(tenantId),
                    Notification = new NotificationInfo() { Alert = true },
                },
                IsGroup = false,
            };

            try
            {
                var exponentialBackoffRetryStrategy = new ExponentialBackoff(
                    5,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(1));

                // Define the Retry Policy
                var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                var conversationResource = await retryPolicy.ExecuteAsync(() =>
                                            connectorClient.Conversations.CreateConversationAsync(parameters))
                                            .ConfigureAwait(false);
                return new NotificationSendStatus() { MessageId = conversationResource.Id, IsSuccessful = true };
            }
            catch (Exception ex)
            {
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }
        }
    }
}
