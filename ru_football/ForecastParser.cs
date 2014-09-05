using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain;
using HtmlAgilityPack;
using IndyCode.Infrastructure.Domain;
using Parser;
using log4net;
using IQueryFactory = Domain.IQueryFactory;
using Match = Domain.Match;

namespace ru_football
{
    public class ForecastParser
    {
        private readonly ILog log;
        private readonly IQueryFactory queryFactory;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly int[] exclusionsMatchNumber = new[] {28};

        public ForecastParser(IQueryFactory queryFactory, ILog log, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.queryFactory = queryFactory;
            this.log = log;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public static IEnumerable<Forecast> Parse(string content, Ljuser user)
        {
            var result = new List<Forecast>();
            content = new Regex("\\<p.*?\\<font.*?\\<a.*?a\\>.*?font\\>.*?p\\>").Replace(content, "").Replace("!", "1");
            string[] lines = content.Split(new[] {"<br>", "<br />"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                GetValue(user, result, line);
            }

            return result;
        }

        private static void GetValue(Ljuser user, List<Forecast> result, string line)
        {
            string clearedLine = new Regex("<.*?>").Replace(line, string.Empty);
            string cl;
            int? number = GetValue(clearedLine, out cl);
            if (number.HasValue == false)
                return;
            int? ownersGoals = GetValue(cl, out cl);
            if (ownersGoals.HasValue == false)
                return;
            int? guestsGoals = GetValue(cl, out cl);
            if (guestsGoals.HasValue == false)
            {
                if (clearedLine.Contains("6"))
                {
                    GetValue(user, result, clearedLine.Replace("6", ":"));
                }
                return;
            }
            result.Add(new Forecast(user)
                           {
                               GuestsGoals = guestsGoals.Value,
                               Number = number.Value,
                               OwnersGoals = ownersGoals.Value
                           }
                );
        }

        private static int? GetValue(string line, out string restOfString)
        {
            restOfString = line;
            var regexNumber = new Regex("\\d+");
            string result = regexNumber.Match(restOfString).Value;
            restOfString = restOfString.Remove(restOfString.IndexOf(result), result.Length);

            int res;
            if (Int32.TryParse(result, out res))
                return res;
            return null;
        }

        public string ParseHtml(string html, string url = "")
        {
            var article = XpathSelector.Get(html, "//article[contains(@class, 'b-singlepost-body')]").First();

            MatchCollection matches = new Regex(@">\s*(?<number>\d+)\.\s*(\w*\s+)?(?<owner>[А-Яа-я]+)\s*(\(\w*\))?\s*[:-]\s*(?<guest>[А-Яа-я]+)").Matches(article.InnerHtml);

            string resultMessage = "";

            using (IUnitOfWork uow = unitOfWorkFactory.Create())
            {

                foreach (System.Text.RegularExpressions.Match regexMatch in matches)
                {
                    var number = int.Parse(regexMatch.Groups["number"].Value);
                    var owners = regexMatch.Groups["owner"].Value;
                    var guests = regexMatch.Groups["guest"].Value;

                    var match = queryFactory.GetMatchByNumber(number).Execute();
                    if (match == null)
                    {
                        uow.Save(new Match()
                        {
                            Number = number,
                            Owners = queryFactory.GetCommandByName(owners).Execute(),
                            Guests = queryFactory.GetCommandByName(guests).Execute()
                        });
                    }

                    resultMessage += string.Format("<br/>{0}. {1} - {2}", number, owners, guests);
                }
                uow.Commit();

            }
            resultMessage += "<br/>";

            List<HtmlNode> comments = XpathSelector.Get(html, "//div[contains(@class, 'b-tree-twig')]").ToList();
            int forecastCount = 0;
            foreach (HtmlNode comment in comments)
            {
                var nameNode = XpathSelector.Get(comment.OuterHtml, "//a/b/text()").FirstOrDefault();
                if (nameNode == null) 
                    continue;
                string name = nameNode.InnerText;
                var singleOrDefault = XpathSelector.Get(comment.OuterHtml, "//div[@class='b-leaf-article']").SingleOrDefault();
                if (singleOrDefault != null)
                {
                    string content = singleOrDefault.InnerHtml;

                    using (IUnitOfWork uow = unitOfWorkFactory.Create())
                    {
                        Ljuser user = queryFactory.GetLjuserByName(name).Execute() ?? new Ljuser {Name = name};

                        List<Forecast> forecasts = Parse(content, user).ToList();

                        if (forecasts.Count() != 8)
                        {
                            string message = string.Format("Юзер {0} сделал {1} прогнозов", user.Name, forecasts.Count());
                            resultMessage += message + Environment.NewLine;
                            log.Warn(message);
                            log.WarnFormat(content);
                        }

                        if (forecasts.Any())
                        {
                            int addForecasts = user.AddForecasts(forecasts);
                            forecastCount += addForecasts;
                            if (addForecasts != 8)
                            {
                                string message = string.Format("Юзеру {0} добавлено {1} прогнозов", user.Name, addForecasts);
                                resultMessage += message + Environment.NewLine;
                                log.Warn(message);
                            }
                            uow.Save(user);
                        }

                        uow.Commit();
                    }
                }
            }

            if (comments.Count() != 25)
            {
                log.WarnFormat("на странице {0} комментариев{1}", comments.Count(), Environment.NewLine);
                log.WarnFormat(url);
            }

            resultMessage += string.Format("Распарсено {0} комментариев {1}", comments.Count(), Environment.NewLine);
            resultMessage += string.Format("добавлено {0} новых прогнозов {1}", forecastCount, Environment.NewLine);


            return resultMessage;
        }

        public string ParseResult(string html)
        {
            string resultMessage = "";
            IEnumerable<HtmlNode> matchesTr = XpathSelector.Get(html, "//table[@class='stat-table']/tbody/tr");
            using (IUnitOfWork uow = unitOfWorkFactory.Create())
            {
                IEnumerable<Match> allMatches = queryFactory.FindAll<Match>().Execute().ToList();
                foreach (HtmlNode matchTr in matchesTr)
                {

                    var scores = XpathSelector.Get(matchTr.OuterHtml, "//td[@class='score-td']//a").Single().InnerText.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                    int ownersGoals;
                    if(int.TryParse(scores.First(), out ownersGoals) == false)
                        break;
                    
                    var owners = XpathSelector.Get(matchTr.OuterHtml, "//td[@class='owner-td']//a").Single().InnerText;
                    var guests = XpathSelector.Get(matchTr.OuterHtml, "//td[@class='guests-td']//a").Single().InnerText;
                    var dateString = XpathSelector.Get(matchTr.OuterHtml, "//td[@class='name-td alLeft']").Single().InnerText;
                    
                    int guestsGoals = int.Parse(scores.Last());

                    DateTime date = DateTime.Parse(dateString.Replace("|", " "));

                    Match matchFromDb = allMatches.SingleOrDefault(x => x.Guests.Name == guests && x.Owners.Name == owners);
                    if (matchFromDb != null)
                    {
                        if (matchFromDb.OwnersGoals.HasValue && matchFromDb.GuestsGoals.HasValue)
                        {
                            if ((matchFromDb.Guests.Name != guests ||
                                 matchFromDb.GuestsGoals != guestsGoals ||
                                 matchFromDb.Owners.Name != owners ||
                                 matchFromDb.OwnersGoals != ownersGoals))
                                throw new ArgumentException(string.Format("Результаты матча {0} не совпадают!", owners + "-" + guests));
                        }
                        else
                        {
                            matchFromDb.GuestsGoals = guestsGoals;
                            matchFromDb.OwnersGoals = ownersGoals;
                            matchFromDb.Date = date;

                            resultMessage += string.Format("{0}. {1} - {2} {3}:{4}{5}", matchFromDb.Number, owners, guests, ownersGoals, guestsGoals, Environment.NewLine);
                        }
                    }
//                    else
//                    {
//                        uow.Save(new Match
//                                     {
//                                         Number = number,
//                                         OwnersGoals = ownersGoals,
//                                         GuestsGoals = guestsGoals,
//                                         Guests = queryFactory.GetCommandByName(guests).Execute(),
//                                         Owners = queryFactory.GetCommandByName(owners).Execute(),
//                                         Date = date
//                                     });
//                    }
                }
                uow.Commit();
            }
            return resultMessage;
        }

        public string ParseResultEuro2012(string html)
        {
            string resultMessage = "";
            const string xpathMatchTr = "//table[@class='date20111115']/following::tr[contains(@class, 'match_res')][td[@class='c b score nob'][a]]";
            IEnumerable<HtmlNode> matchesTr = XpathSelector.Get(html, xpathMatchTr);
            using (IUnitOfWork uow = unitOfWorkFactory.Create())
            {
                IEnumerable<Match> allMatches = queryFactory.FindAll<Match>().Execute().ToList();
                var matchNumber = 31;
                foreach (HtmlNode matchTr in matchesTr)
                {
                    int number = matchNumber;
                    string owners = XpathSelector.Get(matchTr.OuterHtml, "//td[1]//text()").Single().InnerText;
                    string guests = XpathSelector.Get(matchTr.OuterHtml, "//td[5]//text()").Single().InnerText;

                    string[] scores = XpathSelector.Get(matchTr.OuterHtml, "//td[3]//text()").Single().InnerText.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    int ownersGoals = int.Parse(scores.First());
                    int guestsGoals = int.Parse(scores.Last());

                    DateTime date = DateTime.Parse(XpathSelector.Get(html, string.Format("{0}[{1}]/preceding-sibling::tr//span[@class='b dateT']", xpathMatchTr, 1)).Single().InnerText);

                    Match matchFromDb = allMatches.SingleOrDefault(x => x.Number == number);
                    if (matchFromDb != null)
                    {
                        if ((matchFromDb.Guests.Name != guests ||
                            matchFromDb.GuestsGoals != guestsGoals ||
                            matchFromDb.Owners.Name != owners ||
                            matchFromDb.OwnersGoals != ownersGoals) && exclusionsMatchNumber.Contains(number) == false )
                            throw new ArgumentException(string.Format("Результаты матча {0} не совпадают!", number));
                    }
                    else
                    {
                        var guestCommand = queryFactory.GetCommandByName(guests).Execute();
                        if (guestCommand == null)
                            uow.Save(guestCommand = new Command {Name = guests});

                        var ownerCommand = queryFactory.GetCommandByName(owners).Execute();
                        if (ownerCommand == null)
                            uow.Save(ownerCommand = new Command { Name = owners });

                        uow.Save(new Match
                                     {
                                         Number = number,
                                         OwnersGoals = ownersGoals,
                                         GuestsGoals = guestsGoals,
                                         Guests = guestCommand,
                                         Owners = ownerCommand,
                                         Date = date
                                     });
                        resultMessage += string.Format("{0}. {1} - {2} {3}:{4}{5}", number, owners, guests, ownersGoals, guestsGoals, Environment.NewLine);
                    }

                    matchNumber += 1;
                }
                uow.Commit();
            }
            return resultMessage;
        }
    }
}