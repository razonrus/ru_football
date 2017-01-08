using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Domain;
using IndyCode.Infrastructure.Domain;
using ru_football.Models;
using IQueryFactory = Domain.IQueryFactory;
using Match = Domain.Match;

namespace ru_football.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IQueryFactory queryFactory, IUnitOfWorkFactory unitOfWorkFactory, ICalculator calculator) 
            : base(queryFactory, unitOfWorkFactory, calculator)
        {
        }

        public ActionResult Index()
        {
            using (UnitOfWorkFactory.Create())
            {
                var users = QueryFactory.FindAll<Ljuser>().Execute().ToList();
                var matches = QueryFactory.FindAll<Match>().Execute().Where(x=>x.IsOver()).ToList();

                return View(new UserIndexModel
                {
                    UserNames = users.Select(x => x.Name).ToList(),
                    TourNumbers = matches.Select(x=>x.GetTourNumber()).Distinct().ToList()
                });
            }
        }
        
        public ActionResult UserStatistic(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return View();

            return View(new UserStatsModel
            {
                Name = userName,
                Result = Calculator.CalculateForUser(userName)
            });
        }
        
        public ActionResult TourResult(int number)
        {
            var numbers = GetMatchNumbers(number);

            var html = LjToHtml(Calculator.CalculateTourResult(numbers));
            
            return View((object)html);
        }

        public ActionResult CurrentTurnirTable()
        {
            int matchesCount;
            using (UnitOfWorkFactory.Create())
            {
                matchesCount = QueryFactory.FindAll<Match>().Execute().Count(x => x.IsOver());
            }

            return View("TurnirTable", new CalculateTurnirTableModel
            {
                Result = LjToHtml(Calculator.CalculateTurnirTable(matchesCount - 8))
            });
        }
        
        private string LjToHtml(string input)
        {
            return Regex.Replace(input, "<lj user=\"([^\"]+)\">",
                @"<span><a href=""http://$1.livejournal.com/profile"" target=""_self""><img style=""vertical-align: text-bottom;"" src=""http://l-stat.livejournal.net/img/userinfo.gif?v=17080?v=144""></a><a href=""http://$1.livejournal.com/"" target=""_self""><b>$1</b></a></span>"
                );
        }

        public ActionResult Disqus()
        {
            return View();
        }
    }
}