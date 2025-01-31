using HW_20.Domain.Contract.Repositoris;
using HW_20.Domain.Contract.Service;
using HW_20.Domain.Contract.Sevice;
using HW_20.Domain.Entites.Car;
using HW_20.Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;

namespace HW_20_API.Controllers
{
    public class CarModelController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IInspectionRequestService _InspectionRequestService;
        private readonly AppDbContext _appDbContext;
        private readonly IAuthenticationAppService _AuthenticationAppService;
        private readonly ICarModelSevice _CarModelSevice;
        private readonly ICarModelAppSevice _CarModelAppSevice;

        public CarModelController(ILogger<HomeController> logger, IInspectionRequestService InspectionRequestService, AppDbContext appDbContext, IAuthenticationAppService AuthenticationAppService, ICarModelSevice CarModelSevice, ICarModelAppSevice CarModelAppSevice)
        {
            _logger = logger;
            _InspectionRequestService = InspectionRequestService;
            _appDbContext = appDbContext;
            _AuthenticationAppService = AuthenticationAppService;
            _CarModelSevice = CarModelSevice;
            _CarModelAppSevice = CarModelAppSevice;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CarModel()
        {
            var requests = _appDbContext.CarModels.ToList();
            return View(requests);
        }
        public IActionResult AddCarModel()
        {
            CarModel carModel = new CarModel(); // مطمئن شوید که مدل صحیح را ارسال می‌کنید
            return View(carModel);
        }   
        [HttpPost]
        public IActionResult DeleteCarModel(int id)
        {
            var car = DeletCarModel(id);
            if (car == null)
            {
                return NotFound();
            }

            DeletCarModel(id); // حذف مدل از دیتابیس
            return RedirectToAction("CarModel"); // بعد از حذف به صفحه لیست برگردید
        }
        [HttpPost]
        public IActionResult EditCarModel(int id)
        {
            var car = Get(id); // فرض می‌کنیم از یک سرویس برای دریافت اطلاعات استفاده می‌کنید
            if (car == null)
            {
                return NotFound();
            }
            return View("EditCarModel"); // ارسال مدل به صفحه ویرایش
        }
        [HttpPost]
        public IActionResult AddCarModele(string name)
        {
            var cardModel = new CarModel
            {
                Name = name,
            };
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "لطفاً داده‌ها را به صورت صحیح وارد کنید.";
                return View("Index", cardModel);
            }
            var result = _CarModelAppSevice.AddCarModel(name);
            TempData["SuccessMessage"] = "مدل ثبت شد.";
            return RedirectToAction("CarModel");
        }
        [HttpPost]
        public IActionResult DeletCarModel(int id)
        {
            try
            {
                var result = _CarModelAppSevice.DeleteCarModel(id);
                if (result == null)
                {
                    TempData["ErrorMessage"] = "مدل پیدا نشد.";
                    return RedirectToAction("CarModel");
                }
                TempData["SuccessMessage"] = "مدل تأیید شد.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "خطایی در تأیید مدل رخ داد.";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("CarModel");
        }
        [HttpPost]
        public IActionResult EditeCarModel(int id, string name)
        {
            var model = _appDbContext.CarModels.Find(id);
            var cardModel = new CarModel
            {
                Id = id,                
                Name = name
            };
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "لطفاً داده‌ها را به صورت صحیح وارد کنید.";
                return View("Index", cardModel);
            }
            var result = _CarModelAppSevice.EditCarModel(id, name);
            if (result && id != null)
            {
                TempData["ErrorMessage"] = " موفق بود.";
                return RedirectToAction("CarModel");
            }
            else
            {
                TempData["ErrorMessage"] = "ویرایش ناموفق بود.";
                return View("CarModel");
            }
        }
        [HttpGet]
        public IActionResult Get(int id)
        {
            var result = _CarModelSevice.GetCarModel(id);
            if (result == null)
            {
                TempData["ErrorMessage"] = "مدل وجود ندارد.";
                return RedirectToAction("Show");
            }
            else
                return View(result);
        }
    }
}
