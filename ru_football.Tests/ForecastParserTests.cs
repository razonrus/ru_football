using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Xunit;

namespace ru_football.Tests
{
    public class ForecastParserTests
    {
        [Fact]
        public void TestDvoetochie()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 0:1<br>2. 1:0<br>3. 1:1<br>4. 0:1<br>5. 1:1<br>6. 2:1<br>7. 0:1<br>8. 20:2", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(0, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(1, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(3, forecasts[2].Number);
            Assert.Equal(1, forecasts[2].OwnersGoals);
            Assert.Equal(1, forecasts[2].GuestsGoals);

            Assert.Equal(4, forecasts[3].Number);
            Assert.Equal(0, forecasts[3].OwnersGoals);
            Assert.Equal(1, forecasts[3].GuestsGoals);

            Assert.Equal(5, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(1, forecasts[4].GuestsGoals);

            Assert.Equal(6, forecasts[5].Number);
            Assert.Equal(2, forecasts[5].OwnersGoals);
            Assert.Equal(1, forecasts[5].GuestsGoals);

            Assert.Equal(7, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(1, forecasts[6].GuestsGoals);

            Assert.Equal(8, forecasts[7].Number);
            Assert.Equal(20, forecasts[7].OwnersGoals);
            Assert.Equal(2, forecasts[7].GuestsGoals);
        }

        [Fact]
        public void TestSix()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 0:1<br>2. 160<br>", new Ljuser()).ToList();

            Assert.Equal(2, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(0, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(1, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);
        }

        [Fact]
        public void TestOne()
        {
            List<Forecast> forecasts = ForecastParser.Parse("<div class=\"talk-comment-box\">15. 1:1<p style=\"margin: 0.7em 0 0.2em 0\"><font size=\"-2\">(<a href=\"http://ru-football.livejournal.com/7200790.html?replyto=440510742\" onclick=\"return QuickReply.reply('440510742',1720745,'')\" rel=\"nofollow\">Ответить</a>) </font></p><div id=\"ljqrt440510742\" name=\"ljqrt440510742\"></div></div>", new Ljuser()).ToList();

            Assert.Equal(1, forecasts.Count);

            Assert.Equal(15, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);
        }
        
        [Fact]
        public void Testbr()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 1:1<br />2. 1:0<br />3. 2:1<br />4. 1:2<br />5. 1:2<br />6. 3:0<br />7. 0:1<br />8. 1:1<p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=423139141'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(1, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(3, forecasts[2].Number);
            Assert.Equal(2, forecasts[2].OwnersGoals);
            Assert.Equal(1, forecasts[2].GuestsGoals);

            Assert.Equal(4, forecasts[3].Number);
            Assert.Equal(1, forecasts[3].OwnersGoals);
            Assert.Equal(2, forecasts[3].GuestsGoals);

            Assert.Equal(5, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(2, forecasts[4].GuestsGoals);

            Assert.Equal(6, forecasts[5].Number);
            Assert.Equal(3, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(7, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(1, forecasts[6].GuestsGoals);
            Assert.Equal(8, forecasts[7].Number);
            Assert.Equal(1, forecasts[7].OwnersGoals);
            Assert.Equal(1, forecasts[7].GuestsGoals);
        }
        [Fact]
        public void TestVosklZnak()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 1:1<br />2. 1:0<br />3. 2:1<br />4. 1:2<br />5. 1:2<br />6. 3:0<br />7. 0:!<br />8. 1:1<p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=423139141'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(1, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(3, forecasts[2].Number);
            Assert.Equal(2, forecasts[2].OwnersGoals);
            Assert.Equal(1, forecasts[2].GuestsGoals);

            Assert.Equal(4, forecasts[3].Number);
            Assert.Equal(1, forecasts[3].OwnersGoals);
            Assert.Equal(2, forecasts[3].GuestsGoals);

            Assert.Equal(5, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(2, forecasts[4].GuestsGoals);

            Assert.Equal(6, forecasts[5].Number);
            Assert.Equal(3, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(7, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(1, forecasts[6].GuestsGoals);
            Assert.Equal(8, forecasts[7].Number);
            Assert.Equal(1, forecasts[7].OwnersGoals);
            Assert.Equal(1, forecasts[7].GuestsGoals);
        }
        [Fact]
        public void Testhtml()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 1:0<br />2. 0:1<br />3. 1:1<br />4. 0:2<br />5. 0:0<br />6. 2:0<br />7. 0:1<br />8. 1:0<br /><p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=423164229'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(0, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(0, forecasts[1].OwnersGoals);
            Assert.Equal(1, forecasts[1].GuestsGoals);

            Assert.Equal(3, forecasts[2].Number);
            Assert.Equal(1, forecasts[2].OwnersGoals);
            Assert.Equal(1, forecasts[2].GuestsGoals);

            Assert.Equal(4, forecasts[3].Number);
            Assert.Equal(0, forecasts[3].OwnersGoals);
            Assert.Equal(2, forecasts[3].GuestsGoals);

            Assert.Equal(5, forecasts[4].Number);
            Assert.Equal(0, forecasts[4].OwnersGoals);
            Assert.Equal(0, forecasts[4].GuestsGoals);

            Assert.Equal(6, forecasts[5].Number);
            Assert.Equal(2, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(7, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(1, forecasts[6].GuestsGoals);

            Assert.Equal(8, forecasts[7].Number);
            Assert.Equal(1, forecasts[7].OwnersGoals);
            Assert.Equal(0, forecasts[7].GuestsGoals);

        }
        [Fact]
        public void TestFool()
        {
            List<Forecast> forecasts = ForecastParser.Parse(@"19 марта, суббота<br>9. Динамо : Ростов 1:1<br>10. Краснодар : Спартак-Нч 0:0<br><br>20 марта, воскресенье<br>11. Амкар : Локомотив 1:0<br>12. Томь : Кубань 0:0<br>13. Рубин : Терек 2:1<br><br>21 марта, понедельник<br>14. Зенит : Анжи 2:0<br>15. Кр. Советов : ЦСКА 1:3<br>16. Спартак : Волга 2:1<p style=""margin: 0.7em 0 0.2em 0""><font size=""-2"">(<a href=""http://www.livejournal.com/talkscreen.bml?mode=unscreen&amp;journal=ru_football&amp;talkid=425504278"">Раскрыть для ответов</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(9, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(10, forecasts[1].Number);
            Assert.Equal(0, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(11, forecasts[2].Number);
            Assert.Equal(1, forecasts[2].OwnersGoals);
            Assert.Equal(0, forecasts[2].GuestsGoals);

            Assert.Equal(12, forecasts[3].Number);
            Assert.Equal(0, forecasts[3].OwnersGoals);
            Assert.Equal(0, forecasts[3].GuestsGoals);

            Assert.Equal(13, forecasts[4].Number);
            Assert.Equal(2, forecasts[4].OwnersGoals);
            Assert.Equal(1, forecasts[4].GuestsGoals);

            Assert.Equal(14, forecasts[5].Number);
            Assert.Equal(2, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(15, forecasts[6].Number);
            Assert.Equal(1, forecasts[6].OwnersGoals);
            Assert.Equal(3, forecasts[6].GuestsGoals);

            Assert.Equal(16, forecasts[7].Number);
            Assert.Equal(2, forecasts[7].OwnersGoals);
            Assert.Equal(1, forecasts[7].GuestsGoals);

        }
        [Fact]
        public void TestTire()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 0-1<br>2. 1-0<br>3. 1 - 1<br>4. 0    -1<br>5. 1-    1<br>6. 2–1<br>7. 0 – 1<br>8. 20– 2", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(0, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(1, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(3, forecasts[2].Number);
            Assert.Equal(1, forecasts[2].OwnersGoals);
            Assert.Equal(1, forecasts[2].GuestsGoals);

            Assert.Equal(4, forecasts[3].Number);
            Assert.Equal(0, forecasts[3].OwnersGoals);
            Assert.Equal(1, forecasts[3].GuestsGoals);

            Assert.Equal(5, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(1, forecasts[4].GuestsGoals);

            Assert.Equal(6, forecasts[5].Number);
            Assert.Equal(2, forecasts[5].OwnersGoals);
            Assert.Equal(1, forecasts[5].GuestsGoals);

            Assert.Equal(7, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(1, forecasts[6].GuestsGoals);

            Assert.Equal(8, forecasts[7].Number);
            Assert.Equal(20, forecasts[7].OwnersGoals);
            Assert.Equal(2, forecasts[7].GuestsGoals);

        }
        [Fact]
        public void TestEndSpaces()
        {
            List<Forecast> forecasts = ForecastParser.Parse(@"33. 1:1  <br />34. 0:2  <br />35. 1:0  <br />36. 0:1  <br />37. 1:0  <br />38. 0:3  <br />39. 0:0  <br />40. 2:0 <br /><p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://ru-football.livejournal.com/7228809.html?replyto=431103625' rel='nofollow'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(8, forecasts.Count);

            Assert.Equal(33, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(34, forecasts[1].Number);
            Assert.Equal(0, forecasts[1].OwnersGoals);
            Assert.Equal(2, forecasts[1].GuestsGoals);
        }
        [Fact]
        public void TestEmptyForecast()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 1:1<br />2. 0:0<br />3. <br />4. 0:0<br />5. 1:1<br />6. 1:0<br />7. 0:0<br />8. 0:0<br /><p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=424263749'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(7, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(0, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(4, forecasts[2].Number);
            Assert.Equal(0, forecasts[2].OwnersGoals);
            Assert.Equal(0, forecasts[2].GuestsGoals);

            Assert.Equal(5, forecasts[3].Number);
            Assert.Equal(1, forecasts[3].OwnersGoals);
            Assert.Equal(1, forecasts[3].GuestsGoals);

            Assert.Equal(6, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(0, forecasts[4].GuestsGoals);

            Assert.Equal(7, forecasts[5].Number);
            Assert.Equal(0, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(8, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(0, forecasts[6].GuestsGoals);
        }
        [Fact]
        public void TestXForecast()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 1:1<br />2. 0:0<br />3. X:X<br />4. 0:0<br />5. 1:1<br />6. 1:0<br />7. 0:0<br />8. 0:0<br /><p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=424263749'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(7, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(0, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(4, forecasts[2].Number);
            Assert.Equal(0, forecasts[2].OwnersGoals);
            Assert.Equal(0, forecasts[2].GuestsGoals);

            Assert.Equal(5, forecasts[3].Number);
            Assert.Equal(1, forecasts[3].OwnersGoals);
            Assert.Equal(1, forecasts[3].GuestsGoals);

            Assert.Equal(6, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(0, forecasts[4].GuestsGoals);

            Assert.Equal(7, forecasts[5].Number);
            Assert.Equal(0, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(8, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(0, forecasts[6].GuestsGoals);
        }
        [Fact]
        public void TestSevenForecast()
        {
            List<Forecast> forecasts = ForecastParser.Parse("1. 1:1<br />2. 0:0<br />4. 0:0<br />5. 1:1<br />6. 1:0<br />7. 0:0<br />8. 0:0<br /><p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=424263749'>Reply to this</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(7, forecasts.Count);

            Assert.Equal(1, forecasts[0].Number);
            Assert.Equal(1, forecasts[0].OwnersGoals);
            Assert.Equal(1, forecasts[0].GuestsGoals);

            Assert.Equal(2, forecasts[1].Number);
            Assert.Equal(0, forecasts[1].OwnersGoals);
            Assert.Equal(0, forecasts[1].GuestsGoals);

            Assert.Equal(4, forecasts[2].Number);
            Assert.Equal(0, forecasts[2].OwnersGoals);
            Assert.Equal(0, forecasts[2].GuestsGoals);

            Assert.Equal(5, forecasts[3].Number);
            Assert.Equal(1, forecasts[3].OwnersGoals);
            Assert.Equal(1, forecasts[3].GuestsGoals);

            Assert.Equal(6, forecasts[4].Number);
            Assert.Equal(1, forecasts[4].OwnersGoals);
            Assert.Equal(0, forecasts[4].GuestsGoals);

            Assert.Equal(7, forecasts[5].Number);
            Assert.Equal(0, forecasts[5].OwnersGoals);
            Assert.Equal(0, forecasts[5].GuestsGoals);

            Assert.Equal(8, forecasts[6].Number);
            Assert.Equal(0, forecasts[6].OwnersGoals);
            Assert.Equal(0, forecasts[6].GuestsGoals);
        }
        
        
        [Fact]
        public void TestEmpty()
        {
            List<Forecast> forecasts = ForecastParser.Parse("так этот прогноз<p style='margin: 0.7em 0 0.2em 0'><font size='-2'>(<a href='http://community.livejournal.com/ru_football/7192133.html?replyto=425417029'>Reply to this</a>) (<a href='http://community.livejournal.com/ru_football/7192133.html?thread=424197701#t424197701'>Parent</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(0, forecasts.Count);

        }
        
        
        [Fact]
        public void TestCounter()
        {
            List<Forecast> forecasts = ForecastParser.Parse(@"Спасибо! <img src=""http://s03.flagcounter.com/count/YVK/bg=ffffff/txt=ffffff/border=ffffff/columns=1/maxflags=1/viewers=0/labels=0/.jpg"" width=""1"" height=""1""><img src=""http://2.s03.flagcounter.com/count/YVK/bg=ffffff/txt=ffffff/border=ffffff/columns=1/maxflags=1/viewers=0/labels=0/.jpg"" width=""1"" height=""1""><br><p style=""margin: 0.7em 0 0.2em 0""><font size=""-2"">(<a href=""http://community.livejournal.com/ru_football/7220672.html?replyto=429694400"" onclick=""return QuickReply.reply('429694400',1678493,'')"">Ответить</a>) </font></p>", new Ljuser()).ToList();

            Assert.Equal(0, forecasts.Count);

        }
    }
}
