using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using IndyCode.Infrastructure.Domain;
using Parser;
using Selenium;
using log4net;
using ru_football.Models;
using IQueryFactory = Domain.IQueryFactory;

namespace ru_football.Controllers
{
    public class HomeController : Controller
    {
        private ISelenium selenium;
        private readonly ICalculator calculator;
        private readonly ILog log;
        private readonly IQueryFactory queryFactory;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public HomeController(IQueryFactory queryFactory, ILog log, IUnitOfWorkFactory unitOfWorkFactory, ICalculator calculator)
        {
            this.queryFactory = queryFactory;
            this.log = log;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.calculator = calculator;
        }

        public ActionResult Index()
        {
            using (unitOfWorkFactory.Create())
            {
                var users = queryFactory.FindAll<Ljuser>().Execute().ToList();
                var matches = queryFactory.FindAll<Match>().Execute().ToList();

                return View(new UserIndexModel
                {
                    UserNames = users.Select(x => x.Name).ToList(),
                    TourNumbers = matches.Select(x=>x.GetTourNumber()).Distinct().ToList()
                });
            }
        }

        public ActionResult AdminIndex()
        {
            ViewBag.Message = "Admin Index";

            return View();
        }

        public ActionResult ParseTourFromUrl()
        {
            return View();
        }

        public ActionResult UserStatistic(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return View();

            return View(new UserStatsModel
            {
                Name = userName,
                Result = calculator.CalculateForUser(userName)
            });
        }

        [HttpPost]
        public ActionResult ParseTourFromUrl(ParseUrlModel model)
        {
            selenium = new DefaultSelenium("localhost", 4444, "*firefox", "http://localhost");
            selenium.Start();
            selenium.SetTimeout("120000");
            selenium.Open(model.Url);
            selenium.WaitForPageToLoad("60000");
            var html = selenium.GetHtmlSource();
            selenium.Stop();
            //            string html = HtmlDownloader.GetHtml(model.Url);

            string s = new ForecastParser(queryFactory, log, unitOfWorkFactory).ParseHtml(html, model.Url);
            return View(new ParseUrlModel {Result = s});
        }

        public ActionResult CalculateTourResult()
        {
            using (unitOfWorkFactory.Create())
            {
                var matchesCount = queryFactory.FindAll<Match>().Execute().Count();

                var numbers = "";
                for (int i = matchesCount - 7; i <= matchesCount; i++)
                {
                    numbers += i + ",";
                }

                return View(new CalculateTourResultModel
                                {
                                    Numbers = numbers.TrimEnd(',')
                                });
            }
        }

        [HttpPost]
        public ActionResult CalculateTourResult(CalculateTourResultModel model)
        {
            if (string.IsNullOrEmpty(model.Numbers) == false)
            {
                model.Result = calculator.CalculateTourResult(model.Numbers);
                return View(model);
            }

            var tours = model.Tours.Split('-');
            var start = int.Parse(tours.First());
            var end = int.Parse(tours.Last());

            string html="";
            for (int i = start; i <= end; i++)
            {
                html += string.Format(@"<lj-cut text=""Результаты {0}-го тура ЧР.""><BR><h4>Результаты {0}-го тура</h4>", i);
                var numbers = new List<int>();
                for (int j = (i-1)*8+1; j <= i*8; j++)
                {
                    numbers.Add(j);
                }

                html += calculator.CalculateTourResult(numbers);
                html += "</lj-cut><BR><BR>";
            }

            html += string.Format(@"<lj-cut text=""Турнирная таблица после {0}-го тура ЧР.""><BR><h4>Турнирная таблица</h4>", end);
            html += calculator.CalculateTurnirTable((start-1)*8);
            html += "</lj-cut>";

            model.Result = html;
            return View(model);
        }

        public ActionResult SureThing()
        {
            string html = calculator.SureThing();

            return View("CalculateTourResult", new CalculateTourResultModel
                            {
                                Result = html
                            });
        }

        public ActionResult Chart()
        {
            return View();
        }


        public FileResult CreateChart(int id)
        {
            return calculator.Chart(id);
        }

        [HttpPost]
        public ActionResult TourResultFromFile(ParseUrlModel model)
        {
            string text = System.IO.File.ReadAllText(model.Url);
            new ForecastParser(queryFactory, log, unitOfWorkFactory).ParseHtml(text);
            return new EmptyResult();
        }

        public ActionResult CurrentTurnirTable()
        {
            int matchesCount;
            using (unitOfWorkFactory.Create())
            {
                matchesCount = queryFactory.FindAll<Match>().Execute().Count(x => x.IsOver());
            }

            return View("TurnirTable", new CalculateTurnirTableModel
            {
                Result = calculator.CalculateTurnirTable(matchesCount - 8)
            });
        }

        public ActionResult CalculateTurnirTable()
        {
            using (unitOfWorkFactory.Create())
            {
                var matchesCount = queryFactory.FindAll<Match>().Execute().Count();

                return View(new CalculateTurnirTableModel
                                {
                                    LastMatchNumber = matchesCount,
                                    LastMatchNumberOfPreviousTour = matchesCount - 8
                                });
            }
        }

        [HttpPost]
        public ActionResult CalculateTurnirTable(CalculateTurnirTableModel model)
        {
            model.Result = calculator.CalculateTurnirTable(model.LastMatchNumberOfPreviousTour);
            return View(model);
        }

        public ActionResult CalculatePercentTurnirTable()
        {
            string html = calculator.CalculatePercentTurnirTable();

            return View("CalculateTurnirTable", new CalculateTurnirTableModel
                            {
                                Result = html
                            });
        }

        public ActionResult ParseTourResult()
        {
            return View(new ParseMatchesResultModel { Url = "http://www.sports.ru/rfpl/calendar/" });
        }
        
        [HttpPost]
        public ActionResult ParseTourResult(ParseMatchesResultModel model)
        {
            string html = HtmlDownloader.GetHtml(model.Url);
            string s = new ForecastParser(queryFactory, log, unitOfWorkFactory).ParseResult(html);
            return View(new ParseMatchesResultModel { Result = s });
        }
        
        public ActionResult BeforeTour()
        {
            return View(new BeforeTourModel());
        }

        [HttpPost]
        public ActionResult BeforeTour(BeforeTourModel model)
        {
            string html = calculator.CalculateBeforeTour(model.Numbers);

            model.Result = html;
            return View(model);
        }
    }
}