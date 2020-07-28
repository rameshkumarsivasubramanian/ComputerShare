using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Models.FileViewModel> retVal = new List<FileViewModel>();

            string[] filePaths = FileManager.GetReportFiles();

            foreach (string filePath in filePaths)
            {
                string fileContent = FileManager.ReadFile(filePath);
                FileViewModel fileViewModel = JsonConvert.DeserializeObject<FileViewModel>(fileContent);

                retVal.Add(fileViewModel);
            }

            return View(retVal.ToArray());
        }

        [HttpPost]
        public JsonResult Upload()
        {
            List<FileViewModel> retVal = new List<FileViewModel>();

            foreach (IFormFile fl in Request.Form.Files)
            {
                FileViewModel fileViewModel = new FileViewModel(fl);
                fileViewModel.ValidateData(); 
                fileViewModel.SaveFile();
                fileViewModel.SaveReport();

                retVal.Add(fileViewModel);
            }

            return Json(retVal.ToArray());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}