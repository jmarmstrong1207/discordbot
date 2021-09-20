using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CritzlezOS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ulong LoggedMessageID { get; set; }
        public bool IsBotOn { get; set; }

        public MainWindow()
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

            // Initialize memeImages and memeImages/sources folders if it already does not exist

            // Stores images given by users for meme generating
            Directory.CreateDirectory("memeImages");

            // Stores the meme templates
            Directory.CreateDirectory("memeImages/templates");
        }

        public async void StopBotAsync()
        {
            await BotInherited.bot.StopAsync();
            IsBotOn = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsBotOn) StopBotAsync();
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            if (!IsBotOn)
                StartBotAsync();
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            if (IsBotOn)
            {
                StopBotAsync();
            }
        }
    }
}
