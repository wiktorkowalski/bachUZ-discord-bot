using System;
using System.Collections.Concurrent;
namespace BachUZ.Services
{
    class UserOnVoice
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public ulong VoiceChannelId { get; set; }
        public DateTime JoinTime { get; set; }
    }
    static class VoicechatMonitoringService
    {
        public static ConcurrentDictionary<ulong, UserOnVoice> UsersOnVoiceChannels = new ConcurrentDictionary<ulong, UserOnVoice>();
    }
}
