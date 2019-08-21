using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_BotSharp
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private Random rnd = new Random();

        [Command("help", RunMode = RunMode.Async)]
        public async Task Help()
        {
            var eb = new EmbedBuilder();

            eb.WithTitle("Help Commands");
            eb.AddField(",ping", "Replies with pong. Tests for lag");
            eb.AddField(",8ball", "Answers your essential questions");
            eb.AddField(",knock", "Spook someone with this in a voice channel");
            eb.AddField(",roll", "Roll from 0 to 100 or from roll #");
            eb.AddField(",upgrade *image-url image2-url*", "Lets you generate a meme with, using the *upgrade; " +
                "go back* meme template (images respective)");
            eb.AddField(",upgrade *image-url image2-url image3-url*", "Lets you generate a meme with, " +
                "using the *upgrade; go back; I SAID GO BACK* meme template (images respective)");
            eb.WithColor(Discord.Color.Red);

            await Context.User.SendMessageAsync("", false, eb);
        }

        [Command("ping", RunMode = RunMode.Async)]
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync("pong");
        }

        [Command("knock", RunMode = RunMode.Async)]
        public async Task Knock()
        {
            await SpecialCommandMethods.PlayFileAsync("knocking.mp3", Context, 1);
        }

        [Command("bruh", RunMode = RunMode.Async)]
        public async Task Bruh()
        {
            await SpecialCommandMethods.PlayFileAsync("bruh.mp3", Context, 1);
        }

        [Command("knock", RunMode = RunMode.Async)]
        public async Task Knock(int vol)
        {
            await SpecialCommandMethods.PlayFileAsync("knocking.mp3", Context, vol);
        }

        [Command("roll", RunMode = RunMode.Async)]
        public async Task Roll(int number)
        {
            await Context.Channel.SendMessageAsync(rnd.Next(0, number + 1).ToString());
        }

        // Override if there's no number inputted
        [Command("roll", RunMode = RunMode.Async)]
        public async Task Roll()
        {
            await Context.Channel.SendMessageAsync(rnd.Next(0, 101).ToString());
        }

        [Command("mic", RunMode = RunMode.Async)]
        public async Task Mic()
        {
            bool loopIt = true;
            int ffmpegIndex = BotInherited.FfmpegInstances.FindIndex(f => f.Guild == Context.Guild);
            int PID = 0;
            IVoiceChannel channel = (Context.User as IGuildUser)?.VoiceChannel;
            SocketGuildUser botClient = Context.Guild.GetUser(BotInherited.bot.BOT_ID);
            IAudioClient client = await channel.ConnectAsync();

            await Context.Message.DeleteAsync();

            if (channel != null)
            {
                using (var ffmpeg = SpecialCommandMethods.CreateStreamMic(.3f))
                using (var output = ffmpeg.StandardOutput.BaseStream)
                using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
                    try
                    {
                        PID = ffmpeg.Id;

                        if (ffmpegIndex != -1)
                        {
                            BotInherited.FfmpegInstances.RemoveAt(ffmpegIndex);
                        }

                        BotInherited.FfmpegInstances.Add(new Ffmpeg(PID, Context.Guild));
                        ffmpegIndex = BotInherited.FfmpegInstances.FindIndex(f => f.Guild == Context.Guild);

                        await output.CopyToAsync(discord);

                        while (loopIt)
                        {
                            try
                            {
                                Process.GetProcessById(PID);
                            }
                            catch (Exception e)
                            {
                                await client.StopAsync();
                                loopIt = false;
                                Console.WriteLine(e);
                                BotInherited.FfmpegInstances.RemoveAt(ffmpegIndex);
                            }
                        }
                    }
                    finally
                    {
                        await discord.FlushAsync();
                    }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You aren't in a voice channel");
            }
        }

        [Command("say", RunMode = RunMode.Async)]
        public async Task Say([Remainder]string whatToSay)
        {
            await Context.Message.DeleteAsync();

            if (Context.User.Id == 171417090811494400)
            {
                await Context.Channel.SendMessageAsync(whatToSay);
            }
            else
            {
                await Context.User.SendMessageAsync("You aren't Critzlez you dumb libtard");
            }
        }

        [Command("drake", RunMode = RunMode.Async)]
        public async Task Meme(string image, string image2)
        {
            await Context.Message.DeleteAsync();

            MemeGenerator drake = new MemeGenerator(Environment.CurrentDirectory + "/meme-images/sources/drake.jpg", Context.Channel.Id, Context.Guild, image, image2);
            try
            {
                await Context.Channel.SendFileAsync(drake.Generate2ImageMeme(
                               resizex: 318, resizey: 255,
                               whereToPlacex: 322, whereToPlacey: 0,
                               resizexx: 318, resizeyy: 255,
                               whereToPlacexx: 322, whereToPlaceyy: 261
                               ));
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.ToString());
            }
        }

        [Command("upgrade", RunMode = RunMode.Async)]
        public async Task Upgrade(string image, string image2)
        {
            await Context.Message.DeleteAsync();

            MemeGenerator upgrade = new MemeGenerator(Environment.CurrentDirectory + "/meme-images/sources/upgrade.jpg",
                Context.Channel.Id, Context.Guild, image, image2);

            string memeToSend = upgrade.Generate2ImageMeme(
                resizex: 299, resizey: 299,
                whereToPlacex: 0, whereToPlacey: 0,
                resizexx: 299, resizeyy: 300,
                whereToPlacexx: 0, whereToPlaceyy: 300);

            await Context.Channel.SendFileAsync(memeToSend);
        }

        [Command("upgrade2", RunMode = RunMode.Async)]
        public async Task Upgrade2(string image, string image2, string image3)
        {
            await Context.Message.DeleteAsync();

            MemeGenerator upgrade = new MemeGenerator(Environment.CurrentDirectory + "/meme-images/sources/upgrade2.jpg",
                Context.Channel.Id, Context.Guild, image, image2, image3);

            string memeToSend = upgrade.Generate3ImageMeme(
                resizex: 242, resizey: 146,
                whereToPlacex: 0, whereToPlacey: 0,
                resizexx: 242, resizeyy: 163,
                whereToPlacexx: 0, whereToPlaceyy: 147,
                resizexxx: 242, resizeyyy: 165,
                whereToPlacexxx: 0, whereToPlaceyyy: 309);

            await Context.Channel.SendFileAsync(memeToSend);
        }

        [Command("howold", RunMode = RunMode.Async)]
        public async Task HowOld()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Context.User.ToString() + "\'s account was created at " + Context.User.CreatedAt.ToLocalTime().ToString());
        }

        [Command("howold", RunMode = RunMode.Async)]
        public async Task HowOld(ulong id)
        {
            await Context.Message.DeleteAsync();

            SocketGuildUser user = Context.Guild.GetUser(id);

            await Context.Channel.SendMessageAsync(user.ToString() + "\'s account was created at " + user.CreatedAt.ToLocalTime().ToString());
        }

        [Command("impact", RunMode = RunMode.Async)]
        public async Task Impact(string image, string text)
        {
            try
            {
                MemeGenerator upgrade = new MemeGenerator(image,
                Context.Channel.Id, Context.Guild);

                string memeToSend = upgrade.GenerateImpactMeme(text, "test");
                await Context.Channel.SendMessageAsync(memeToSend);

                await Context.Channel.SendFileAsync(memeToSend);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.ToString());

            }
        }

        [Command("snap", RunMode = RunMode.Async)]
        public async Task Snap()
        {
            await Context.Message.DeleteAsync();

            List<ulong> peopleToKill = new List<ulong>();
            List<SocketGuildUser> userList = new List<SocketGuildUser>();

            foreach (SocketGuildUser x in Context.Guild.Users) userList.Add(x);

            for (int i = 1; i <= Context.Guild.Users.Count / 2; i++)
            {
                ulong id = userList[rnd.Next(userList.Count)].Id;
                if (id != BotInherited.bot.BOT_ID)
                {
                    peopleToKill.Add(id);
                }
                else i--;
            }

            var eb = new EmbedBuilder();
            eb.WithTitle("oh god thanos no why did you snap your fingers");
            eb.WithColor(Discord.Color.Red);
            foreach (ulong id in peopleToKill)
            {
                eb.Description += "<@!" + id + ">\n";
            }
            eb.AddField("Oh god thanos you killed half of the server", "-");

            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("gnome", RunMode = RunMode.Async)]
        public async Task Gnome()
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
                    using (var ffmpeg = SpecialCommandMethods.CreateStream(Path.GetFullPath("gnome.m4a"), .1f))
                    using (var output = ffmpeg.StandardOutput.BaseStream)
                    using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
                        try
                        {
                            BotInherited.FfmpegInstances.Add(new Ffmpeg(ffmpeg.Id, Context.Guild));
                            ffmpegIndex = BotInherited.FfmpegInstances.FindIndex(f => f.Guild == Context.Guild);
                            Console.WriteLine(output);
                            await output.CopyToAsync(discord);
                        }
                        finally
                        {
                            BotInherited.FfmpegInstances.RemoveAt(ffmpegIndex);
                            await discord.FlushAsync();
                            await client.StopAsync();
                        }
                }
                else await Context.Channel.SendMessageAsync("The bot is currently playing something");
            }
            else await Context.Channel.SendMessageAsync("You aren't in a voice channel");
        }
    }
}
