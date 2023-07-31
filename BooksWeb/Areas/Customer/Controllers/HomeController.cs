using Books.DataAccess.Repository.IRepository;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BooksWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(productList);
        }

        public IActionResult Details(int? productId)
        {
            if (productId is null || productId == 0)
                return NotFound();

            var product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category");
            if (product == null) 
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}