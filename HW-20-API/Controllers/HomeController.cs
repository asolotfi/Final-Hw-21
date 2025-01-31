using Find_HW_20.Models;
using Find_HW_20.Models.CarManegment;
using HW_20.Domain.Contract.Repositoris;
using HW_20.Domain.Contract.Service;
using HW_20.Domain.Contract.Sevice;
using HW_20.Domain.Entites.Car;
using HW_20.Domain.Enum;
using HW_20.Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HW_20_API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IInspectionRequestService _InspectionRequestService;
        private readonly AppDbContext _appDbContext;
        private readonly IAuthenticationAppService _AuthenticationAppService;
        private readonly ICarModelSevice _CarModelSevice;
        private readonly ICarModelAppSevice _CarModelAppSevice;

        public HomeController(ILogger<HomeController> logger, IInspectionRequestService InspectionRequestService, AppDbContext appDbContext, IAuthenticationAppService AuthenticationAppService, ICarModelSevice CarModelSevice, ICarModelAppSevice CarModelAppSevice)
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
        public IActionResult signin()
        {
            return View();
        }
        public IActionResult Show()
        {
            var requests = _appDbContext.InspectionRequests.ToList();
            return View(requests);
        }
        public IActionResult CarModel()
        {
            var requests = _appDbContext.CarModels.ToList();
            return View(requests);
        }
        public IActionResult Get()
        {
            // دریافت درخواست‌ها و برگرداندن به ویو
            var inspectionRequests = _appDbContext.InspectionRequests.ToList();
            return View(inspectionRequests);
        }
        public IActionResult Approve()
        {
            return View();
        }
        public IActionResult Reject()
        {
            return View();
        }
        public IActionResult CreateOldInspectionRequest()
        {
            return View();
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult createInspection(AddInspectionRequestViewModel model)
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateInspectionRequest(AddInspectionRequestViewModel model)
        {
            var InspectionRequestq = new InspectionRequest
            {
                PhoneNumber = model.PhoneNumber,
                CodeMeli = model.CodeMeli,
                PlateNumber = model.PlateNumber,
                Car = model.Car,
                Company = model.Company,
                ProductionDate = model.ProductionDate,

            };
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "لطفاً داده‌ها را به صورت صحیح وارد کنید.";
                ModelState.AddModelError("M1", "جرا همه فرم خالی فرستادی ");
                ModelState.AddModelError("M1", "فیلد ها رو درست پر کن ");
                return View("Index");
                //return BadRequest();
            }

            else if (!_InspectionRequestService.AddInspectionRequest(model.PhoneNumber, model.CodeMeli, model.PlateNumber, model.Car, model.Company, model.ProductionDate))
            {
                TempData["ErrorMessage"] = "درخواست معاینه ثبت نشد.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "درخواست معاینه فنی ثبت شد.";
            return RedirectToAction("Index");
        }
        [HttpPost]

        [HttpPost]
        public IActionResult CreateOldInspectionRequest(string PhoneNumber, string codeMeli, string PlateNumber, string Car, string company, DateTime ProductionDate)
        {
            var InspectionRequestq = new InspectionRequest
            {
                PhoneNumber = PhoneNumber,
                CodeMeli = codeMeli,
                PlateNumber = PlateNumber,
                Car = Car,
                Company = company,
                ProductionDate = ProductionDate,
            };
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "لطفاً داده‌ها را به صورت صحیح وارد کنید.";
                return View(InspectionRequestq);
            }

            else if (!_InspectionRequestService.AddInspectionRequest(PhoneNumber, codeMeli, PlateNumber, Car, company, ProductionDate))
            {
                TempData["ErrorMessage"] = "درخواست معاینه فنی در قسمت قدیمی ثبت نشد.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "درخواست معاینه فنی ثبت شد.";
            return RedirectToAction("Index");
        }
        // متد تأیید درخواست
        [HttpPost]
        public IActionResult Approve(int id)
        {
            try
            {
                var request = _appDbContext.InspectionRequests.Find(id);
                if (request == null)
                {
                    TempData["ErrorMessage"] = "درخواست معاینه پیدا نشد.";
                    return RedirectToAction("Index");
                }

                request.Status = RequestStatusEnum.Approved;
                _appDbContext.SaveChanges();
                TempData["SuccessMessage"] = "درخواست معاینه تأیید شد.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "خطایی در تأیید درخواست رخ داد.";
                Console.WriteLine(ex.Message);
            }
            // بازگشت به صفحه‌ی اصلی لیست
            return RedirectToAction("Show");
        }
        [HttpPost]
        public IActionResult Reject(int id)
        {
            try
            {
                var request = _appDbContext.InspectionRequests.Find(id);
                if (request != null)
                {
                    request.Status = RequestStatusEnum.Rejected;
                    _appDbContext.SaveChanges();
                    TempData["SuccessMessage"] = "درخواست معاینه رد شد.";
                }
                else
                {
                    TempData["ErrorMessage"] = "درخواست معاینه پیدا نشد.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "خطایی در رد درخواست رخ داد.";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Show");
        }
        [HttpGet]
        public IActionResult Get(RequestStatusEnum status)
        {
            var result = _InspectionRequestService.Get(status);
            if (result == null)
            {
                TempData["ErrorMessage"] = "درخواستی وجود ندارد.";
                return RedirectToAction("Show");
            }
            else
                return View("Show", result);
        }
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string userName, string password)
        {
            if (_AuthenticationAppService.Login(userName, password))
            {
                return RedirectToAction("Show", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "ورود به سیستم ناموفق بود";
                return View("Show");
            }
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
