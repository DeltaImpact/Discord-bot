// THIS FILE IS A PART OF EMZI0767'S BOT EXAMPLES
//
// --------
// 
// Copyright 2017 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// --------
//
// This is an interactivity example. It shows how to properly utilize 
// Interactivity module.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using Serilog;


namespace DSPlus.Examples
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public InteractivityModule Interactivity { get; set; }
        public CommandsNextModule Commands { get; set; }

        public static void Main(string[] args)
        {
            string[] pathsToLog = {"files", "bot-log.log"};
            string pathToLog = Path.Combine(pathsToLog);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(pathToLog)
                .CreateLogger();

            var position = new {botEvent = "loaded"};
            var tmp = JsonConvert.SerializeObject(position);
            Logger.SaveString(tmp);
            //Console.WriteLine(Memes.MemesString());
            //Logger.SaveEvent("bot", "awaken");

            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }


        public async Task RunBotAsync()
        {
            // first, let's load our configuration file
            var json = "";
            string[] pathsToConfig =
                {Path.GetFullPath(Directory.GetCurrentDirectory()), "files", "config.json"};
            string pathToConfigRaw = Path.Combine(pathsToConfig);

            ConfigJson? cfgjson = null;

            //Console.WriteLine(pathToConfigRaw);
            json = await File.ReadAllTextAsync(pathToConfigRaw);

            if (json == "")
            {
                throw new ArgumentNullException($"Loaded Json empty.");
            }

            cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            var cfg = new DiscordConfiguration
            {
                Token = cfgjson.Value.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            // then we want to instantiate our client
            Client = new DiscordClient(cfg);

            Client.GuildMemberAdded += Client_GuildMemberAdded;
            Client.GuildMemberUpdated += Client_GuildMemberUpdated;
            Client.MessageCreated += Client_MessageCreated;

            // If you are on Windows 7 and using .NETFX, install 
            // DSharpPlus.WebSocket.WebSocket4Net from NuGet,
            // add appropriate usings, and uncomment the following
            // line
            //this.Client.SetWebSocketClient<WebSocket4NetClient>();

            // If you are on Windows 7 and using .NET Core, install 
            // DSharpPlus.WebSocket.WebSocket4NetCore from NuGet,
            // add appropriate usings, and uncomment the following
            // line
            //this.Client.SetWebSocketClient<WebSocket4NetCoreClient>();

            // If you are using Mono, install 
            // DSharpPlus.WebSocket.WebSocketSharp from NuGet,
            // add appropriate usings, and uncomment the following
            // line
            //this.Client.SetWebSocketClient<WebSocketSharpClient>();

            // if using any alternate socket client implementations, 
            // remember to add the following to the top of this file:
            //using DSharpPlus.Net.WebSocket;

            // next, let's hook some events, so we know
            // what's going on
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientError;

            // let's enable interactivity, and set default options
            Client.UseInteractivity(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,

                // default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });

            // up next, let's set up our commands
            var ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = cfgjson.Value.CommandPrefix,

                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true
            };

            // and hook them up
            Commands = Client.UseCommandsNext(ccfg);

            // let's hook some command events, so we know what's 
            // going on
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //up next, let's register our commands
            Commands.RegisterCommands<ExampleInteractiveCommands>();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // finally, let's connect and log in
            await Client.ConnectAsync();

            // when the bot is running, try doing <prefix>help
            // to see the list of registered commands, and 
            // <prefix>help <command> to see help about specific
            // command.

            // and this is to prevent premature quitting
            await Task.Delay(-1);
        }

        private async Task<Task> Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Surely-bot",
                $"Joined user: {e.Member.Username}#{e.Member.Discriminator}({e.Member.Id})",
                DateTime.Now);

            AssignedNickname user =
                AssignedNicknames.GetUserByIdAndGuildId(e.Member.Id.ToString(), e.Guild.Id.ToString());
            if (user != null)
            {
                try
                {
                    await e.Member.ModifyAsync(user.NewNickname,
                        reason:
                        $"{e.Member.Username} ({e.Member.Id}) rejoin server with assigned nickname {user.NewNickname}.");

                    var logRecord = new
                    {
                        botEvent = "rejoin server with assigned nickname",
                        newNickname = user.NewNickname,
                        oldNickname = e.Member.DisplayName,
                        userDiscriminator = e.Member.Discriminator,
                        guild = e.Guild.Name,
                        userId = e.Member.Id,
                        guildId = e.Guild.Id
                    };
                    var tmp = JsonConvert.SerializeObject(logRecord);
                    Logger.SaveString(tmp);
                }
                catch (Exception exception)
                {
                    var position = new {botEvent = "exception", exception};
                    var tmp = JsonConvert.SerializeObject(position);
                    Logger.SaveString(tmp);

                    Console.WriteLine(exception);
                    //throw;
                }
            }
            else
            {
                string newNickname = Nickname.FixNickname(e.Member.DisplayName, e.Member.Discriminator);

                if (e.Member.DisplayName != Nickname.FixNickname(newNickname, e.Member.Discriminator))
                {
                    await e.Member.ModifyAsync(newNickname,
                        reason:
                        $"{e.Member.Username} ({e.Member.Id}) joined server with incorrect nickname. New nickname {newNickname}.");

                    var logRecord = new
                    {
                        botEvent = "join server with incorrect nickname",
                        newNickname = newNickname,
                        oldNickname = e.Member.DisplayName,
                        userDiscriminator = e.Member.Discriminator,
                        guild = e.Guild.Name,
                        userId = e.Member.Id,
                        guildId = e.Guild.Id
                    };
                    var tmp = JsonConvert.SerializeObject(logRecord);
                    Logger.SaveString(tmp);
                }
            }

            return Task.CompletedTask;
        }

        private async Task<Task> Client_GuildMemberUpdated(GuildMemberUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Surely-bot",
                $"Updated user: {e.Member.Username}#{e.Member.Discriminator}({e.Member.Id})",
                DateTime.Now);

            AssignedNickname user =
                AssignedNicknames.GetUserByIdAndGuildId(e.Member.Id.ToString(), e.Guild.Id.ToString());

            if (user != null)
            {
                try
                {
                    await e.Member.ModifyAsync(user.NewNickname,
                        reason:
                        $"{e.Member.Username} ({e.Member.Id}) tried to change assigned nickname {user.NewNickname}.");

                    var logRecord = new
                    {
                        botEvent = "tried to change assigned nickname",
                        newNickname = user.NewNickname,
                        oldNickname = e.Member.DisplayName,
                        userDiscriminator = e.Member.Discriminator,
                        guild = e.Guild.Name,
                        userId = e.Member.Id,
                        guildId = e.Guild.Id
                    };
                    var tmp = JsonConvert.SerializeObject(logRecord);
                    Logger.SaveString(tmp);
                }
                catch (Exception exception)
                {
                    var position = new {botEvent = "exception", exception};
                    var tmp = JsonConvert.SerializeObject(position);
                    Logger.SaveString(tmp);

                    Console.WriteLine(exception);
                    //throw;
                }
            }
            else
            {
                string newNickname = Nickname.FixNickname(e.Member.DisplayName, e.Member.Discriminator);

                if (e.Member.DisplayName != Nickname.FixNickname(newNickname, e.Member.Discriminator))
                {
                    await e.Member.ModifyAsync(newNickname,
                        reason:
                        $"{e.Member.Username} ({e.Member.Id}) tried to change nickname to inadmissible. New nickname {newNickname}.");

                    var logRecord = new
                    {
                        botEvent = "tried to change nickname to inadmissible",
                        newNickname = user.NewNickname,
                        oldNickname = e.Member.DisplayName,
                        userDiscriminator = e.Member.Discriminator,
                        guild = e.Guild.Name,
                        userId = e.Member.Id,
                        guildId = e.Guild.Id
                    };
                    var tmp = JsonConvert.SerializeObject(logRecord);
                    Logger.SaveString(tmp);
                }
            }

            return Task.CompletedTask;
        }

        private async Task<Task> Client_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author != Client.CurrentUser)
            {
                //Regex reg = new Regex(@"\\zxc{([а-яА-Я0-9 _]+)}");
                Regex reg = new Regex(@"\w+");
                List<string> mc = reg.Matches(e.Message.Content.ToLower()).Select(match => match.Value).ToList();
                if (mc.Contains("бот") && mc.Contains("где"))
                {
                    var searchItem = e.Message.Content.ToLower();
                    searchItem = searchItem.Replace("где", "");
                    searchItem = searchItem.Replace("падает", "");
                    searchItem = searchItem.Replace("бот", "");
                    searchItem = searchItem.Replace(".", string.Empty);
                    searchItem = searchItem.Replace(",", string.Empty);
                    searchItem = searchItem.Replace("?", string.Empty);
                    searchItem = searchItem.TrimStart();
                    searchItem = searchItem.TrimEnd();
                    e.Client.DebugLogger.LogMessage(LogLevel.Info, "Surely-bot",
                        $"Executed on wiki search: {e.Author.Username}#{e.Author.Discriminator}({e.Author.Id}) \"{searchItem}\"",
                        DateTime.Now);
                    List<SearchResult> items;
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = e.Message.Author is DiscordMember m
                            ? m.Color
                            : new DiscordColor(0xFF00FF),
                        Title = $"Цель поиска: \"{searchItem}\"{'.'}"
                    };
                    try
                    {
                        items = await WikiParsing.GetInfoAboutItem(searchItem);
                        foreach (var item in items)
                        {
                            embed.AddField(item.header, item.link);
                        }
                    }
                    catch (Exception)
                    {
                        embed.AddField("Не найден.", "Может поможет иной запрос?", true);
                    }


                    await e.Message.RespondAsync(embed: embed);
                }


                Meme command = Memes.FindMeme(e.Message.Content);

                if (command != null)
                {
                    string message = Memes.GenerateStringWithoutMeme(command, e.Message.Content);
                    var logRecord = new
                    {
                        botEvent = "meme command",
                        memeCommand = command.Name,
                        tetxtOfMessage = e.Message.Content,
                        authorUsername = e.Message.Author.Username,
                        authorDiscriminator = e.Message.Author.Discriminator,
                        guild = e.Message.Channel.Guild.Name,
                        authorId = e.Message.Author.Id,
                        guildId = e.Message.Channel.GuildId,
                        authorIsBot = e.Message.Author.IsBot
                    };
                    var tmp = JsonConvert.SerializeObject(logRecord);
                    Logger.SaveString(tmp);

                    await e.Message.DeleteAsync($"Because meme command {command}.");

                    var embed = new DiscordEmbedBuilder
                    {
                        Color = e.Message.Author is DiscordMember m
                            ? m.Color
                            : new DiscordColor(0xFF00FF),
                        Description = message,
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = e.Message.Author is DiscordMember mx
                                ? mx.DisplayName
                                : e.Message.Author.Username,
                            IconUrl = e.Message.Author.AvatarUrl,
                        },
                        ImageUrl = command.Path,
                    };
                    await e.Message.RespondAsync(embed: embed);
                }
            }

            return Task.CompletedTask;
        }


        private async Task<Task> Client_Ready(ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Surely-bot", "Client is ready to process events.",
                DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done


            string status = "sb!help " +
                            Client.Guilds.Count + " сервера " +
                            Memes.CountOfMemes + " файлов";

            await Client.UpdateStatusAsync(new DiscordGame(status));


            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Surely-bot", $"Guild available: {e.Guild.Name}",
                DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Surely-bot",
                $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "Surely-bot",
                $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            // let's log the error details
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Surely-bot",
                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}",
                DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }
    }


    // this structure will hold data from config.json
    public struct ConfigJson
    {
        [JsonProperty("token")] public string Token { get; private set; }

        [JsonProperty("prefix")] public string CommandPrefix { get; private set; }
        //[JsonProperty("log file")] public string LogPath { get; private set; }
    }
}