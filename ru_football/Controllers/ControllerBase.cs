using System.Collections.Generic;
using System.Web.Mvc;
using IndyCode.Infrastructure.Domain;
using IQueryFactory = Domain.IQueryFactory;

namespace ru_football.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly ICalculator Calculator;
        protected readonly IQueryFactory QueryFactory;
        protected readonly IUnitOfWorkFactory UnitOfWorkFactory;

        public ControllerBase(IQueryFactory queryFactory, IUnitOfWorkFactory unitOfWorkFactory, ICalculator calculator)
        {
            QueryFactory = queryFactory;
            UnitOfWorkFactory = unitOfWorkFactory;
            Calculator = calculator;
        }

        protected static List<int> GetMatchNumbers(int tourNumber)
        {
            var numbers = new List<int>();
            for (int j = (tourNumber - 1)*8 + 1; j <= tourNumber * 8; j++)
            {
                numbers.Add(j);
            }
            return numbers;
        }
    }
}