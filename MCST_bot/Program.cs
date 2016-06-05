using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MCST_bot
{
    class Program
    {
        static bool serverdown = false;
        static void Main(string[] args)
        {
          
            Run().Wait();
        }

        static async Task Run()
        {
            var Bot = new Api("Apikey");

            var me = await Bot.GetMe();

            Console.WriteLine("Hello my name is {0}", me.Username);

            var offset = 0;

            TcpClient MinecraftServer = new TcpClient();
            MineStat ms = null;
            while (true)
            {
                var updates = await Bot.GetUpdates(offset);

                foreach (var update in updates)
                {
                    if (update.Message.Text == "/start")
                    {
                        await Bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
                        await Bot.SendTextMessage(update.Message.Chat.Id, "그냥 URL이나 서버 ip를 입력해주시면 서버 상태를 알려줍니다");
                    }
                    else
                    {
                        Console.WriteLine(update.Message.Text);
                        await Bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
                        await Bot.SendTextMessage(update.Message.Chat.Id, "처리중");
                        ms = new MineStat(update.Message.Text, 25565);
                        Console.WriteLine("Minecraft server status of {0} on port {1}:", ms.GetAddress(), ms.GetPort());
                        if (ms.IsServerUp())
                        {
                            await Bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
                            await Bot.SendTextMessage(update.Message.Chat.Id, "버전:" + ms.GetVersion() + " 접속자수:" + ms.GetCurrentPlayers() + " 최대인원:" + ms.GetMaximumPlayers() + " 메세지:" + ms.GetMotd());

                            Console.WriteLine("Server is online running version {0} with {1} out of {2} players.", ms.GetVersion(), ms.GetCurrentPlayers(), ms.GetMaximumPlayers());
                            Console.WriteLine("Message of the day: {0}", ms.GetMotd());
                        }
                        else
                        {
                            await Bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
                            await Bot.SendTextMessage(update.Message.Chat.Id, "서버다운");
                        }
                    }
                    offset = update.Id + 1;
                }

            }
        }

       
    }
}
