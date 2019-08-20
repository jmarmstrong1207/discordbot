using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_BotSharp
{
    class SpecialCommandMethods
    {
        public static Process CreateStream(string path, float vol)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{path}\" -ac 2 -filter:a \"volume = {Math.Abs(vol)}\" -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
        }

        public static Process CreateStreamMic(float vol)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-f dshow -i audio=\"VoiceMeeter Output (VB-Audio VoiceMeeter VAIO)\" -ac 2 -filter:a \"volume = {Math.Abs(vol)}\" -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
        }

        public static async Task PlayFileAsync(string filename, SocketCommandContext Context, int vol)
        {
            await Context.Message.DeleteAsync();
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            var botClient = Context.Guild.GetUser(BotInherited.bot.BOT_ID);
            var client = await channel.ConnectAsync();

            int ffmpegIndex = BotInherited.FfmpegInstances.FindIndex(f => f.Guild == Context.Guild);
            int SongInstanceIndex = BotInherited.SongQueueInstances.FindIndex(f => f.Guild == Context.Guild);

            if (channel != null)
            {
                if (ffmpegIndex == -1 && SongInstanceIndex == -1)
                {
                    using (var ffmpeg = SpecialCommandMethods.CreateStream(Path.GetFullPath(filename), vol))
                    using (var output = ffmpeg.StandardOutput.BaseStream)
                    using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
                        try
                        {
                            Console.WriteLine(output);
                            await output.CopyToAsync(discord);
                        }
                        finally
                        {
                            await discord.FlushAsync();
                            await client.StopAsync();
                        }
                }
                else await Context.Channel.SendMessageAsync("The bot is currently playing music");
            }
            else await Context.Channel.SendMessageAsync("You aren't in a voice channel");
        }
    }
}
