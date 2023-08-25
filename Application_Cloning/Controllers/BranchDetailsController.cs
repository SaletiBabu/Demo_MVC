using Application_Cloning.DbContext_Data;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.Diagnostics;

namespace Application_Cloning.Controllers
{
    //[Route("Branch")]
    public class BranchDetailsController : Controller
    {
        private readonly DemoDbContext _Db;
        private readonly string _tempFolderRoot = Path.Combine(Directory.GetCurrentDirectory(), "TempClones");

        public BranchDetailsController(DemoDbContext db)
        {
            _Db = db;
        }


        // GET: BranchDetails List 
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lstBranch = await _Db.BranchDetails.ToListAsync();            
            return View(lstBranch);
        }

        [HttpPost("addBranch")]
        public async Task<IActionResult> addBranch(string branchName, string migrationType)
        {
            try
            {
                bool vaildCheck = _Db.BranchDetails.Any(cus => cus.Branch == branchName && cus.MigrationType==migrationType);
                if (vaildCheck)
                {
                    var addBranch = new BranchDetails()
                    {
                        Id = Guid.NewGuid(),
                        Branch = branchName,
                        MigrationType = migrationType
                    };
                    await _Db.BranchDetails.AddAsync(addBranch);
                    await _Db.SaveChangesAsync();

                    var tempFolderName = $"Cloning_{Guid.NewGuid()}";
                    var tempFolderPath = Path.Combine(_tempFolderRoot, tempFolderName);

                    string guid = Guid.NewGuid().ToString();
                    //string tempFolderPath = $"Cloning_{guid}";
                    CloneBranch(branchName, tempFolderPath);

                    // Run migration command
                    //string migrationResult = RunMigrationCommand(tempFolderPath, migrationType);
                    string migrationResult = RunMigrationCommand("C:\\Users\\Saleti.Babu\\source\\repos\\Application_Cloning\\Application_Cloning", "InitialCreate");

                    return Json(new { status = "success", message = "Data saved successfully" });//RedirectToAction(nameof(Index));
                }
                else { return Json(new { status = "error", message = "This name already exists. Do you want to try a different name?" }); ; } 
            }
            catch
            {
                return View();
            }
        }

        // GET: BranchDetailsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BranchDetailsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BranchDetailsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BranchDetailsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BranchDetailsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BranchDetailsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BranchDetailsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private void CloneBranch(string branchName, string targetFolderPath)
        {
            Repository.Clone("https://github.com/SumanthBabu4", targetFolderPath,
                new CloneOptions
                {
                    BranchName = branchName
                });
        }

        private string RunMigrationCommand(string workingDirectory, string migrationType)
        {
            // Replace with your actual migration command
            string migrationCommand = $"dotnet ef database update --migration {migrationType}";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = psi };
            process.Start();

            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(migrationCommand);
                }
            }

            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();

            return result;
        }
    }

}
