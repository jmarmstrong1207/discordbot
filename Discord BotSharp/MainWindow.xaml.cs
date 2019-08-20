using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord_BotSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MainStart 
    {
        public ulong LoggedMessageID { get; set; }
        public bool IsBotOn { get; set; }
        public MainStart()
        {
            InitializeComponent();

            IsBotOn = false;
            LoggedMessageID = 0;
        }

        public string GetLogFormat()
        {
            string channelName = BotInherited.bot.Message.Channel.ToString();
            if (channelName.Length > 20) channelName = channelName.Substring(0, 20) + "...";

            DateTime time = DateTime.Now;
            string minute = DateTime.Now.Minute.ToString();
            if (int.Parse(minute) < 10)
            {
                minute = "0" + minute;
            }
            
            return "\n\n" + time.Year + "/" + time.Month + "/" + time.Day + " @ " +
                time.Hour + ":" + minute + " |" + BotInherited.bot.Context.Guild + "|" + channelName + "| " + 
                BotInherited.bot.Context.User + ": " + BotInherited.bot.LogMessage;
        }

        public async void StartBotAsync()
        {
            await BotInherited.bot.MainAsync();
            IsBotOn = true;
            Ffmpeg.MasterStop = false;

            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "songs"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "memeImages"));
        }

        public async void StopBotAsync()
        {
            Ffmpeg.MasterStop = true;
            if (Ffmpeg.KillAllFfmpegAndSongQueueInstanceProcesses())
            {
                await BotInherited.bot.StopAsync();
            }

            BotInherited.FfmpegInstances.Clear();
            BotInherited.SearchInstances.Clear();
            BotInherited.SongQueueInstances.Clear();

            IsBotOn = false;
        }

        public async void LogChatAsync()
        {
            while (true)
            {
                if (BotInherited.bot.MessageID != LoggedMessageID)
                {
                    Log.Text += GetLogFormat();
                    LoggedMessageID = BotInherited.bot.MessageID;
                    Log.ScrollToEnd();
                }

                await Task.Delay(100);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsBotOn) StopBotAsync();
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            if (!IsBotOn)
            {
                StartBotAsync();
                LogChatAsync();
                Log.Text += "\n" + "Bot Started\n" + "-----------------------";
            }
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            if (IsBotOn)
            {
                StopBotAsync();
                Log.Text += "\nBot Stopped\n" + "-----------------------";
            }
        }
    }
}
