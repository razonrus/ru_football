using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using Domain;
using IndyCode.Infrastructure.Domain;
//using Chart = System.Web.Helpers.Chart;
using IQueryFactory = Domain.IQueryFactory;

namespace ru_football
{
    public interface ICalculator
    {
        string CalculateTourResult(string numberString);
        string SureThing();
        string CalculateTurnirTable(int lastMatchNumberOfPreviousTour);
        string CalculatePercentTurnirTable();
        string CalculateBeforeTour(string numbers);
        FileStreamResult Chart(int commandId);
    }

    public class Calculator : ICalculator
    {
        private readonly IQueryFactory queryFactory;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public Calculator(IQueryFactory queryFactory, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.queryFactory = queryFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        #region ICalculator Members

        public string CalculateTourResult(string numberString)
        {
            var results = new List<Forecast>();

            IEnumerable<int> numbers = numberString.Split(',').Select(x => int.Parse(x.Trim())).ToList();

            var tablo = "";
            IList<Match> matches;
            using (unitOfWorkFactory.Create())
            {
                matches = queryFactory.FindAll<Match>().Execute().ToList();

                foreach (int number in numbers)
                {
                    IEnumerable<Forecast> forecasts = queryFactory.GetForecastsByNumber(number).Execute();
                    Match match = matches.SingleOrDefault(x => x.Number == number);

                    if (match == null)
                        throw new NullReferenceException("Матч не сыгран");

                    tablo += string.Format("{0}. {1} — {2} {3}:{4}<br/>", match.Number, match.Owners.Name, match.Guests.Name, match.OwnersGoals, match.GuestsGoals);

                    foreach (Forecast forecast in forecasts)
                    {
                        SetScore(forecast, match);
                    }

                    results.AddRange(forecasts);
                }
            }

            string statistic = @"<u>Статистика тура:</u><br/><table border=""3""><tr align=""center"">";
            statistic += GetTd("Номер матча", 160, "left");
            foreach (int number in numbers)
            {
                statistic += GetTd(number);
            }
            statistic += @"</tr>";
            statistic += @"<tr align=""center"">";
            statistic = AddResults(statistic, numbers, matches);
            statistic += @"</tr>";
            statistic += AddStatisticRow(results, numbers, "Угаданных счетов<br/>(4 очка)",
                                        ScoreType.ScoreMatch);
            statistic += AddStatisticRow(results, numbers, "Угаданных разниц<br/>(2 очка)",
                                        ScoreType.Difference);
            statistic += AddStatisticRow(results, numbers, "Угаданных исходов<br/>(1 очко)", ScoreType.Result);
            statistic += AddStatisticRow(results, numbers, "Угадано всего<br/>(хотя бы 1 очко)",
                                        ScoreType.Result, ScoreType.ScoreMatch, ScoreType.Difference);
            statistic += @"</table>";
            statistic += "<br/>";

            IEnumerable<IGrouping<string, Forecast>> groupedByUser = results.GroupBy(x => x.Ljuser.Name).ToList();
            string best = GetTheBestFromTour(groupedByUser);
            
            var avg = string.Format("Среднее количество набранных очков: <b>{0}</b><br/><br/>", groupedByUser.Select(x => x.Sum(z => (int) z.Score)).Average().ToString("F"));

            string html = tablo + "<br/>" + statistic + best + avg + @"<table border=""3""><tr align=""center"">";
            html = AddResults(html, numbers, matches);

            html += GetTd("");
            html += @"</tr><tr align=""center"">" + GetTd("ljuser", null, "left");
            foreach (int number in numbers)
            {
                html += GetTd(number, 35);
            }
            html += GetTd("");
            html += @"</tr>";
            
            foreach (var userForecasts in groupedByUser.OrderBy(x => x.Key))
            {
                html += @"<tr align=""center"">";
                html += GetTd(LjUserTag(userForecasts.Key), null, "left");
                foreach (int number in numbers)
                {
                    Forecast forecast = userForecasts.SingleOrDefault(x => x.Number == number);
                    string content = forecast != null ? forecast.ToTag() : "<s>X:X</s>";
                    html += GetTd(content, null, null, null);
                }

                html += GetTd(userForecasts.Sum(x => (int) x.Score));

                html += @"</tr>";
            }
            html += @"</table>";

            return html;
        }

        public string SureThing()
        {
            var forecasts = new List<Forecast>();

            using (unitOfWorkFactory.Create())
            {
                IList<Match> matches = queryFactory.FindAll<Match>().Execute().ToList();

                foreach (Match match in matches)
                {
                    IEnumerable<Forecast> matchForecasts = queryFactory.GetForecastsByNumber(match.Number).Execute();

                    foreach (Forecast forecast in matchForecasts)
                    {
                        SetScore(forecast, match);
                    }

                    forecasts.AddRange(matchForecasts);
                }


                IEnumerable<IGrouping<int, Forecast>> bestScoreToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderByDescending(
                            x => x.Sum(f => (int) f.Score)/(decimal) x.Count())
                        .Take(10);

                IEnumerable<IGrouping<int, Forecast>> lowestScoreToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderBy(
                            x => x.Sum(f => (int) f.Score)/(decimal) x.Count())
                        .Take(20);


                string statistic = GetTableMatchStatistic(bestScoreToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/>";
                statistic += GetTableMatchStatistic(lowestScoreToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/>";


                IEnumerable<IGrouping<int, Forecast>> bestResultToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderByDescending(
                            x => x.Count(f => (int) f.Score > 0)/(decimal) x.Count())
                        .Take(10);

                IEnumerable<IGrouping<int, Forecast>> lowestResultToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderBy(
                            x => x.Count(f => (int) f.Score > 0)/(decimal) x.Count())
                        .Take(20);


                statistic += GetTableMatchByResultStatistic(bestResultToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/>";
                statistic += GetTableMatchByResultStatistic(lowestResultToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/>";


                IEnumerable<Command> commands = queryFactory.FindAll<Command>().Execute();

                statistic += @"<table border=""3""><tr align=""center"">";
                statistic += GetTd("Команда");
                statistic += GetTd("Угадано исходов в среднем за прогноз в матчах с её участием");
                statistic += GetTd("Набрано очков в среднем за прогноз в матчах с её участием");
                statistic += GetTd("Угадано исходов в среднем за прогноз в домашних матчах");
                statistic += GetTd("Угадано исходов в среднем за прогноз в гостевых матчах");
                statistic += @"</tr>";

                foreach (Command command in commands
                    .OrderByDescending(c =>
                                           {
                                               List<Forecast> commandForecasts =
                                                   forecasts.Where(
                                                       f =>
                                                       matches.Where(x => x.Owners == c || x.Guests == c).Select(
                                                           z => z.Number).Contains(f.Number)).ToList();
                                               return commandForecasts.Count(f => (int) f.Score > 0)/
                                                      (decimal) commandForecasts.Count();
                                           })
                    )
                {
                    statistic += @"<tr align=""center"">";
                    statistic += GetTd(command.Name);
                    List<Forecast> commandForecasts =
                        forecasts.Where(
                            f =>
                            matches.Where(x => x.Owners == command || x.Guests == command).Select(z => z.Number).
                                Contains(f.Number)).ToList();
                    List<Forecast> commandHomeForecasts =
                        forecasts.Where(
                            f => matches.Where(x => x.Owners == command).Select(z => z.Number).Contains(f.Number)).
                            ToList();
                    List<Forecast> commandGuestForecasts =
                        forecasts.Where(
                            f => matches.Where(x => x.Guests == command).Select(z => z.Number).Contains(f.Number)).
                            ToList();
                    statistic += GetTd(
                        (commandForecasts.Count(f => (int) f.Score > 0)/(decimal) commandForecasts.Count()).ToString(
                            "0.##")
                        );
                    statistic += GetTd(
                        (commandForecasts.Sum(f => (int) f.Score)/(decimal) commandForecasts.Count()).ToString("0.##")
                        );
                    statistic += GetTd(
                        (commandHomeForecasts.Count(f => (int) f.Score > 0)/(decimal) commandHomeForecasts.Count()).
                            ToString("0.##")
                        );
                    statistic += GetTd(
                        (commandGuestForecasts.Count(f => (int) f.Score > 0)/(decimal) commandGuestForecasts.Count()).
                            ToString("0.##")
                        );

                    statistic += @"</tr>";
                }
                statistic += @"</table>";


                return statistic;
            }
        }

        public FileStreamResult Chart(int commandId)
        {
            var forecasts = new List<Forecast>();

            var stForecasts = new List<Forecast>();
            var stMatches = new List<Match>();

            using (unitOfWorkFactory.Create())
            {
                IList<Match> matches = queryFactory.FindAll<Match>().Execute().ToList();
                var command = queryFactory.FindById<Command>(commandId).Execute();



                var commandMatches = matches.Where(x => 
                    x.Owners.Id == commandId 
                    || 
                    x.Guests.Id == commandId
                    ).ToList();
                foreach (Match match in commandMatches)
                {
                    IEnumerable<Forecast> matchForecasts = queryFactory.GetForecastsByNumber(match.Number).Execute().ToList();

                    foreach (Forecast forecast in matchForecasts)
                    {
                        SetScore(forecast, match);
                    }

                    forecasts.AddRange(matchForecasts);
                    
                    if(matchForecasts.GroupBy(x=>Math.Sign(x.GetDifference())).Any(x=>x.Count() / (double)matchForecasts.Count() >= 0.8))
                    {
                        stForecasts.AddRange(matchForecasts);
                        stMatches.Add(match);
                    }
                }
                
                var chart = new Chart
                        {
                            Width = 1400
                        };
                var area = new ChartArea
                               {
                                   AxisX = {Interval = 1}
                               };
                // configure your chart area (dimensions, etc) here.
                chart.ChartAreas.Add(area);

                // create and customize your data series.
                var series = new Series(command.Name);

                var com = command.Name[0] + ".";

//                var xValues = commandMatches.
                var xValues = stMatches.
                    Select(x => string.Format("{0} {1}:{2} {3}", (x.Owners.Id != commandId ? x.Owners.Name + "-" + com : com + "-" + x.Guests.Name), x.OwnersGoals, x.GuestsGoals, x.Date.Value.ToShortDateString()))
                    .ToList();
//                var yValues = forecasts.GroupBy(x => x.Number).Select(RightResultPart).Select(x => Math.Round(x*100)).ToList();
                var yValues = stForecasts.GroupBy(x => x.Number).Select(RightResultPart).Select(x => Math.Round(x * 100)).ToList();

                for (int i = 0; i < xValues.Count(); i++)
                {
                    series.Points.AddXY(xValues.ElementAt(i), yValues.ElementAt(i));                    
                }
                series.Font = new Font("Segoe UI", 8.0f, FontStyle.Bold);
                
                chart.Series.Add(series);

                var legend = new Legend();
                chart.Legends.Add(legend);
                
                var returnStream = new MemoryStream();
                chart.ImageType = ChartImageType.Png;
                chart.SaveImage(returnStream);
                returnStream.Position = 0;
                return new FileStreamResult(returnStream, "image/png");

            }
        }

        private static decimal RightResultPart(IEnumerable<Forecast> forecasts)
        {
            return forecasts.Count(f => (int) f.Score > 0)/(decimal) forecasts.Count();
        }

        public string CalculateBeforeTour(string numberString)
        {
            var results = new List<Forecast>();

            IEnumerable<int> numbers = numberString.Split(',')
                .Select(x => int.Parse(x.Trim()))
                .ToList();

            IList<Match> matches;
            using (unitOfWorkFactory.Create())
            {
                matches = queryFactory.FindAll<Match>().Execute().ToList();

                foreach (int number in numbers)
                {
                    IEnumerable<Forecast> forecasts = queryFactory.GetForecastsByNumber(number).Execute();

                    results.AddRange(forecasts);
                }


                string statistic = StatisticBeforeTour(results, numbers, "Прогнозы тура");


                statistic += "<br/>";

                IEnumerable<string> orderedLjusers = OrderedLjusers(0).Take(10).Select(x => x.Key.Name);
                statistic += StatisticBeforeTour(results.Where(x => orderedLjusers.Contains(x.Ljuser.Name)).ToList(),
                                                 numbers, @"Прогнозы топ-10");

                return statistic;
            }
        }

        public string CalculateTurnirTable(int lastMatchNumberOfPreviousTour)
        {
            string html = @"<table border=""3""><tr align=""center"">";
            html += GetTd("№", 30);
            html += GetTd("d", 50);
            html += GetTd("ljuser", 200, "left");
            html += GetTd("Матчей", 70);
            html += GetTd("Счет", 70);
            html += GetTd("Разница", 70);
            html += GetTd("Исход", 70);
            html += GetTd("Очки", 70);
            html += "</tr>";

            IEnumerable<IGrouping<Ljuser, Forecast>> orderedEnumerable = OrderedLjusers(lastMatchNumberOfPreviousTour);

            foreach (var userForecasts in orderedEnumerable)
            {
                html += "<tr align=\"center\">";
                html += GetTd(userForecasts.Key.Rank);
                html += GetTd(GetDifferenceRank(userForecasts));
                html += GetTd(string.Format("<lj user=\"{0}\">", userForecasts.Key.Name), null, "left");
                html += GetTd(userForecasts.Key.Forecasts.Count());
                html += GetTd(userForecasts.Key.Forecasts.Count(x => x.Score == ScoreType.ScoreMatch));
                html += GetTd(userForecasts.Key.Forecasts.Count(x => x.Score == ScoreType.Difference));
                html += GetTd(userForecasts.Key.Forecasts.Count(x => x.Score == ScoreType.Result));
                html += GetTd(userForecasts.Key.Scores);
                html += "</tr>";
            }

            html += "</table>";

            return html;
        }

        public string CalculatePercentTurnirTable()
        {
            string html = @"<table border=""3""><tr align=""center"">";
            html += GetTd("№", 30);
            html += GetTd("ljuser", 200, "left");
            html += GetTd("Матчей", 70);


            html += GetTd("Очки", 70);
            html += GetTd("Очков за матч", 70);
            html += "</tr>";

            IEnumerable<IGrouping<Ljuser, Forecast>> orderedEnumerable = OrderedByPercentLjusers();

            foreach (var userForecasts in orderedEnumerable)
            {
                html += "<tr align=\"center\">";
                html += GetTd(userForecasts.Key.Rank);
                html += GetTd(string.Format("<lj user=\"{0}\">", userForecasts.Key.Name), null, "left");
                html += GetTd(userForecasts.Key.Forecasts.Count());

                html += GetTd(userForecasts.Key.Scores);
                html += GetTd((userForecasts.Key.Scores/(decimal) userForecasts.Key.Forecasts.Count()).ToString("0.##"));
                html += "</tr>";
            }

            html += "</table>";

            return html;
        }

        #endregion

        private static string GetTableMatchStatistic(IEnumerable<IGrouping<int, Forecast>> bestScoreToCount,
                                                     IList<Match> matches)
        {
            string statistic = @"<table border=""3""><tr align=""center"">";
            statistic += GetTd("Номер");
            statistic += GetTd("Матч");
            statistic += GetTd("Дата");
            statistic += GetTd("Результат");
            statistic += GetTd("Кол-во прогнозов");
            statistic += GetTd("Кол-во набранных очков");
            statistic += GetTd("Среднее кол-во набранных очков");
            statistic += @"</tr>";

            foreach (var match in bestScoreToCount)
            {
                statistic += @"<tr align=""center"">";

                statistic += GetTd(matches.Single(x => x.Number == match.Key).Number);
                statistic += GetTd(matches.Single(x => x.Number == match.Key).Caption(), align: "left");
                statistic += GetTd(matches.Single(x => x.Number == match.Key).Date.Value.ToShortDateString());
                statistic += GetTd(matches.Single(x => x.Number == match.Key).ToString());
                statistic += GetTd(match.Count());
                statistic += GetTd(match.Sum(f => (int) f.Score));
                statistic += GetTd((match.Sum(f => (int) f.Score)/(decimal) match.Count()).ToString("0.##"));

                statistic += @"</tr>";
            }
            statistic += @"</table>";
            return statistic;
        }

        private static string GetTableMatchByResultStatistic(IEnumerable<IGrouping<int, Forecast>> bestScoreToCount,
                                                             IList<Match> matches)
        {
            string statistic = @"<table border=""3""><tr align=""center"">";
            statistic += GetTd("Номер");
            statistic += GetTd("Матч");
            statistic += GetTd("Дата");
            statistic += GetTd("Результат");
            statistic += GetTd("Кол-во прогнозов");
            statistic += GetTd("Кол-во угаданных исходов");
            statistic += GetTd("Среднее кол-во угаданных исходов");
            statistic += @"</tr>";

            foreach (var match in bestScoreToCount)
            {
                statistic += @"<tr align=""center"">";

                statistic += GetTd(matches.Single(x => x.Number == match.Key).Number);
                statistic += GetTd(matches.Single(x => x.Number == match.Key).Caption(), align: "left");
                statistic += GetTd(matches.Single(x => x.Number == match.Key).Date.Value.ToShortDateString());
                statistic += GetTd(matches.Single(x => x.Number == match.Key).ToString());
                statistic += GetTd(match.Count());
                statistic += GetTd(match.Count(f => (int) f.Score > 0));
                statistic += GetTd((match.Count(f => (int) f.Score > 0)/(decimal) match.Count()).ToString("0.##"));

                statistic += @"</tr>";
            }
            statistic += @"</table>";
            return statistic;
        }

        private static string StatisticBeforeTour(List<Forecast> results, IEnumerable<int> numbers, string caption)
        {
            string statistic = @"<u>" + caption + @":</u><br/><table border=""3""><tr align=""center"">";
            statistic += GetTd("Номер матча", 160, "left");
            foreach (int number in numbers)
            {
                statistic += GetTd(number);
            }
            statistic += @"</tr>";
            statistic = AddForecastStatisticRow(results, statistic, numbers, "Победа хозяев",
                                                x => x.OwnersGoals > x.GuestsGoals);
            statistic = AddForecastStatisticRow(results, statistic, numbers, "Ничья",
                                                x => x.OwnersGoals == x.GuestsGoals);
            statistic = AddForecastStatisticRow(results, statistic, numbers, "Победа гостей",
                                                x => x.OwnersGoals < x.GuestsGoals);
            statistic += @"</table>";
            return statistic;
        }

        private static string AddForecastStatisticRow(List<Forecast> results, string statistic, IEnumerable<int> numbers,
                                                      string label, Func<Forecast, bool> func)
        {
            statistic += @"<tr align=""center"">";
            statistic += GetTd(label, align: "left");
            foreach (int number in numbers)
            {
                decimal count = results.Count(x => x.Number == number && func(x));
                int allCount = results.Count(x => x.Number == number);
                decimal round = allCount != 0 ? Math.Round(count/allCount*100) : 0;
                statistic += GetTd(string.Format("{0}%", round), tag: null);
            }
            statistic += @"</tr>";
            return statistic;
        }

        private static string AddStatisticRow(IList<Forecast> resultsWithoutScore, IEnumerable<int> numbers, string label, params ScoreType[] scoreType)
        {
            var tr = @"<tr align=""center"">";
            tr += GetTd(label, 160, "left");
            foreach (int number in numbers)
            {
                decimal count = resultsWithoutScore.Count(x => x.Number == number && scoreType.Contains(x.Score));
                tr +=
                    GetTd(
                        string.Format("{0} ({1}%)", count,
                                      Math.Round(count/resultsWithoutScore.Count(x => x.Number == number)*100)),
                        tag: null);
            }
            tr += @"</tr>";
            return tr;
        }

        private string AddResults(string html, IEnumerable<int> numbers, IList<Match> matches)
        {
            html += GetTd("Счет", 160, "left");
            foreach (int number in numbers)
            {
                Match match = matches.SingleOrDefault(x => x.Number == number);

                if (match == null)
                    throw new NullReferenceException("Матч не сыгран");

                html += GetTd(match.ToString());
            }
            return html;
        }

        private static string GetTheBestFromTour(IEnumerable<IGrouping<string, Forecast>> groupedByUser)
        {
            IOrderedEnumerable<IGrouping<int, IGrouping<string, Forecast>>> orderedUsers = groupedByUser
                .GroupBy(z => z.Sum(x => (int) x.Score))
                .OrderByDescending(x => x.Key);
            string best = "<u>Лучшие в туре:</u><br/>";
            best += GetPlace(orderedUsers, 1, ",");
            best += GetPlace(orderedUsers, 2, ",");
            best += GetPlace(orderedUsers, 3, ".");
            best += "<u>Худшие в туре:</u><br/>";
            best += GetPlace(orderedUsers, orderedUsers.Count() - 1, ",");
            best += GetPlace(orderedUsers, orderedUsers.Count(), ".");
            best += "<br/>";
            return best;
        }

        private static string GetPlace(IEnumerable<IGrouping<int, IGrouping<string, Forecast>>> orderedUsers, int place,
                                       string endSymbol)
        {
//            if (place >= orderedUsers.Count())
//                return string.Empty;

            IGrouping<int, IGrouping<string, Forecast>> bestInTour = orderedUsers.ElementAt(place - 1);
            string aggregate = bestInTour
                .Aggregate("<b>" + place + "-е место</b> в туре ",
                           (current, q) => current + (LjUserTag(q.First().Ljuser.Name) + ", "));
            aggregate = string.Format("{0}: {1} очков{2}<br/>", aggregate.Trim(' ', ','), bestInTour.Key, endSymbol);

            return aggregate;
        }

        private IEnumerable<IGrouping<Ljuser, Forecast>> OrderedLjusers(int lastMatchNumberOfPreviousTour)
        {
            IEnumerable<Forecast> allForecasts;
            IEnumerable<Ljuser> users;
            using (unitOfWorkFactory.Create())
            {
                users = queryFactory.FindAll<Ljuser>().Execute().ToList();
                allForecasts = users.SelectMany(x => x.Forecasts).ToList();
                IEnumerable<Match> allMatches = queryFactory.FindAll<Match>().Execute().ToList();

                foreach (Forecast forecast in allForecasts)
                {
                    Match match = allMatches.SingleOrDefault(x => x.Number == forecast.Number);

                    if (match == null)
                        continue;

                    SetScore(forecast, match);
                }
            }
            foreach (var userForecasts in allForecasts.GroupBy(x => x.Ljuser))
            {
                userForecasts.Key.ScoresAfterPreviousTour =
                    userForecasts.Where(x => x.Number <= lastMatchNumberOfPreviousTour).Sum(x => (int) x.Score);
                userForecasts.Key.Scores = userForecasts.Sum(x => (int) x.Score);
            }
            int i = 1;
            foreach (
                Ljuser ljuser in
                    users.OrderBy(x => x.Forecasts.Count(z => z.Number <= lastMatchNumberOfPreviousTour)).
                        OrderByDescending(x => x.ScoresAfterPreviousTour))
            {
                if (ljuser.Forecasts.Count(z => z.Number <= lastMatchNumberOfPreviousTour) == 0)
                    ljuser.RankAfterPreviousTour = null;
                else
                    ljuser.RankAfterPreviousTour = i++;
            }
            i = 1;
            foreach (Ljuser ljuser in users.OrderBy(x => x.Forecasts.Count()).OrderByDescending(x => x.Scores))
            {
                ljuser.Rank = i++;
            }
            IOrderedEnumerable<IGrouping<Ljuser, Forecast>> orderedEnumerable =
                allForecasts.GroupBy(x => x.Ljuser).OrderBy(x => x.Key.Rank);
            return orderedEnumerable;
        }

        private IEnumerable<IGrouping<Ljuser, Forecast>> OrderedByPercentLjusers()
        {
            IEnumerable<Forecast> allForecasts;
            IEnumerable<Ljuser> users;
            using (unitOfWorkFactory.Create())
            {
                users = queryFactory.FindAll<Ljuser>().Execute().Where(x => x.Forecasts.Count() > 15).ToList();
                allForecasts = users.SelectMany(x => x.Forecasts).ToList();
                IEnumerable<Match> allMatches = queryFactory.FindAll<Match>().Execute().ToList();

                foreach (Forecast forecast in allForecasts)
                {
                    Match match = allMatches.SingleOrDefault(x => x.Number == forecast.Number);

                    if (match == null)
                        continue;

                    SetScore(forecast, match);
                }
            }

            foreach (var userForecasts in allForecasts.GroupBy(x => x.Ljuser))
            {
                userForecasts.Key.Scores = userForecasts.Sum(x => (int) x.Score);
            }
            int i = 1;
            foreach (Ljuser ljuser in users.OrderByDescending(x => x.Scores/(decimal) x.Forecasts.Count()))
            {
                ljuser.Rank = i++;
            }
            return allForecasts.GroupBy(x => x.Ljuser).OrderBy(x => x.Key.Rank);
        }

        private static string LjUserTag(string name)
        {
            return string.Format("<lj user=\"{0}\">", name);
        }

        private static string GetDifferenceRank(IGrouping<Ljuser, Forecast> userForecasts)
        {
            if (userForecasts.Key.RankAfterPreviousTour.HasValue)
            {
                int difference = userForecasts.Key.RankAfterPreviousTour.Value - userForecasts.Key.Rank;
                string stringDifference = difference.ToString();
                if (difference > 0)
                    stringDifference = "+" + stringDifference;
                return stringDifference;
            }
            return "-";
        }

        private static void SetScore(Forecast forecast, Match result)
        {
            if (result.OwnersGoals == forecast.OwnersGoals && result.GuestsGoals == forecast.GuestsGoals)
            {
                forecast.Score = ScoreType.ScoreMatch;
                return;
            }
            if (result.GetDifference() == forecast.GetDifference())
            {
                forecast.Score = ScoreType.Difference;
                return;
            }
            if (result.GetDifference()*forecast.GetDifference() > 0)
            {
                forecast.Score = ScoreType.Result;
            }
        }

        private static string GetTd(object content, int? width = null, string align = null, string tag = "b")
        {
            string attributes = (width.HasValue ? string.Format(@" width=""{0}""", width) : string.Empty) +
                                (string.IsNullOrEmpty(align) == false
                                     ? string.Format(@" align=""{0}""", align)
                                     : string.Empty);
            return string.Format(@"<td{0}>{1}{2}{3}</td>",
                                 attributes,
                                 string.IsNullOrEmpty(tag) == false ? string.Format("<{0}>", tag) : string.Empty,
                                 content,
                                 string.IsNullOrEmpty(tag) == false ? string.Format("</{0}>", tag) : string.Empty);
        }
    }
}