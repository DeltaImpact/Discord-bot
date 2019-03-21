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
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSPlus.Examples.ExtensionMethods;
using Newtonsoft.Json;

namespace DSPlus.Examples
{
    // note that in here we explicitly ask for duration. This is optional,
    // since we set the defaults.
    public class ExampleInteractiveCommands
    {
        /*
        [Command("poll"), Description("Run a poll with reactions.")]
        public async Task Poll(CommandContext ctx, [Description("How long should the poll last.")] TimeSpan duration,
            [Description("What options should people have.")] params DiscordEmoji[] options)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();
            var poll_options = options.Select(xe => xe.ToString());

            // then let's present the poll
            var embed = new DiscordEmbedBuilder
            {
                Title = "Poll time!",
                Description = string.Join(" ", poll_options)
            };
            var msg = await ctx.RespondAsync(embed: embed);

            // add the options as reactions
            for (var i = 0; i < options.Length; i++)
                await msg.CreateReactionAsync(options[i]);

            // collect and filter responses
            var poll_result = await interactivity.CollectReactionsAsync(msg, duration);
            var results = poll_result.Reactions.Where(xkvp => options.Contains(xkvp.Key))
                .Select(xkvp => $"{xkvp.Key}: {xkvp.Value}");

            // and finally post the results
            await ctx.RespondAsync(string.Join("\n", results));
        }

        [Command("waitforcode"), Description("Waits for a response containing a generated code.")]
        public async Task WaitForCode(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // generate a code
            var codebytes = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(codebytes);

            var code = BitConverter.ToString(codebytes).ToLower().Replace("-", "");

            // announce the code
            await ctx.RespondAsync($"The first one to type the following code gets a reward: `{code}`");

            // wait for anyone who types it
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Content.Contains(code),
                TimeSpan.FromSeconds(60));
            if (msg != null)
            {
                // announce the winner
                await ctx.RespondAsync($"And the winner is: {msg.Message.Author.Mention}");
            }
            else
            {
                await ctx.RespondAsync("Nobody? Really?");
            }
        }

        [Command("waitforreact"), Description("Waits for a reaction.")]
        public async Task WaitForReaction(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // specify the emoji
            var emoji = DiscordEmoji.FromName(ctx.Client, ":point_up:");

            // announce
            await ctx.RespondAsync($"React with {emoji} to quote a message!");

            // wait for a reaction
            var em = await interactivity.WaitForReactionAsync(xe => xe == emoji, ctx.User, TimeSpan.FromSeconds(60));
            if (em != null)
            {
                // quote
                var embed = new DiscordEmbedBuilder
                {
                    Color = em.Message.Author is DiscordMember m ? m.Color : new DiscordColor(0xFF00FF),
                    Description = em.Message.Content,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = em.Message.Author is DiscordMember mx ? mx.DisplayName : em.Message.Author.Username,
                        IconUrl = em.Message.Author.AvatarUrl
                    }
                };
                await ctx.RespondAsync(embed: embed);
            }
            else
            {
                await ctx.RespondAsync("Seriously?");
            }
        }

        [Command("waitfortyping"), Description("Waits for a typing indicator.")]
        public async Task WaitForTyping(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // then wait for author's typing
            var chn = await interactivity.WaitForTypingChannelAsync(ctx.User, TimeSpan.FromSeconds(60));
            if (chn != null)
            {
                // got 'em
                await ctx.RespondAsync($"{ctx.User.Mention}, you typed in {chn.Channel.Mention}!");
            }
            else
            {
                await ctx.RespondAsync("*yawn*");
            }
        }

        [Command("sendpaginated"), Description("Sends a paginated message.")]
        public async Task SendPaginated(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // generate pages.
            var lipsum = "123 " +
                         "34";
            var lipsum_pages = interactivity.GeneratePagesInEmbeds(lipsum);

            // send the paginator
            await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, lipsum_pages, TimeSpan.FromMinutes(5),
                TimeoutBehaviour.Delete);
        }


        [Command("pointTo"), Description("Point to person")]
        public async Task PointTo(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // specify the emoji
            var emoji = DiscordEmoji.FromName(ctx.Client, ":x:");

            // announce
            await ctx.RespondAsync($"Поставь эмодзи {emoji} сообщению пользователя на которого хочешь указать.");

            // wait for a reaction
            var em = await interactivity.WaitForReactionAsync(xe => xe == emoji, ctx.User, TimeSpan.FromSeconds(60));
            if (em != null)
            {
                // quote
                var embed = new DiscordEmbedBuilder
                {
                    Color = em.Message.Author is DiscordMember m ? m.Color : new DiscordColor(0xFF00FF),
                    //Description = em.Message.Content,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = em.Message.Author is DiscordMember mx ? mx.DisplayName : em.Message.Author.Username,
                        IconUrl = em.Message.Author.AvatarUrl
                    }
                };
                await ctx.RespondAsync(embed: embed);
            }
            else
            {
                await ctx.RespondAsync("Не дождался.");
            }
        }



        [Command("waitforreact1"), Description("Waits for a reaction.")]
        public async Task WaitForReaction1(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // specify the emoji
            var emoji = DiscordEmoji.FromName(ctx.Client, ":point_up:");

            // announce
            await ctx.RespondAsync($"React with {emoji} to quote a message!");

            // wait for a reaction
            var em = await interactivity.WaitForReactionAsync(xe => xe == emoji, ctx.User, TimeSpan.FromSeconds(60));
            if (em != null)
            {
                // quote
                var embed = new DiscordEmbedBuilder
                {
                    Color = em.Message.Author is DiscordMember m ? m.Color : new DiscordColor(0xFF00FF),
                    Description = em.Message.Content,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = em.Message.Author is DiscordMember mx ? mx.DisplayName : em.Message.Author.Username,
                        IconUrl = em.Message.Author.AvatarUrl
                    }
                };
                await ctx.RespondAsync(embed: embed);
            }
            else
            {
                await ctx.RespondAsync("Seriously?");
            }
        }



        [Command("paste"), Description("Example.")]
        public async Task Paste(CommandContext ctx)
        {
            //Console.WriteLine("1");



            var embed = new DiscordEmbedBuilder
            {
                Color = ctx.Message.Author is DiscordMember m ? m.Color : new DiscordColor(0xFF00FF),
                Description = "Все ссылки пока будут здесь.",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = ctx.Message.Author is DiscordMember mx ? mx.DisplayName : ctx.Message.Author.Username,
                    IconUrl = ctx.Message.Author.AvatarUrl
                },

            };
            embed.AddField("Нужна игра? - Скачать можно по ссылкам:", " https://factorio.com/ ");
            embed.AddField("Google Drive:", "  https://goo.gl/SJA1dE ");
            embed.AddField("Mega:", "  https://goo.gl/U11ezu ");
            //embed.AddField("Все ссылки пока будут здесь.", " ");

            //await ctx.RespondAsync("embed: ");

            Console.WriteLine("2");

            //ctx.Message.del
            await ctx.RespondAsync(embed: embed);



        }


        */


        [Command("memes"), Description("Лист названий возможных мемов.")]
        public async Task List(CommandContext ctx)
        {
            var embed = Memes.MemesEmbed();

            await ctx.RespondAsync(embed: embed);


            ////var pages = IEnumerable<Page>;
            //List<Page> Pages = new List<Page>();

            //// first retrieve the interactivity module from the client
            //var interactivity = ctx.Client.GetInteractivityModule();

            //foreach (var memeWFWikiCategory in Memes.MemeWfWikiDictionary.Keys)
            //{
            //    foreach (var memeWFWiki in Memes.MemeWfWikiDictionary[memeWFWikiCategory])
            //    {
            //        var embed = new DiscordEmbedBuilder
            //        {
            //            Title = memeWFWiki.Name,
            //            Description = memeWFWikiCategory,
            //            ImageUrl = memeWFWiki.Path,
            //        };

            //        var page = new Page();
            //        page.Embed = embed;
            //        Pages.Add(page);
            //    }
            //}

            //// send the paginator

            //await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, Pages, TimeSpan.FromMinutes(5),
            //    TimeoutBehaviour.Delete);
        }

        [Command("dictionary"), Description("Книга с мемами.")]
        public async Task DictionaryOfMemes(CommandContext ctx)
        {
            //var pages = IEnumerable<Page>;
            List<Page> Pages = new List<Page>();

            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            foreach (var memeWFWikiCategory in Memes.MemeWfWikiDictionary.Keys)
            {
                foreach (var memeWFWiki in Memes.MemeWfWikiDictionary[memeWFWikiCategory])
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = memeWFWiki.Name,
                        Description = memeWFWikiCategory,
                        ImageUrl = memeWFWiki.Path,
                    };

                    var page = new Page();
                    page.Embed = embed;
                    Pages.Add(page);
                }
            }

            // send the paginator

            await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, Pages, TimeSpan.FromMinutes(5),
                TimeoutBehaviour.Delete);
        }

        [Command("reload"), Description("Перезагрузка списка мемов.")]
        public async Task UpdateMemes(CommandContext ctx)
        {
            if (ctx.User.Id == 131456507018477568)
            {
                int categoriesBefore = Memes.CountOfCategories;
                int memesBefore = Memes.CountOfMemes;
                Memes.LoadMemesInfo();
                int categoriesAfter = Memes.CountOfCategories;
                int memesAfter = Memes.CountOfMemes;

                var embed = new DiscordEmbedBuilder();
                embed.AddField("Было", categoriesBefore + " категорий, содержащих " + memesBefore + " файлов");
                embed.AddField("Стало", categoriesAfter + " категорий, содержащих " + memesAfter + " файлов");

                await ctx.RespondAsync(embed: embed);
            }
            else
            {
                await ctx.RespondAsync("Ты моя мамочка?");
            }
        }

        [Command("clearNicknames"),
         Description("Берет всех пользователей без ролей, и проверяет допустимость их ников.")]
        public async Task ClearNicknames(CommandContext ctx,
            [Description("Должен ли бот также сбросить допустимые ники. [y/n]")]
            string saveAcceptableNickanames = "y"
        )
        {
            if (
                ctx.User.Id == 131456507018477568 ||
                ctx.Member.IsOwner)
            {
                var embedAboutStart = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "Чистка ников инициирована.",
                        IconUrl = ctx.Guild.CurrentMember.AvatarUrl
                    }
                };
                embedAboutStart.AddField("Должен ли бот также оставить допустимые ники. [y/n] [y by default]",
                    saveAcceptableNickanames);
                await ctx.RespondAsync(embed: embedAboutStart);

                var embedWithUserCounter = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "Чистка ников начата.",
                        IconUrl = ctx.Guild.CurrentMember.AvatarUrl
                    }
                };
                embedWithUserCounter.AddField("Число участников на сервере.", ctx.Guild.MemberCount.ToString());

                var members = ctx.Guild.Members.ToList();
                var membersAvailableToInteractionList = new List<DiscordMember>();

                foreach (var member in members)
                {
                    if (MemberExtensions.CanInteract(ctx.Guild.CurrentMember, member))
                    {
                        membersAvailableToInteractionList.Add(member);
                        //var roles = member.Roles.ToList();
                        //if (roles.Count == 0 && saveAcceptableNickanames == "y")
                        //    membersAvailableToInteractionList.Add(member);
                        //if (saveAcceptableNickanames == "n") membersAvailableToInteractionList.Add(member);
                    }
                }

                embedWithUserCounter.AddField("Число участников на сервере, к взаимодействию с которыми есть доступ.",
                    membersAvailableToInteractionList.Count.ToString());

                foreach (var member in membersAvailableToInteractionList)
                {
                    try
                    {
                        await ClearUserNickname(member, ctx.Guild, saveAcceptableNickanames == "y");
                    }
                    catch (Exception e)
                    {
                        await ctx.RespondAsync(e.Message);
                        Console.WriteLine(e);
                        //throw;
                    }
                }

                await ctx.RespondAsync(embed: embedWithUserCounter);
            }
            else
            {
                await ctx.RespondAsync("Это команду может использовать только владелец сервера.");
            }
        }

        private async Task ClearUserNickname(DiscordMember member, DiscordGuild ctxGuild,
            bool shouldBotSaveAcceptableNicknames)
        {
            string cleanedCurrentDisplayName = Nickname.FixNickname(member.DisplayName, member.Discriminator);
            string cleanedCurrentUsername = Nickname.FixNickname(member.Username, member.Discriminator);

            if (shouldBotSaveAcceptableNicknames) // Если бот не должен трогать допустимые ники
            {
                if (cleanedCurrentDisplayName != member.DisplayName) //И текущий не допустим
                {
                    await UpdateUser(member, cleanedCurrentDisplayName, ctxGuild); //Сменить ник
                }
            }
            else // Если бот должен трогать допустимые ники
            {
                if (member.DisplayName != cleanedCurrentUsername) // И подчищенный юзернейм отличаетс отличается от ника на севере.
                {
                    await UpdateUser(member, cleanedCurrentUsername, ctxGuild); //Сменить ник
                }
            }
        }

        private async Task UpdateUser(DiscordMember member, string newDisplayName, DiscordGuild ctxGuild)
        {
            try
            {
                await member.ModifyAsync(newDisplayName,
                    reason:
                    $"{member.Username} ({member.Id}) got under server cleaning nicknames. New nickname {newDisplayName}.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var logRecord = new
            {
                botEvent = "Server cleaning display names.",
                newDisplayName = newDisplayName,
                oldDisplayName = member.DisplayName,
                userDiscriminator = member.Discriminator,
                guild = ctxGuild.Name,
                userId = member.Id,
                guildId = ctxGuild.Id
            };
            var tmp = JsonConvert.SerializeObject(logRecord);
            Logger.SaveString(tmp);
        }

        /*
        [Command("search"), Description("Ищет на вики.")]
        public async Task Poll(CommandContext ctx, [Description("Что искать.")] string request)
        {
            //await ctx.RespondAsync(ctx.Message.Content);
            //await ctx.RespondAsync(request);
            //await ctx.RespondAsync(ctx.RawArgumentString);


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            var win11251 = Encoding.GetEncoding(1252);


            string uriString = "http://www.google.com/search";
            string requestString = ctx.RawArgumentString;
            WebClient webClient = new WebClient();

            //webClient.Encoding = win1251;
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("q", requestString);
            nameValueCollection.Add("as_sitesearch", "http://ru.warframe.wikia.com/");
            nameValueCollection.Add("start", "0");
            nameValueCollection.Add("num", "10");
            //nameValueCollection.Add("output", "xml_no_dtd");
            //http://www.google.com/search?start=0&num=10&q =где падает реактор орокин&as_sitesearch=http://ru.warframe.wikia.com/&output=xml_no_dtd&client=google-csbe
            //http://www.google.com/search?start=0&num=10&q=где падает реактор орокин&as_sitesearch=http://ru.warframe.wikia.com/&client=google-csbe&output=xml_no_dtd
            //    start = 0
            //& num = 10
            //            & q = red + sox
            //                      & cr = countryCA
            //                                 & client = google - csbe
            //                                                & output = xml_no_dtd

            //nameValueCollection.Add("ie", "UTF-8");
            webClient.QueryString.Add(nameValueCollection);
            string result = webClient.DownloadString(uriString);
            //await ctx.RespondAsync(result.Remove(1999));


            String CurrentPath = Directory.GetCurrentDirectory();
            String PathWithName = CurrentPath + "\\textReq1.html";
            StreamWriter w = new StreamWriter(PathWithName, false, win1251);

            w.Write(result);
            w.Close();


            //string result = Win1251ToUTF8(webClient.DownloadString(uriString));

            string pattern =
                "<h3 class=\"r\"><a href=\".*\">(.*)<\\/a><\\/h3><div class=\"s\"><div class=\"kv\" style=\"margin-bottom:2px\"><cite>(.*)<\\/b><\\/cite>";
            Regex regex = new Regex(pattern);
            Console.WriteLine("Запрос:" + requestString + "\n");

            if (regex.IsMatch(result))
            {
                await ctx.RespondAsync("Чет есть.\n" + requestString + "\n" + regex.Matches(result).Count);
            }
            else
            {
                await ctx.RespondAsync("Ничего.\n" + requestString + "\n" + regex.Matches(result).Count);
            }

            Console.WriteLine(pattern);
            foreach (Match match in regex.Matches(result))
            {
                string title = match.Groups[1].Value;
                string link = match.Groups[2].Value;

                title = deleteB(title);
                link = deleteB(link);
                Console.WriteLine("->" + title + "\n" + link);
            }

            var embed = new DiscordEmbedBuilder
            {
                //Url = 
                //Color = em.Message.Author is DiscordMember m ? m.Color : new DiscordColor(0xFF00FF),
                ////Description = em.Message.Content,
                //Author = new DiscordEmbedBuilder.EmbedAuthor
                //{
                //    Name = em.Message.Author is DiscordMember mx ? mx.DisplayName : em.Message.Author.Username,
                //    IconUrl = em.Message.Author.AvatarUrl
                //}
            };

            foreach (Match match in regex.Matches(result))
            {
                string title = match.Groups[1].Value;
                string link = match.Groups[2].Value;

                title = deleteB(title);
                link = deleteB(link);
                embed.AddField(title, link);
            }


            await ctx.RespondAsync(embed: embed);
        }
*/
        private static string deleteB(string source)
        {
            source = source.Replace("<b>", "");
            source = source.Replace("</b>", "");
            return source;
        }


        [Command("users"), Description("Лист, установленных ников")]
        public async Task ListOfUsers(CommandContext ctx
            //,[Optional, Description("Пользователь, которому надо назначить ник..")]
            //DiscordMember member
        )
        {
            await ctx.TriggerTypingAsync();

            if (ctx.Message.MentionedUsers.Count == 0)
            {
                bool first = true;

                List<AssignedNickname> users = AssignedNicknames.GetAllUsersByGuildId(ctx.Guild.Id.ToString());

                if (users.Count == 0)
                {
                    await ctx.RespondAsync("Пользователей не внесено.");
                }
                else
                {
                    string respond = "";

                    foreach (var VARIABLE in users)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            respond += "\n";
                        }

                        respond = VARIABLE.ToString() + "\n" + respond;
                    }

                    // first retrieve the interactivity module from the client
                    var interactivity = ctx.Client.GetInteractivityModule();
                    var respondPages = interactivity.GeneratePagesInEmbeds(respond);

                    // send the paginator
                    await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, respondPages,
                        TimeSpan.FromMinutes(5),
                        TimeoutBehaviour.Delete);

                    //ctx.RespondAsync(respond);
                }
            }
            else
            {
                var member = ctx.Message.MentionedUsers[0];
                AssignedNickname user =
                    AssignedNicknames.GetUserByIdAndGuildId(member.Id.ToString(), ctx.Guild.Id.ToString());
                if (user == null)
                {
                    await ctx.RespondAsync("Пользователь не найден в базе.");
                }
                else
                {
                    await ctx.RespondAsync(user.ToString());
                }
            }
        }

        [Command("sn"), Description("Назначает ник человеку, и следит за его сохранением.")]
        public async Task SetNickname(CommandContext ctx,
            [Required, Description("Пользователь, которому надо назначить ник..")]
            DiscordMember member,
            [Required, RemainingText, Description("Назначаемый ник.")]
            string newNickname)
        {
            await ctx.TriggerTypingAsync();

            try
            {
                string guildName = ctx.Guild.Name;
                string guildId = ctx.Guild.Id.ToString();

                var issuer = ctx.Member;

                var targetId = ctx.Message.MentionedUsers[0].Id;
                var targetMember = ctx.Guild.GetMemberAsync(targetId).Result;

                bool haveRights = false;

                if (MemberExtensions.CanInteract(issuer, targetMember))
                {
                    haveRights = true;
                    //await ctx.RespondAsync("У тебя хватает прав.");
                }
                else
                {
                    await ctx.RespondAsync("У тебя не хватает прав.");
                }

                if (haveRights)
                {
                    await member.ModifyAsync(newNickname, reason: $"Changed by {ctx.User.Username} ({ctx.User.Id}).");

                    try
                    {
                        DateTime date = DateTime.Now;
                        //Console.WriteLine(date1);

                        string orderer = ctx.User.Username + "#" + ctx.User.Discriminator;

                        var embed = new DiscordEmbedBuilder
                        {
                            Title = "Установка ника",
                            Description = "Инициатор " + orderer
                        };

                        string targetUser = ctx.Message.MentionedUsers[0].Username + "#" +
                                            ctx.Message.MentionedUsers[0].Discriminator;

                        string targetUserId = ctx.Message.MentionedUsers[0].Id.ToString();
                        string dateOfApplying = date.ToString(CultureInfo.InvariantCulture);

                        embed.AddField("Дата установки: ", dateOfApplying);

                        embed.AddField("Устанавливаемый ник: ", newNickname);
                        embed.AddField("Цель: ", targetUser);
                        embed.AddField("ID цели: ", targetUserId);


                        await ctx.RespondAsync(embed: embed);

                        AssignedNicknames.AddUser(orderer, targetUser, targetUserId, dateOfApplying, newNickname,
                            guildName, guildId);

                        for (int i = 0; i < ctx.Guild.Roles.Count; i++)
                        {
                            if (ctx.Guild.Roles[i].Name == "Человек Неразумный")
                            {
                                await ctx.Member.GrantRoleAsync(ctx.Guild.Roles[i],
                                    "Роль снята при использовании команды.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //e = e;
                        if (e.Message == "Unauthorized: 403")
                        {
                            await ctx.RespondAsync(ctx.User.Mention + " у меня не хватает прав.");
                        }
                        else
                        {
                            if (e.HResult == -2147467261)
                            {
                                await ctx.RespondAsync(ctx.User.Mention +
                                                       " ты не указал новый ник. Команду нужно использовать в формате: %команда% %упоминание пользователя% %ник%");
                            }
                            else
                            {
                                await ctx.RespondAsync("Что-то пошло не так. И это явно не было запланировано.\n" +
                                                       e.Message);
                            }
                        }

                        if (e.Message == "Value cannot be null.\\r\\nParameter name: value")
                        {
                            await ctx.RespondAsync(ctx.User.Mention + " у меня не хватает прав.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized: 403")
                {
                    await ctx.RespondAsync(ctx.User.Mention + " у меня не хватает прав.");
                }
                else
                {
                    await ctx.RespondAsync(ctx.User.Mention +
                                           ", что-то пошло не так на этапе проверки прав, и это явно не было запланировано.\n" +
                                           e.Message);
                }
            }
        }

        [Command("dn"), Description("Удаляет упомянутого пользователя, из списка присвоенных ников.")]
        public async Task DeleteAssignedNickname(CommandContext ctx
            //,[Optional, Description("Пользователь, которому надо назначить ник..")]
            //DiscordMember member
        )
        {
            await ctx.TriggerTypingAsync();
            if (ctx.Message.MentionedUsers.Count != 0)
            {
                try
                {
                    var issuer = ctx.Member;

                    var targetId = ctx.Message.MentionedUsers[0].Id;
                    var targetMember = ctx.Guild.GetMemberAsync(targetId).Result;

                    bool haveRights = false;

                    if (MemberExtensions.CanInteract(issuer, targetMember))
                    {
                        haveRights = true;
                        //await ctx.RespondAsync("У тебя хватает прав.");
                    }
                    else
                    {
                        //haveRights = false;
                        await ctx.RespondAsync("У тебя не хватает прав.");
                    }

                    if (haveRights)
                    {
                        if (AssignedNicknames.DeleteUserByIdAndGuildId(targetId.ToString(), ctx.Guild.Id.ToString()))
                        {
                            await ctx.RespondAsync("Успешно.");

                            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
                            {
                                if (ctx.Guild.Roles[i].Name == "Человек Неразумный")
                                {
                                    await ctx.Member.RevokeRoleAsync(ctx.Guild.Roles[i],
                                        "Роль снята при использовании команды.");
                                }
                            }
                        }
                        else
                        {
                            await ctx.RespondAsync("Возникла ошибка.");
                        }
                    }
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync(ctx.User.Mention +
                                           ", что-то пошло не так на этапе проверки прав, и это явно не было запланировано.");

                    //// oh no, something failed, let the invoker now
                    //var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                    //await ctx.RespondAsync(emoji);
                }
            }
        }
    }
}