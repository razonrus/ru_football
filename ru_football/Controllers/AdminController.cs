using System.Linq;
using System.Web.Mvc;
using Domain;
using IndyCode.Infrastructure.Domain;
using log4net;
using Parser;
using ru_football.Models;
using Selenium;
using IQueryFactory = Domain.IQueryFactory;

namespace ru_football.Controllers
{
    [AllowAdminAttribute]
    public class AdminController : ControllerBase
    {
        private ISelenium selenium;
        private readonly ILog log;

        public AdminController(IQueryFactory queryFactory, ILog log, IUnitOfWorkFactory unitOfWorkFactory, ICalculator calculator)
             : base(queryFactory, unitOfWorkFactory, calculator)
        {
            this.log = log;
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

            string s = new ForecastParser(QueryFactory, log, UnitOfWorkFactory).ParseHtml(html, model.Url);
            return View(new ParseUrlModel {Result = s});
        }

        public ActionResult CalculateTourResult()
        {
            using (UnitOfWorkFactory.Create())
            {
                var matchesCount = QueryFactory.FindAll<Match>().Execute().Count();

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
                model.Result = Calculator.CalculateTourResult(model.Numbers);
                return View(model);
            }

            var tours = model.Tours.Split('-');
            var start = int.Parse(tours.First());
            var end = int.Parse(tours.Last());

            string html = "";
            for (int i = start; i <= end; i++)
            {
                html += string.Format(@"<lj-cut text=""Результаты {0}-го тура ЧР.""><BR><h4>Результаты {0}-го тура</h4>", i);
                var numbers = GetMatchNumbers(i);
                
                html += Calculator.CalculateTourResult(numbers);
                html += "</lj-cut><BR><BR>";
            }

            html += string.Format(@"<lj-cut text=""Турнирная таблица после {0}-го тура ЧР.""><BR><h4>Турнирная таблица</h4>", end);
            html += Calculator.CalculateTurnirTable((start - 1) * 8);
            html += "</lj-cut>";

            model.Result = html;
            return View(model);
        }
        
        public ActionResult SureThing()
        {
            string html = Calculator.SureThing();

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
            return Calculator.Chart(id);
        }

        [HttpPost]
        public ActionResult TourResultFromFile(ParseUrlModel model)
        {
            string text = System.IO.File.ReadAllText(model.Url);
            new ForecastParser(QueryFactory, log, UnitOfWorkFactory).ParseHtml(text);
            return new EmptyResult();
        }

        public ActionResult CalculateTurnirTable()
        {
            using (UnitOfWorkFactory.Create())
            {
                var matchesCount = QueryFactory.FindAll<Match>().Execute().Count();

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
            model.Result = Calculator.CalculateTurnirTable(model.LastMatchNumberOfPreviousTour);
            return View(model);
        }

        public ActionResult CalculatePercentTurnirTable()
        {
            string html = Calculator.CalculatePercentTurnirTable();

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
            string s = new ForecastParser(QueryFactory, log, UnitOfWorkFactory).ParseResult(html);
            return View(new ParseMatchesResultModel { Result = s });
        }
        
        public ActionResult BeforeTour()
        {
            return View(new BeforeTourModel());
        }

        [HttpPost]
        public ActionResult BeforeTour(BeforeTourModel model)
        {
            string html = Calculator.CalculateBeforeTour(model.Numbers);

            model.Result = html;
            return View(model);
        }
    }
}