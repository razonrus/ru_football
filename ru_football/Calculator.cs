#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using Domain;
using IndyCode.Infrastructure.Domain;
using JetBrains.Annotations;
using IQueryFactory = Domain.IQueryFactory;
//using Chart = System.Web.Helpers.Chart;

#endregion

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
        string CalculateTourResult(IList<int> matchNumbers, IList<string> users = null);
        Dictionary<int, Dictionary<string, double>> CalculateForUser(string userName);
        Dictionary<int, int> CalculateTourProgress(string userName);
    }

    [UsedImplicitly]
    public class Calculator : ICalculator
    {
        public const string NameForAvg = "_avg___";
        private readonly IQueryFactory queryFactory;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public Calculator(IQueryFactory queryFactory, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.queryFactory = queryFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        private static string GetTableMatchStatistic(IEnumerable<IGrouping<int, Forecast>> bestScoreToCount,
            IList<Match> matches)
        {
            string statistic = @"<table border=""3""><tr align=""center"">";
            statistic += GetTd("�����");
            statistic += GetTd("����");
            statistic += GetTd("����");
            statistic += GetTd("���������");
            statistic += GetTd("���-�� ���������");
            statistic += GetTd("���-�� ��������� �����");
            statistic += GetTd("������� ���-�� ��������� �����");
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
                statistic += GetTd((match.Sum(f => (int) f.Score) / (decimal) match.Count()).ToString("0.##"));

                statistic += @"</tr>";
            }
            statistic += @"</table>";
            return statistic;
        }

        private static string GetTableMatchByResultStatistic(IEnumerable<IGrouping<int, Forecast>> bestScoreToCount,
            IList<Match> matches)
        {
            string statistic = @"<table border=""3""><tr align=""center"">";
            statistic += GetTd("�����");
            statistic += GetTd("����");
            statistic += GetTd("����");
            statistic += GetTd("���������");
            statistic += GetTd("���-�� ���������");
            statistic += GetTd("���-�� ��������� �������");
            statistic += GetTd("������� ���-�� ��������� �������");
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
                statistic += GetTd((match.Count(f => (int) f.Score > 0) / (decimal) match.Count()).ToString("0.##"));

                statistic += @"</tr>";
            }
            statistic += @"</table>";
            return statistic;
        }

        private static string StatisticBeforeTour(List<Forecast> results, IList<int> numbers, string caption)
        {
            string statistic = @"<u>" + caption + @":</u><br/><table border=""3""><tr align=""center"">";
            statistic += GetTd("����� �����", 160, "left");
            foreach (int number in numbers)
            {
                statistic += GetTd(number);
            }
            statistic += @"</tr>";
            statistic = AddForecastStatisticRow(results, statistic, numbers, "������ ������",
                x => x.OwnersGoals > x.GuestsGoals);
            statistic = AddForecastStatisticRow(results, statistic, numbers, "�����",
                x => x.OwnersGoals == x.GuestsGoals);
            statistic = AddForecastStatisticRow(results, statistic, numbers, "������ ������",
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
                decimal round = allCount != 0 ? Math.Round(count / allCount * 100) : 0;
                statistic += GetTd($"{round}%", tag: null);
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
                tr += GetTd($"{count} ({Math.Round(count / resultsWithoutScore.Count(x => x.Number == number) * 100)}%)", tag: null);
            }
            tr += @"</tr>";
            return tr;
        }

        private string AddResults(string html, IEnumerable<int> numbers, IList<Match> matches)
        {
            html += GetTd("����", 160, "left");
            foreach (int number in numbers)
            {
                Match match = matches.SingleOrDefault(x => x.Number == number);

                if (match == null)
                    throw new NullReferenceException("���� �� ������");

                html += GetTd(match.ToString());
            }
            return html;
        }

        private static string GetTheBestFromTour(IEnumerable<IGrouping<string, Forecast>> groupedByUser)
        {
            var orderedUsers = groupedByUser
                .GroupBy(z => z.Sum(x => (int) x.Score))
                .OrderByDescending(x => x.Key)
                .ToList();
            string best = "<u>������ � ����:</u><br/>";
            best += GetPlace(orderedUsers, 1, ",");
            best += GetPlace(orderedUsers, 2, ",");
            best += GetPlace(orderedUsers, 3, ".");
            best += "<u>������ � ����:</u><br/>";
            best += GetPlace(orderedUsers, orderedUsers.Count - 1, ",");
            best += GetPlace(orderedUsers, orderedUsers.Count, ".");
            best += "<br/>";
            return best;
        }

        private static string GetPlace(IList<IGrouping<int, IGrouping<string, Forecast>>> orderedUsers, int place,
            string endSymbol)
        {
            if (place > orderedUsers.Count)
                return string.Empty;

            IGrouping<int, IGrouping<string, Forecast>> bestInTour = orderedUsers.ElementAt(place - 1);
            string aggregate = bestInTour
                .Aggregate("<b>" + place + "-� �����</b> � ���� ",
                    (current, q) => current + LjUserTag(q.First().Ljuser.Name) + ", ");

            aggregate = $"{aggregate.Trim(' ', ',')}: {bestInTour.Key} {ScoreText(bestInTour.Key)}{endSymbol}<br/>";

            return aggregate;
        }

        private static string ScoreText(int score)
        {
            switch (score)
            {
                case 1:
                case 21:
                    return "����";
                case 2:
                case 22:
                case 3:
                case 23:
                case 4:
                case 24:
                    return "����";
                default:
                    return "�����";
            }
        }

        private IEnumerable<IGrouping<Ljuser, Forecast>> OrderedLjusers(int lastMatchNumberOfPreviousTour, IList<Forecast> allForecasts, IList<Ljuser> users)
        {
            foreach (var userForecasts in allForecasts.GroupBy(x => x.Ljuser))
            {
                userForecasts.Key.ScoresAfterPreviousTour =
                    userForecasts.Where(x => x.Number <= lastMatchNumberOfPreviousTour).Sum(x => (int) x.Score);
                userForecasts.Key.Scores = userForecasts.Sum(x => (int) x.Score);
            }
            int i = 1;
            foreach (
                Ljuser ljuser in
                users.OrderBy(x => x.Forecasts.Count(z => z.Number <= lastMatchNumberOfPreviousTour)).OrderByDescending(x => x.ScoresAfterPreviousTour))
            {
                if (ljuser.Forecasts.Count(z => z.Number <= lastMatchNumberOfPreviousTour) == 0)
                    ljuser.RankAfterPreviousTour = null;
                else
                    ljuser.RankAfterPreviousTour = i++;
            }
            i = 1;
            foreach (Ljuser ljuser in users
                .OrderByDescending(x => x.Scores)
                .ThenBy(x => x.Forecasts.Count())
                .ThenByDescending(x => x.Forecasts.Count(f => f.Score == ScoreType.ScoreMatch))
                .ThenByDescending(x => x.Forecasts.Count(f => f.Score == ScoreType.Difference))
            )
            {
                ljuser.Rank = i++;
            }
            IOrderedEnumerable<IGrouping<Ljuser, Forecast>> orderedEnumerable =
                allForecasts.GroupBy(x => x.Ljuser).OrderBy(x => x.Key.Rank);
            return orderedEnumerable;
        }

        private Data LoadData()
        {
            using (unitOfWorkFactory.Create())
            {
                var users = queryFactory.FindAll<Ljuser>().Execute().ToList();
                var allForecasts = users.SelectMany(x => x.Forecasts).ToList();
                IEnumerable<Match> allMatches = queryFactory.FindAll<Match>().Execute().ToList();

                foreach (Forecast forecast in allForecasts)
                {
                    Match match = allMatches.SingleOrDefault(x => x.Number == forecast.Number);

                    if (match == null || (match.OwnersGoals == null && match.GuestsGoals == null))
                        continue;

                    SetScore(forecast, match);
                }
                return new Data(allForecasts, users, allMatches);
            }
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
            foreach (Ljuser ljuser in users.OrderByDescending(x => x.Scores / (decimal) x.Forecasts.Count()))
            {
                ljuser.Rank = i++;
            }
            return allForecasts.GroupBy(x => x.Ljuser).OrderBy(x => x.Key.Rank);
        }

        private static string LjUserTag(string name)
        {
            return $"<lj user=\"{name}\">";
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
            if (result.GetDifference() * forecast.GetDifference() > 0)
            {
                forecast.Score = ScoreType.Result;
            }
        }

        private static string GetTd(object content, int? width = null, string align = null, string tag = "b")
        {
            string attributes = (width.HasValue ? $@" width=""{width}""" : string.Empty) +
                                (string.IsNullOrEmpty(align) == false
                                    ? $@" align=""{align}"""
                                    : string.Empty);
            return $@"<td{attributes}>{(string.IsNullOrEmpty(tag) == false ? $"<{tag}>" : string.Empty)}{content}{(string.IsNullOrEmpty(tag) == false ? $"</{tag}>" : string.Empty)}</td>";
        }

        private class Data
        {
            public Data(IList<Forecast> allForecasts, IList<Ljuser> users, IEnumerable<Match> allMatches)
            {
                AllForecasts = allForecasts;
                Users = users;
                AllMatches = allMatches;
            }

            public IList<Forecast> AllForecasts { get; }

            public IList<Ljuser> Users { get; }

            public IEnumerable<Match> AllMatches { get; }
        }

        #region ICalculator Members

        public string CalculateTourResult(string numberString)
        {
            return CalculateTourResult(numberString.Split(',').Select(x => int.Parse(x.Trim())).ToList());
        }

        public Dictionary<int, Dictionary<string, double>> CalculateForUser(string userName)
        {
            using (unitOfWorkFactory.Create())
            {
                var matches = queryFactory.FindAll<Match>().Execute().Where(x => x.IsOver()).ToList();
                var allForecasts = queryFactory.FindAll<Forecast>().Execute().ToList();

                foreach (var match in matches)
                {
                    foreach (var forecast in allForecasts.Where(x => x.Number == match.Number))
                    {
                        SetScore(forecast, match);
                    }
                }

                var dictionary = allForecasts
                    .GroupBy(x => (x.Number - 1) / 8 + 1)
                    .ToDictionary(x => x.Key, x =>
                    {
                        return new Dictionary<string, double>
                        {
                            {NameForAvg, x.Sum(f => (int) f.Score) / (double) x.Select(f => f.Ljuser.Name).Distinct().Count()},
                            {userName, x.Where(f => f.Ljuser.Name == userName).Sum(f => (int) f.Score)}
                        };
                    });

                foreach (var key in dictionary.Keys.ToList())
                {
                    if (dictionary[key].All(d => Math.Abs(d.Value) < 0.01))
                        dictionary.Remove(key);
                }

                return dictionary;
            }
        }

        public string CalculateTourResult(IList<int> matchNumbers, IList<string> users = null)
        {
            var results = new List<Forecast>();

            var tablo = "";
            IList<Match> matches;
            using (unitOfWorkFactory.Create())
            {
                matches = queryFactory.FindAll<Match>().Execute().ToList();

                if (matchNumbers == null || matchNumbers.Any() == false)
                    matchNumbers = matches.Where(x => x.IsOver()).Select(x => x.Number).ToList();

                foreach (int matchNumber in matchNumbers)
                {
                    var forecasts = queryFactory.GetForecastsByNumber(matchNumber).Execute().Where(x => users == null || users.Contains(x.Ljuser.Name)).ToList();
                    Match match = matches.SingleOrDefault(x => x.Number == matchNumber);

                    if (match == null)
                        throw new NullReferenceException("���� �� ������");

                    tablo += $"{match.Number}. {match.Owners.Name} � {match.Guests.Name} {match.OwnersGoals}:{match.GuestsGoals}<br/>";

                    foreach (Forecast forecast in forecasts)
                    {
                        SetScore(forecast, match);
                    }

                    results.AddRange(forecasts);
                }
            }

            string html = tablo + "<br/>";
            IEnumerable<IGrouping<string, Forecast>> groupedByUser = results.GroupBy(x => x.Ljuser.Name).ToList();

            if (users == null || users.Any() == false)
            {
                string statistic = @"<u>���������� ����:</u><br/><table border=""3""><tr align=""center"">";
                statistic += GetTd("����� �����", 160, "left");
                foreach (int number in matchNumbers)
                {
                    statistic += GetTd(number);
                }
                statistic += @"</tr>";
                statistic += @"<tr align=""center"">";
                statistic = AddResults(statistic, matchNumbers, matches);
                statistic += @"</tr>";
                statistic += AddStatisticRow(results, matchNumbers, "��������� ������<br/>(4 ����)",
                    ScoreType.ScoreMatch);
                statistic += AddStatisticRow(results, matchNumbers, "��������� ������<br/>(2 ����)",
                    ScoreType.Difference);
                statistic += AddStatisticRow(results, matchNumbers, "��������� �������<br/>(1 ����)", ScoreType.Result);
                statistic += AddStatisticRow(results, matchNumbers, "������� �����<br/>(���� �� 1 ����)",
                    ScoreType.Result, ScoreType.ScoreMatch, ScoreType.Difference);
                statistic += @"</table>";
                statistic += "<br/>";

                string best = GetTheBestFromTour(groupedByUser);

                var avg = $"������� ���������� ��������� �����: <b>{groupedByUser.Select(x => x.Sum(z => (int) z.Score)).Average():F}</b><br/><br/>";

                html += statistic + best + avg;
            }
            html += @"<table border=""3""><tr align=""center"">";
            html = AddResults(html, matchNumbers, matches);

            html += GetTd("");
            html += @"</tr><tr align=""center"">" + GetTd("ljuser", null, "left");
            foreach (int number in matchNumbers)
            {
                html += GetTd(number, 35);
            }
            html += GetTd("");
            html += @"</tr>";

            foreach (var userForecasts in groupedByUser.OrderBy(x => x.Key))
            {
                html += @"<tr align=""center"">";
                html += GetTd(LjUserTag(userForecasts.Key), null, "left");
                foreach (int number in matchNumbers)
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
                            x => x.Sum(f => (int) f.Score) / (decimal) x.Count())
                        .Take(10);

                IEnumerable<IGrouping<int, Forecast>> lowestScoreToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderBy(
                            x => x.Sum(f => (int) f.Score) / (decimal) x.Count())
                        .Take(20);



                IEnumerable<IGrouping<int, Forecast>> bestResultToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderByDescending(
                            x => x.Count(f => (int)f.Score > 0) / (decimal)x.Count())
                        .Take(10);

                IEnumerable<IGrouping<int, Forecast>> lowestResultToCount =
                    forecasts.GroupBy(x => x.Number)
                        .OrderBy(
                            x => x.Count(f => (int)f.Score > 0) / (decimal)x.Count())
                        .Take(20);

                string statistic = @"<br/><br/><lj-cut text=""����� ������������� �����""><br/><STRONG>����� ������������� �����</STRONG>";
                statistic += GetTableMatchStatistic(bestScoreToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/>";
                statistic += GetTableMatchByResultStatistic(bestResultToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/></lj-cut><br/><br/><lj-cut text=""����� ��������������� �����""><br/><STRONG>����� ��������������� �����</STRONG><br/><br/>";
                statistic += GetTableMatchStatistic(lowestScoreToCount, matches);
                statistic += @"<br/>";
                statistic += @"<br/>";
                statistic += GetTableMatchByResultStatistic(lowestResultToCount, matches);
                statistic += @"<br/>";
                
                statistic += @"<br/></lj-cut><br/><br/><lj-cut text=""���������������� ������""><br/><STRONG>���������������� ������</STRONG><br/><br/>";
                IEnumerable<Command> commands = queryFactory.FindAll<Command>().Execute();

                statistic += @"<table border=""3""><tr align=""center"">";
                statistic += GetTd("�������");
                statistic += GetTd("������� ������� � ������� �� ������� � ������ � � ��������");
                statistic += GetTd("������� ����� � ������� �� ������� � ������ � � ��������");
                statistic += GetTd("������� ������� � ������� �� ������� � �������� ������");
                statistic += GetTd("������� ������� � ������� �� ������� � �������� ������");
                statistic += @"</tr>";

                foreach (Command command in commands
                    .OrderByDescending(c =>
                    {
                        List<Forecast> commandForecasts =
                            forecasts.Where(f => matches.Where(x => x.Owners == c || x.Guests == c).Select(z => z.Number).Contains(f.Number)).ToList();
                        return commandForecasts.Count(f => (int) f.Score > 0) /
                               (decimal) commandForecasts.Count;
                    })
                )
                {
                    statistic += @"<tr align=""center"">";
                    statistic += GetTd(command.Name);
                    List<Forecast> commandForecasts =
                        forecasts.Where(
                            f =>
                                matches.Where(x => x.Owners == command || x.Guests == command).Select(z => z.Number).Contains(f.Number)).ToList();
                    List<Forecast> commandHomeForecasts =
                        forecasts.Where(
                            f => matches.Where(x => x.Owners == command).Select(z => z.Number).Contains(f.Number)).ToList();
                    List<Forecast> commandGuestForecasts =
                        forecasts.Where(
                            f => matches.Where(x => x.Guests == command).Select(z => z.Number).Contains(f.Number)).ToList();
                    statistic += GetTd(
                        (commandForecasts.Count(f => (int) f.Score > 0) / (decimal) commandForecasts.Count).ToString(
                            "0.##")
                    );
                    statistic += GetTd(
                        (commandForecasts.Sum(f => (int) f.Score) / (decimal) commandForecasts.Count).ToString("0.##")
                    );
                    statistic += GetTd(
                        (commandHomeForecasts.Count(f => (int) f.Score > 0) / (decimal) commandHomeForecasts.Count).ToString("0.##")
                    );
                    statistic += GetTd(
                        (commandGuestForecasts.Count(f => (int) f.Score > 0) / (decimal) commandGuestForecasts.Count).ToString("0.##")
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

                    if (matchForecasts.GroupBy(x => Math.Sign(x.GetDifference())).Any(x => x.Count() / (double) matchForecasts.Count() >= 0.8))
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
                var xValues = stMatches.Select(x => $"{(x.Owners.Id != commandId ? x.Owners.Name + "-" + com : com + "-" + x.Guests.Name)} {x.OwnersGoals}:{x.GuestsGoals} {x.Date.Value.ToShortDateString()}")
                    .ToList();
//                var yValues = forecasts.GroupBy(x => x.Number).Select(RightResultPart).Select(x => Math.Round(x*100)).ToList();
                var yValues = stForecasts.GroupBy(x => x.Number).Select(x=>RightResultPart(x.ToList())).Select(x => Math.Round(x * 100)).ToList();

                for (int i = 0; i < xValues.Count; i++)
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

        private static decimal RightResultPart(IList<Forecast> forecasts)
        {
            return forecasts.Count(f => f.Score > 0) / (decimal) forecasts.Count;
        }

        public string CalculateBeforeTour(string numberString)
        {
            var results = new List<Forecast>();

            var numbers = numberString.Split(',')
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


                string statistic = StatisticBeforeTour(results, numbers, "�������� ����");


                statistic += "<br/>";

                var loadData = LoadData();
                IEnumerable<string> orderedLjusers = OrderedLjusers(0, loadData.AllForecasts, loadData.Users).Take(10).Select(x => x.Key.Name);
                statistic += StatisticBeforeTour(results.Where(x => orderedLjusers.Contains(x.Ljuser.Name)).ToList(),
                    numbers, @"�������� ���-10");

                return statistic;
            }
        }

        public Dictionary<int, int> CalculateTourProgress(string userName)
        {
            var result = new Dictionary<int, int>();
            var loadData = LoadData();

            var matches = loadData.AllMatches.Where(x => x.IsOver()).ToList();

            var max = matches.Max(x => x.Number);
            var tour = 1;
            for (int i = 8; i <= max; i += 8)
            {
                var orderedEnumerable = OrderedLjusers(0, loadData.AllForecasts.Where(x => x.Number <= i).ToList(), loadData.Users);
                result.Add(tour, orderedEnumerable.TakeWhile(x => x.Key.Name != userName).Count() + 1);
                tour++;
            }
            return result;
        }

        public string CalculateTurnirTable(int lastMatchNumberOfPreviousTour)
        {
            string html = @"<table border=""3""><tr align=""center"">";
            html += GetTd("�", 30);
            html += GetTd("d", 50);
            html += GetTd("ljuser", 200, "left");
            html += GetTd("������", 70);
            html += GetTd("����", 70);
            html += GetTd("�������", 70);
            html += GetTd("�����", 70);
            html += GetTd("����", 70);
            html += "</tr>";

            var loadData = LoadData();
            IEnumerable<IGrouping<Ljuser, Forecast>> orderedEnumerable = OrderedLjusers(lastMatchNumberOfPreviousTour, loadData.AllForecasts, loadData.Users);

            foreach (var userForecasts in orderedEnumerable)
            {
                html += "<tr align=\"center\">";
                html += GetTd(userForecasts.Key.Rank);
                html += GetTd(GetDifferenceRank(userForecasts));
                html += GetTd($"<lj user=\"{userForecasts.Key.Name}\">", null, "left");
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
            html += GetTd("�", 30);
            html += GetTd("ljuser", 200, "left");
            html += GetTd("������", 70);


            html += GetTd("����", 70);
            html += GetTd("����� �� ����", 70);
            html += "</tr>";

            IEnumerable<IGrouping<Ljuser, Forecast>> orderedEnumerable = OrderedByPercentLjusers();

            foreach (var userForecasts in orderedEnumerable)
            {
                html += "<tr align=\"center\">";
                html += GetTd(userForecasts.Key.Rank);
                html += GetTd($"<lj user=\"{userForecasts.Key.Name}\">", null, "left");
                html += GetTd(userForecasts.Key.Forecasts.Count());

                html += GetTd(userForecasts.Key.Scores);
                html += GetTd((userForecasts.Key.Scores / (decimal) userForecasts.Key.Forecasts.Count()).ToString("0.##"));
                html += "</tr>";
            }

            html += "</table>";

            return html;
        }

        #endregion
    }
}