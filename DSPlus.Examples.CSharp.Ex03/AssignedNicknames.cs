using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DSPlus.Examples
{
    static class AssignedNicknames
    {
        static List<AssignedNickname> assignedNicknames;

        static string path = "files/AssignedNicknames.json";

        static AssignedNicknames()
        {
            assignedNicknames = new List<AssignedNickname>();
            Load();
        }

        public static async void Load()
        {
            var json1 = "";
            using (var fs = File.OpenRead("files/AssignedNicknames.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json1 = await sr.ReadToEndAsync();

            var nicknames = JsonConvert.DeserializeObject<List<AssignedNickname>>(json1);
            SetUsersList(nicknames);
            Console.WriteLine("AssignedNicknames loaded");
        }

        public static void PrintAllOk()
        {
            Console.WriteLine("AllOk");
        }

        public static void SetUsersList(List<AssignedNickname> users)
        {
            assignedNicknames = users;
        }


        public static AssignedNickname GetUserByIdAndGuildId(string userId, string guildId)
        {
            for (int i = 0; i < assignedNicknames.Count; i++)
            {
                if (assignedNicknames[i].TargetUserId == userId && assignedNicknames[i].GuildId == guildId)
                {
                    return assignedNicknames[i];
                }
            }

            return null;
        }

        public static List<AssignedNickname> GetAllUsersByGuildId(string guildId)
        {
            List<AssignedNickname> users = new List<AssignedNickname>();
            for (int i = 0; i < assignedNicknames.Count; i++)
            {
                if (assignedNicknames[i].GuildId == guildId)
                {
                    users.Add(assignedNicknames[i]);
                }
            }

            return users;
        }

        public static bool DeleteUserByIdAndGuildId(string userId, string guildId)
        {
            /*
                for (int i = 0; i < e.Guild.Roles.Count; i++)
                        {
                            if (e.Guild.Roles[i].Name == "Человек Неразумный")
                            {
                                e.Member.GrantRoleAsync(e.Guild.Roles[i], "Сбросил себе роль, перезайдя.");
                            }
                        }
             */
            for (int i = 0; i < assignedNicknames.Count; i++)
            {
                if (assignedNicknames[i].TargetUserId == userId && assignedNicknames[i].GuildId == guildId)
                {
                    assignedNicknames.RemoveAt(i);
                    return true;
                }
            }

            SaveToJson();

            return false;
        }


        public static void AddUser(string orderer, string targetUser, string targetUserId, string dateOfApplying,
            string newNickname, string guildName, string guildId)
        {
            bool alradyExist = false;

            for (int i = 0; i < assignedNicknames.Count; i++)
            {
                if (assignedNicknames[i].TargetUserId == targetUserId && assignedNicknames[i].GuildId == guildId)
                {
                    alradyExist = true;
                    AssignedNickname newRecord = new AssignedNickname(orderer, targetUser, targetUserId, dateOfApplying,
                        newNickname, guildName, guildId);
                    assignedNicknames[i] = newRecord;
                }
            }

            if (!alradyExist)
                assignedNicknames.Add(new AssignedNickname(orderer, targetUser, targetUserId, dateOfApplying,
                    newNickname, guildName, guildId));

            SaveToJson();
        }

        public static void SaveToJson()
        {
            var json = JsonConvert.SerializeObject(assignedNicknames, Formatting.Indented);
            var writer = new StreamWriter(path, false);
            writer.Write(json);
            writer.Flush();
            writer.Dispose();
        }
    }

    public class AssignedNickname
    {
        public string Orderer;
        public string TargetUser;
        public string TargetUserId;
        public string DateOfApplying;
        public string NewNickname;
        public string GuildName;
        public string GuildId;

        public AssignedNickname(string orderer, string targetUser, string targetUserId, string dateOfApplying,
            string newNickname, string guildName, string guildId)
        {
            Orderer = orderer;
            TargetUser = targetUser;
            TargetUserId = targetUserId;
            DateOfApplying = dateOfApplying;
            NewNickname = newNickname;
            GuildName = guildName;
            GuildId = guildId;
        }

        public override string ToString()
        {
            return "Инициатор : " + Orderer + " |  Целевой пользователь: " + TargetUser + " |  Дата применения: " +
                   DateOfApplying + " | Ник: " + NewNickname;
        }
    }
}