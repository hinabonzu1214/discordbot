using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;


namespace TestHoge
{
    class Program
    {
        int b = 100;
        int c = 3;
        int d = 100;
        int t = 0;
        string todayweather;
        int m = 0;
        int mk = 0;
        private DiscordSocketClient _client;
        private Dictionary<IUser, bool> lotteryWaitingState;
        public static CommandService _commands;
        public static IServiceProvider _services;





        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            lotteryWaitingState = new Dictionary<IUser, bool>();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
            _client.Log += Log;
            _commands = new CommandService();
            _services = new ServiceCollection().BuildServiceProvider();
            _client.MessageReceived += CommandRecieved;
            //次の行に書かれているstring token = "hoge"に先程取得したDiscordTokenを指定する。
            string token = "NzEwNzg4OTQxMDYxNTU0MTkw.Xr9t-Q.HGWWhA_0T9Ul0V1my8j04xw92gI";
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();


            await Task.Delay(-1);
        }
        public interface ITextChannel : IMessageChannel, IMentionable, INestedChannel, IGuildChannel, IChannel, ISnowflakeEntity, IEntity<ulong>, IDeletable

        {

            Task<IWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null);
            Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null);
            
            bool IsNsfw { get; }
            int SlowModeInterval { get; }
            string Topic { get; }
            
        }




        /// <summary>
        /// 何かしらのメッセージの受信
        /// </summary>
        /// <param name="msgParam"></param>
        /// <returns></returns>
        private async Task CommandRecieved(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            void Form1_Load(object sender, EventArgs e)
            {
                ////今日の天気予報の取得   
                string baseUrl = "https://weather.livedoor.com/forecast/webservice/json/v1";
                //東京都のID
                string cityname = "130010";

                string url = $"{baseUrl}?city={cityname}";
                string json = new HttpClient().GetStringAsync(url).Result;
                JObject jobj = JObject.Parse(json);

                todayweather = (string)((jobj["forecasts"][0]["telop"] as JValue).Value);//今日の天気の取得
                Console.WriteLine(todayweather);
            }


            //デバッグ用メッセージを出力
            Console.WriteLine("{0} {1}:{2}", message.Channel.Name, message.Author.Username, message);
            //メッセージがnullの場合
            if (message == null)
                return;

            //発言者がBotの場合無視する
            if (message.Author.IsBot)
                return;


            var context = new CommandContext(_client, message);

            //ここから記述--------------------------------------------------------------------------
            var CommandContext = message.Content;





            if (lotteryWaitingState.ContainsKey(message.Author) && lotteryWaitingState[message.Author])
            {
                if (CommandContext == "はい" || CommandContext == "いいえ")
                {
                    if (CommandContext == "はい")
                    {
                        // ここに抽選開始で「はい」を選んだときの処理
                        await message.Channel.SendMessageAsync("抽選開始 <開始番号 終了番号>を入力してください");
                    }
                    else if (CommandContext == "いいえ")
                    {
                        // ここに抽選開始で「いいえ」を選んだときの処理
                        await message.Channel.SendMessageAsync("終了します");
                    }
                }
                else if (Regex.IsMatch(CommandContext, "抽選開始"))
                {
                    // 抽選開始を選んだときの処理
                    string[] cmd = CommandContext.Split(' ');
                    int start = int.Parse(cmd[1]);
                    int end = cmd.Length < 3 ? 10 : int.Parse(cmd[2]);
                    Random r = new Random();
                    string random = Convert.ToString(r.Next(start, end));
                    await message.Channel.SendMessageAsync($">>> ***抽選結果は ||{random}|| になりました***🎉おめでとう🎉" + Environment.NewLine + "```当選した方との情報のやり取りは、今後DMを使用します。```" + Environment.NewLine + "***IN率が悪く、3日たっても反応しない場合は、当選を取り消します***" + Environment.NewLine + " " + Environment.NewLine + " " + Environment.NewLine + " " + Environment.NewLine + " ");
                }
                //lotteryWaitingState[message.Author] = false;
            }
            // コマンド("おはよう")かどうか判定
            if(CommandContext == "!mc" || CommandContext =="モードチェンジ")
            {
                mk = 1;
                await message.Channel.SendMessageAsync("***モードチェンジが実行されました***"+Environment.NewLine + "```!m1```写真ありで会話します" + Environment.NewLine + "```!m2```写真なしで会話します");
            }
            if(CommandContext == "!m1" && mk ==1)
            {
                m = 0;
                await message.Channel.SendMessageAsync("***写真ありモードで会話します。変更したい場合は***```!mc```と打ちましょう。");
                mk = 0;
            }
            if (CommandContext == "!m2" && mk == 1)
            {
                m = 1;
                await message.Channel.SendMessageAsync("***写真なしモードで会話します。変更したい場合は***```!mc```と打ちましょう。");
                mk = 0;
            }
            if (CommandContext == "コマンド一覧" || CommandContext == "cmdlist")
            {
                await message.Channel.SendMessageAsync(">>> ```コマンド一覧                                                                 ```" + Environment.NewLine + "***今表示してるやつだよ***" + Environment.NewLine + "```!mc or モードチェンジ```" + Environment.NewLine + "***写真ありで会話するかなしで会話するか選べるよ***" + Environment.NewLine + "```抽選開始```" + Environment.NewLine + "***抽選が開始されるよ***" + Environment.NewLine + "***はいorいいえで答えましょう***" + Environment.NewLine + "***抽選開始 範囲1 範囲2 例:抽選開始 1 10***" + Environment.NewLine + "```募集or人集め```" + Environment.NewLine + "***メンションがついて人を集めるよ***" + Environment.NewLine + "***!募集する人数で人を集めれるよ***" + Environment.NewLine + "***募集に参加したい人は、!ss で応募できます***" + Environment.NewLine + "```その他挨拶```" + Environment.NewLine + "***挨拶をすると返事してくれるよ***" + Environment.NewLine + "```慰めてorなぐさめて```" + Environment.NewLine + "***僕が優しく慰めるよ。***" + Environment.NewLine + "```おやすみorおやorGoodNight```" + Environment.NewLine + "***おやすみって言おうかな？***");
            }
            if (CommandContext == "おはよう" || CommandContext == "おは" || CommandContext == "おはようございます")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}おはよーーーん(o^―^o)ﾆｺ|   ||дﾟ)/");
            }
            if (CommandContext == "助けて" || CommandContext == "たすけて" || CommandContext == "help" || CommandContext == "怖い" || CommandContext == "こわい")
            {
                await message.Channel.SendMessageAsync("```もしかしてこいつにおびえているのか？```" + Environment.NewLine + "いやーーーーーーーーーーーーーー　来ないでーーーーーーーーーーーー" + Environment.NewLine + "うわーー" + Environment.NewLine + "バタ......うぐぅぅぅぅ" + Environment.NewLine + " https://tenor.com/view/pennywise-it-scary-smile-gif-12240248");
            }
            if (CommandContext == "こんにちは" || CommandContext == "こん" || CommandContext == "こんちゃ" || CommandContext == "今日は")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}こんにちわんこそば(o^―^o)ﾆｺ");
            }
            if (CommandContext == "こんばんは")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}こんばんは　あ、すいません人違いでした(￣ー￣)ﾆﾔﾘ");
            }
            if (CommandContext == "慰めて" || CommandContext == "なぐさめて" )
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}よしよしヾ(・ω・｀)泣かないで。いつでも話しておくれ。");
            }
            if (CommandContext == "慰めて" || CommandContext == "なぐさめて")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}よしよしヾ(・ω・｀)泣かないで。いつでも話しておくれ。");
            }
            if (CommandContext == "お疲れ" || CommandContext == "おつかれ" || CommandContext == "お疲れ様" || CommandContext == "疲れた" || CommandContext == "つかれた")
                if (m == 1)
                {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}お疲れ様です～～～～　(｀・∀・´)ｴｯﾍﾝ!! ");
            }
            if (CommandContext == "お疲れ" || CommandContext == "おつかれ" || CommandContext == "お疲れ様" || CommandContext == "疲れた" || CommandContext == "つかれた" )
                if (m == 0)
                {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}お疲れ様です～～～～　どうぞ https://tenor.com/view/kitten-cute-cat-lick-kiss-gif-12816950");
            }
            if (CommandContext == "おやすみ" || CommandContext == "おや" || CommandContext == "GoodNight")
                if (m == 0)
                {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}おやすみなさ～～～～～～～～～～～～～～～～～～～～～い https://tenor.com/view/ja-good-night-gif-4627454");
            }
            if (CommandContext == "おやすみ" || CommandContext == "おや" || CommandContext == "GoodNight" )
                if (m == 1)
                {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}おやすみなさ～～～～～～～～～～～～～～～～～～～～～い 💤💤💤"+ Environment.NewLine + "***はっ、私ボットだから寝なくていいんだった***");
            }
            /*if (CommandContext == "応募条件" || CommandContext == "応募条件教えて")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}さんどうぞ" + Environment.NewLine + ">>> ``` 1.主のYouTubeを登録する。```" + Environment.NewLine + "***https://www.youtube.com/channel/UCI9rZxe4J1psNV1sK44PnBA?view_as=subscriber***" + Environment.NewLine + "``` 2. https://pc.moppy.jp/entry/invite.php?invite=TuVHe1b0 から、Moppyに登録する```" + Environment.NewLine + " https://pc.moppy.jp/entry/invite.php?invite=TuVHe1b0" + Environment.NewLine + "***注:```フリーメールアドレス(例:gmail)などで登録した場合ポイントを300ポイント以上貯めること。```***" + Environment.NewLine +"```3.主のTwitterをフォローする。```" + Environment.NewLine + "https://twitter.com/hinap64002280");
            }*/
            //if (CommandContext == "人集め" || CommandContext == "募集")
            //{
            //  await message.Channel.SendMessageAsync($"{message.Author.Mention} さんが呼んでますよ");
            //}
            if (CommandContext == "おなかへった" || CommandContext == "はらへった")
                if (m == 0)
            {
                await message.Channel.SendMessageAsync($">>> {message.Author.Mention}さん、ぼくもーーーー```" + Environment.NewLine + "これ食べたいなーー" + Environment.NewLine + "https://cdn.discordapp.com/attachments/710021001391636556/711108774525403136/image0.jpg");
            }
            if (CommandContext == "おなかへった" || CommandContext == "はらへった" )
                if (m == 1)
                {
                await message.Channel.SendMessageAsync($">>> {message.Author.Mention}さん、ぼくもーーーー```" + Environment.NewLine + "これ食べたいなーー" + Environment.NewLine + "🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌🍌");
            }
            if (CommandContext == "@Fortnite抽選BOT")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention} さん 呼びましたか？");
            }
            if (CommandContext == "うーん" || CommandContext == "どうしよう" || CommandContext == "困るな" || CommandContext == "困った" || CommandContext == "こまった" )
                if (m == 0)
                {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}困ってますね。どうしよう、どうしよう、どうしよう、どうしよう。あっ、💡　　　https://cdn.discordapp.com/attachments/575919680015826945/711119390367547482/konwaku.mp4");
            }
            if (CommandContext == "うーん" || CommandContext == "どうしよう" || CommandContext == "困るな" || CommandContext == "困った" || CommandContext == "こまった" || CommandContext == "え" || CommandContext == "は？" || CommandContext == "え？" || CommandContext == "は？")
                if (m == 1)
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention}困ってますね。どうしよう、どうしよう、どうしよう、どうしよう。あっ、💡　　　****工藤新一さんにお願いしましょう**");
                }
            if (CommandContext == "抽選開始")
            {
                lotteryWaitingState[message.Author] = true;
                await message.Channel.SendMessageAsync("抽選を開始しますか?　はい or いいえ");
            }
            if (CommandContext == "今日の天気は" || CommandContext == "天気を教えて" || CommandContext == "天気をおしえて" || CommandContext == "今日の天気は?")
            {
                await message.Channel.SendMessageAsync(todayweather);
            }
            if (CommandContext == "!mlist" || CommandContext == "音楽一覧" || CommandContext == "曲一覧")
            {
                await message.Channel.SendMessageAsync(">>> ***おすすめのMUSIC List***" + Environment.NewLine + "***米津玄師***:落ち着きたいときに```Lemon,メトロノーム,春雷,Flamingo,orion,灰色と青,ピースサイン,LOSER,アイネクライネ,打ち上げ花火,海の幽霊```" + Environment.NewLine + "***あいみょん***```貴方解剖純愛歌 〜死ね〜,生きていたんだよな,愛を伝えたいだとか,君はロックを聴かない,ふたりの世界,マリーゴールド,今夜このまま,恋をしたから,ハルノヒ,空の青さを知る人よ```");
            }
            if (CommandContext == "シネ" || CommandContext == "しね" || CommandContext == "死ね")
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention}お前が、||死ね||よ。");
            }
            if (CommandContext == "BOT依頼")
            {
                await message.Channel.SendMessageAsync(">>> ***```ひーくんのボット作成依頼```***" + Environment.NewLine + "***料金***" + Environment.NewLine + "***体験版***:今表示してるボット(どこのサーバーでも使えるけどめんてがよくはいります)" + Environment.NewLine + "***基本代金***:100円(挨拶機能+そのサーバー限定の名前+BOT)" + Environment.NewLine + "***```+α機能```***" + Environment.NewLine + "***抽選機能***:50円" + Environment.NewLine + "***募集機能***:50円" + Environment.NewLine + "***隋自家発するたびにどんどん強化されて九プラン***:500円" + Environment.NewLine + "機能を追加するごとに料金を請求します。");
            }
            if (CommandContext == "!tl")
            {
                t = 1;
                await message.Channel.SendMessageAsync("本当にリセットしてもいいですか？ はいorいいえ");
            }
            if (CommandContext == "はい" && t ==1)
            {
                //var messages = await textChannel.GetMessagesAsync(250).FlattenAsync();
                //wait textChannel.DeleteMessagesAsync(messages);
                t = 0;
            }


            //ここから募集プログラム
            if (CommandContext == "募集" || CommandContext == "人集め")
            {
                await message.Channel.SendMessageAsync("募集する人数を書いてください　範囲は !0から!10までです");
                b = 1;
            }
            if (CommandContext == "!0" && b == 1)
            {
                await message.Channel.SendMessageAsync($">>> {message.Author.Mention}.....***ボッチなんだね***かわいそう" + Environment.NewLine + "https://tenor.com/view/milkandmocha-cry-sad-tears-upset-gif-11667710");
            }
            if (CommandContext == "!1" || CommandContext == "１" && b ==1)
            {
                b = 2;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!2" || CommandContext == "２" && b == 1)
            {
                b = 3;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b-1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!3" || CommandContext == "３" && b == 1)
            {
                b = 4;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b-1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!4" || CommandContext == "４" && b == 1)
            {
                b = 5;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!5" || CommandContext == "５" && b == 1)
            {
                b = 6;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!6" || CommandContext == "６" && b == 1)
            {
                b = 7;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!7" || CommandContext == "７" && b == 1)
            {
                b = 8;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!8" || CommandContext == "８" && b == 1)
            {
                b = 9;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!9" || CommandContext == "９" && b == 1)
            {
                b = 10;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!10" || CommandContext == "１０" && b == 1)
            {
                b = 11;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}が{b - 1}人の募集を開始しましたしました。参加する人は、!ssと打ちましょう");
            }
            if (CommandContext == "!ss")
            {
                c--;
                d = 0;
                await message.Channel.SendMessageAsync($"{message.Author.Mention}参加完了しました。残り募集人数は***{c - 1}***人です。");
            }
            if (c == 1)
            {
                c = 11;
                await message.Channel.SendMessageAsync("残り募集人数は***0***人です。");
                d = 100;
            }
            while(d != 0)
            {
                if (b == 2)
                {
                    c = 2;
                }
                if (b == 3)
                {
                    c = 3;
                }
                if (b == 4)
                {
                    c = 4;
                }
                if (b == 5)
                {
                    c = 5;
                }
                if (b == 6)
                {
                    c = 6;
                }
                if (b == 7)
                {
                    c = 7;
                }
                if (b == 8)
                {
                    c = 8;
                }
                if (b == 9)
                {
                    c = 9;
                }
                if (b == 10)
                {
                    c = 10;
                }
                if (b == 11)
                {
                    c = 11;
                }
                break;
            }
            
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}