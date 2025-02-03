using AutoForwarder.Service.Exceptions;
using AutoForwarder.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TL;
using WTelegram;

namespace AutoForwarder.Service.Services;

public class UserBotService : IUserBotService
{
    private readonly Client client;
    private readonly ILogger<UserBotService> logger;
    public UserBotService(
        Client client,
        IConfiguration configuration,
        ILogger<UserBotService> logger)
    {
        this.client = client;
        this.logger = logger;
    }
    public async Task ForwardMessageAsync(CancellationToken cancellationToken)
    {
        try
        {
            var user = await this.client.LoginUserIfNeeded();
            var selfId = user.id;


            long targetChatId = 2020480421;
            long targetAccessHash = 4824410005556210455;

            this.client.OnUpdates += async updates =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return; // Eventni to'xtatish
                    }
                    if (updates is Updates updatesObj)
                    {
                        foreach (var update in updatesObj.updates)
                        {
                            if (update is UpdateNewMessage newMessage && newMessage.message is Message message)
                            {
                                // Target guruhdan xabar kelganda o'tkazib yuborish
                                if ((message.peer_id is PeerChannel peerChannel && peerChannel.channel_id == targetChatId))
                                {
                                    continue;
                                }

                                ChatBase chat = updatesObj.chats.TryGetValue(message.peer_id, out var foundChat) ? foundChat : null;
                                if (message.from_id is PeerUser peerUser1)
                                {
                                    if (!updatesObj.users.TryGetValue(peerUser1.user_id, out var foundUser))
                                    {
                                        var userList = await client.Users_GetUsers(new InputUser(peerUser1.user_id, 0));
                                        foundUser = userList.OfType<User>().FirstOrDefault();
                                    }
                                    User sender = foundUser;
                                    if (sender?.IsBot == true)
                                        continue;

                                    if (message.from_id is PeerUser peerUser2)
                                    {
                                        string senderName = sender?.first_name ?? "None";

                                        string senderUsername = sender != null && !string.IsNullOrEmpty(sender.username)
                                                                ? $"[{sender.username.Replace("_", "\\_")}](https://t.me/{sender.username})"
                                                                : "@None";

                                        string senderPhone = sender?.phone ?? "None";
                                        string groupLink = updatesObj.chats.TryGetValue(message.peer_id, out var chat1)
                                            ? $"[{chat1.Title}](https://t.me/{chat1.MainUsername.Replace("_", "\\_")})"
                                            : "Group username mavjud emas";
                                        string userLinkAndroid = $"[{senderName}](tg://openmessage?user_id={sender?.ID})";
                                        string userLinkIOS = $"[{senderName}](tg://user?id={sender?.ID})";
                                        string messageLink = $"[havola](https://t.me/c/{chat?.ID}/{message?.id})";

                                        string text = message.message;
                                        if (message?.media != null)
                                            text = "Guruhga ovozli xabar yoki media keldi";

                                        string forwardMessage =
                                            $"📜 *Xabar*: \n{text}\n\n" +
                                            $"👤 *Yuboruvchi Android*: {userLinkAndroid}\n" +
                                            $"👤 *Yuboruvchi IOS*: {userLinkIOS}\n" +
                                            $"📞 *Raqami*: +{senderPhone}\n" +
                                            $"🏷️ *Username*: {senderUsername}\n" +
                                            $"📌 *Group*: {groupLink}\n" +
                                            $"👉 *Xabarga o'tish*: {messageLink}\n";

                                        var entities = client.MarkdownToEntities(ref forwardMessage);

                                        await client.SendMessageAsync(
                                                new InputPeerChannel(targetChatId, targetAccessHash),
                                                forwardMessage,
                                                entities: entities
                                            );

                                    }
                                    else
                                    {
                                        this.logger.LogInformation("Foydalanuvchi topilmadi");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new AutoForwarderException(500, ex.Message);
                }
            };
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new AutoForwarderException(500, ex.Message);
        }
    }
}
