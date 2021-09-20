using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace CritzlezOS
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
            eb.AddField(",roll", "Roll from 0 to 100 or from roll #");
            eb.AddField(",upgrade *image-url image2-url*", "Lets you generate a meme with, using the *upgrade; " +
                "go back* meme template (images respective)");
            eb.WithColor(Discord.Color.Red);

            await Context.User.SendMessageAsync("", false, eb.Build());
        }

        [Command("ping", RunMode = RunMode.Async)]
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync("pong");
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
                await Context.User.SendMessageAsync("You aren't James");
            }
        }

        [Command("upgrade", RunMode = RunMode.Async)]
        public async Task Upgrade(string image, string image2)
        {
            await Context.Message.DeleteAsync();

            MemeGenerator upgrade = new MemeGenerator(Environment.CurrentDirectory + "/memeImages/sources/upgrade.jpg",
                Context, image, image2);

            string memeToSend = upgrade.Generate2ImageMeme(
                resizex: 299, resizey: 299,
                whereToPlacex: 0, whereToPlacey: 0,
                resizexx: 299, resizeyy: 300,
                whereToPlacexx: 0, whereToPlaceyy: 300);

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
            }

            var eb = new EmbedBuilder();
            eb.WithTitle("oh god thanos no why did you snap your fingers");
            eb.WithColor(Discord.Color.Red);
            foreach (ulong id in peopleToKill)
            {
                eb.Description += "<@!" + id + ">\n";
            }
            eb.AddField("Oh god thanos you killed half of the server", "-");

            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }
    }
}
