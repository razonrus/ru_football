using System.Collections.Generic;
using System.IO;
using System.Web.Helpers;
using System.Web.Mvc;
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
            ViewBag.Message = "Welcome to ASP.NET MVC!";

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

            selenium.Open(model.Url);
            selenium.WaitForPageToLoad("30000");
            var html = selenium.GetHtmlSource();
            selenium.Stop();
            //            string html = HtmlDownloader.GetHtml(model.Url);

            string s = new ForecastParser(queryFactory, log, unitOfWorkFactory).ParseHtml(html, model.Url);
            return View(new ParseUrlModel {Result = s});
        }

        public ActionResult CalculateTourResult()
        {
            return View(new CalculateTourResultModel());
        }

        [HttpPost]
        public ActionResult CalculateTourResult(CalculateTourResultModel model)
        {
            string html = calculator.CalculateTourResult(model.Numbers);

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

        public ActionResult CalculateTurnirTable()
        {
            return View(new CalculateTurnirTableModel());
        }

        [HttpPost]
        public ActionResult CalculateTurnirTable(CalculateTurnirTableModel model)
        {
            string html = calculator.CalculateTurnirTable(model.LastMatchNumberOfPreviousTour);

            model.Result = html;
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
            return View(new ParseMatchesResultModel { Url = "http://rfpl.org/calendar" });
        }

        public ActionResult ParseTourResultEuro2012()
        {
            return View("ParseTourResult",new ParseMatchesResultModel { Url = "http://ru.uefa.com/uefaeuro/season=2012/matches/all/index.html" });
        }

        [HttpPost]
        public ActionResult ParseTourResult(ParseMatchesResultModel model)
        {
            string html = HtmlDownloader.GetHtml(model.Url);
            string s = new ForecastParser(queryFactory, log, unitOfWorkFactory).ParseResult(html);
            return View(new ParseMatchesResultModel { Result = s });
        }
        
        [HttpPost]
        public ActionResult ParseTourResultEuro2012(ParseMatchesResultModel model)
        {
            string html = HtmlDownloader.GetHtml(model.Url);
            string s = new ForecastParser(queryFactory, log, unitOfWorkFactory).ParseResultEuro2012(html);
            return View("ParseTourResult", new ParseMatchesResultModel { Result = s });
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