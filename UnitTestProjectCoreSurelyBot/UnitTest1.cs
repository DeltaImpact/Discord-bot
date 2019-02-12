using System.Collections.Generic;
using DSPlus.Examples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProjectCoreSurelyBot
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataRow("/ex ", "ex")]
        [DataRow("/ex", "ex")]
        [DataRow("asdasd /ex ", "ex")]
        [DataRow("asd/exsad", null)]
        [DataRow("/ex /ex", "ex")]
        [DataRow("/derp", "Derp")]
        [DataRow("/exas ", null)]
        [DataRow("asdasd //ex", "ex")]
        [DataRow("/Ex ", "ex")]
        [DataRow("/Ex ", "ex")]
        [DataRow("/eX", "ex")]
        [DataRow("asdasd /EX", "ex")]
        [DataRow("asd/exsad", null)]
        [DataRow("/Ex /ex", "ex")]
        [DataRow("/dErp", "Derp")]
        [DataRow("/exas ", null)]
        [DataRow("asdasd //ex", "ex")]
        [DataRow("\\ex ", "ex")]
        [DataRow("\\ex", "ex")]
        [DataRow("asdasd \\ex", "ex")]
        [DataRow("asd\\exsad", null)]
        [DataRow("\\ex \\ex", "ex")]
        [DataRow("\\derp", "Derp")]
        [DataRow("\\exas ", null)]
        [DataRow("asdasd \\\\ex", "ex")]
        [DataRow(
            "D:\\Downloads\\DSharpPlus - Example - Bot - master\\DSPlus.Examples.CSharp.Ex03\\bin\\Release\\PublishOutput",
            null)]
        public void FindMemeInString(string before, string expected)
        {
            Meme actual = Memes.FindMeme(before);
            if (actual == null)
            {
                Assert.AreEqual(actual, expected);
            }
            else
            {
                Assert.AreEqual(actual.Name, expected);
            }
        }

        [TestMethod]
        [DataRow("[Даня]AnotherDanya", "Даня]AnotherDanya")]
        [DataRow("Lil Fubs69", "Lil Fubs69")]
        [DataRow("Dragon/Fire/Slap", "Dragon/Fire/Slap")]
        [DataRow("-RS-Mix_MaN -", "RS-Mix_MaN -")]
        [DataRow("akleks (Алекс) -met-", "akleks (Алекс) -met-")]
        [DataRow("ars_321", "ars_321")]
        [DataRow("ANOTHER ONE BITES THE DUST", "ANOTHER ONE BITES THE DUST")]
        [DataRow("10no", "no")]
        [DataRow("Dr. Feels Man", "Dr. Feels Man")]
        [DataRow("👗 ОчереднаяСтранность", "ОчереднаяСтранность")]
        [DataRow("℟₳Ꮥ₮Ꮍ96 (Бубер)", "Бубер)")]
        [DataRow("LilubsZ69", "LilubsZ69")]
        [DataRow("Jor60⧸⎠╱", "Jor60")]
        [DataRow("MiMaN[strange clantag]", "MiMaN[strange clantag]")]
        [DataRow("MiMaN [strange clantag]", "MiMaN [strange clantag]")]
        [DataRow("миша☺", "миша")]
        [DataRow("𝓓𝓻𝓪𝔀𝓖𝓪𝓶𝓮𝓟𝓵𝓪𝔂", "D.1010")]
        [DataRow("┘ム┌", "D.1010")]
        [DataRow("123123123", "D.1010")]
        [DataRow("血としての赤", "D.1010")]
        [DataRow("! ⛧ 𝔽𝕠𝕥𝕦𝕤𝟟𝟞 ⛧ ⚠", "D.1010")]
        [DataRow(" ̓ͨ͐҉̕͠͝", "D.1010")]
        [DataRow("Roker(Rus)", "Roker(Rus)")]
        [DataRow("йПё", "йПё")]
        [DataRow("Французский Фёдр", "Французский Фёдр")]
        public void TestMethodFixNickname(string before, string expected)
        {
            string actual = Program.FixNickname(before, "1010");
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        [DataRow(new string[]{"GͥOͣDͫ", "GOD"}, false)]
        public void TestMethodDiactricsEquality(string[] obj, bool compResult)
        {
            Assert.AreEqual(obj[0] == obj[1], compResult);
        }

        [TestMethod]
        [DataRow("𝓖𝓪𝓶𝓮𝓓𝓻𝓪𝔀𝓟𝓵𝓪𝔂", "")]
        [DataRow("┘ム┌", "")]
        [DataRow("123123123", "")]
        [DataRow("血としての赤", "")]
        [DataRow("! ⛧ 𝔽𝕠𝕥𝕦𝕤𝟟𝟞 ⛧ ⚠", "")]
        [DataRow(" ̓ͨ͐҉̕͠͝", "")]
        [DataRow("⎝⎝✧GͥOͣDͫ✧⎠⎠", "GOD")]
        public void TestMethodFindAcceptableParthOfNickname(string before, string expected)
        {
            string actual = Program.FindAcceptablePathOfNickname(before);
            Assert.AreEqual(actual, expected);
        }


    }
}