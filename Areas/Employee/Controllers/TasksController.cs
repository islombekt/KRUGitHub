using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public TasksController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Manager,  Employee")]
        //  [Authorize(Roles = SD.Role_Employee)]
        // [Authorize(Roles = SD.Role_Requester)]
        [HttpGet]
        public JsonResult GetTasks()
        {
            int count = 0;
            if (User.IsInRole(SD.Role_Employee))
            {
                var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
               count = _context.Tasks.Include(i => i.Task_Emples).Where(r => (r.Task_Emples.ToList().Select(i => i.EmplId).Contains(EmpId) || r.MasulEmplId == EmpId) && r.Finished == false).Count();

            }
            else if (User.IsInRole(SD.Role_Manager))
            {
                var ManId = _context.Managers.Include(i => i.User).ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).User.DepartmentId;
                count = _context.Tasks.ToList().Where(w => w.Finished == false && w.DepartmentId == ManId).Count();

            }
            return Json(count);
        }
        [HttpGet]
        public async Task<IActionResult> ExcelSpAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


           
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var EmpName = _context.Employees.Include(u => u.User).ToList().FirstOrDefault(u => u.EmployeeId == EmpId);
            var finReport = (await _context.FinanceReports.Include(a => a.Address).Include(i => i.Category111).Include(i => i.Category11).ThenInclude(t => t.Category111s)
                .Include(i => i.Category1).ThenInclude(i => i.Category11s).ThenInclude(i => i.Category111s).ToListAsync()).Where(w => w.TaskId == id);
            var task = (await _context.Tasks.ToListAsync()).FirstOrDefault(t => t.TaskId == id);
            var employeesIds = _context.FinanceReports.Where(t => t.TaskId == id).ToList().Select(i => i.EmployeeId).Distinct();
            //string addrr = finReport.FirstOrDefault().Address.Building;
            var addr = finReport.Select(i => i.Address.Building).Distinct();
            string addrr = "";
            foreach(var i in addr)
            {
                addrr += " " + i;
            }
            var employees = _context.Employees.Include(u => u.User);
            var totalYear = task.TaskEnd.Year - task.TaskStarted.Year;
           // XLWorkbook sampleFile_ = XLWorkbook.OpenFromTemplate("wwwroot/files/excTemp/Spravka.xlsm");
            
            using (var workbook = new XLWorkbook())
            {
 
                if (employeesIds.Count() != 0)
                {
                    foreach (int item in employeesIds)
                    {

                        var name = employees.Where(u => u.EmployeeId == item).FirstOrDefault();
                        var sheet = workbook.Worksheets.Add(name.User.FullName);
                      
                        sheet.Column("A").Style.ToString();
                       
                        #region 
                        sheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        sheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        sheet.Columns("B").Width = 30;
                        var range3 = sheet.Range("A4:F4");
                        range3.Value = "Ўрганиш ўтказган Молиявий ва комплаенс назорати департаменти ходимининг исм шарифи :";
                        range3.Merge();
                        range3.Style.Font.Bold = true;
                        range3.Style.Font.Underline = XLFontUnderlineValues.Single;
                        var range4 = sheet.Range("A6:B6");
                        range4.Value = "Ўрганиш ўтказилган ташкилотнинг номи: " +  addrr;
                        range4.Merge();
                        range4.Style.Font.FontSize = 10;
                        range4.Style.Font.SetFontName("Arial Cyr");
                        range4.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range4.Style.Font.Bold = true;
                        range4.Style.Font.Underline = XLFontUnderlineValues.Single;
                        var range444 = sheet.Range("C6:E6");
                        range444.Merge();
                        var range5 = sheet.Range("A8:C8");
                        range5.Value = "Юқори органнинг номи:";
                        range5.Merge();
                        range5.Style.Font.FontSize = 10;
                        range5.Style.Font.SetFontName("Arial Cyr");
                        range5.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range5.Style.Font.Bold = true;
                        range5.Style.Font.Underline = XLFontUnderlineValues.Single;

                        var range6 = sheet.Range("A10:C10");
                        range6.Value = "Ўрганиш ўтказишга асос: \"Ўзбекнефтгаз\" АЖ нинг";
                        range6.Merge();
                        range6.Style.Font.FontSize = 10;
                        range6.Style.Font.SetFontName("Arial Cyr");
                        range6.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range6.Style.Font.Bold = true;
                        range6.Style.Font.Underline = XLFontUnderlineValues.Single;

                        var range7 = sheet.Range("A12:B12");
                        range7.Value = "Ўрганиш бошланган сана";
                        range7.Merge();
                        range7.Style.Font.FontSize = 10;
                        range7.Style.Font.SetFontName("Arial Cyr");
                        range7.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range7.Style.Font.Bold = true;
                        range7.Style.Font.Underline = XLFontUnderlineValues.Single;

                        var range8 = sheet.Range("A14:B14");
                        range8.Value = "Текшириш якунланган сана";
                        range8.Merge();
                        range8.Style.Font.FontSize = 10;
                        range8.Style.Font.SetFontName("Arial Cyr");
                        range8.Style.Font.Bold = true;
                        range8.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range8.Style.Font.Underline = XLFontUnderlineValues.Single;

                        var range9 = sheet.Range("A16:B16");
                        range9.Value = "Ўрганиш даври:";
                        range9.Merge();
                        range9.Style.Font.FontSize = 10;
                        range9.Style.Font.SetFontName("Arial Cyr");
                        range9.Style.Font.Bold = true;
                        range9.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range9.Style.Font.Underline = XLFontUnderlineValues.Single;

                        var range10 = sheet.Range("A18:C18");
                        range10.Value = "Ўрганишга ҳақиқий сарфланган вақт:";
                        range10.Merge();
                        range10.Style.Font.FontSize = 10;
                        range10.Style.Font.SetFontName("Arial Cyr");
                        range10.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        range10.Style.Font.Bold = true;
                        range10.Style.Font.Underline = XLFontUnderlineValues.Single;

                        var range11 = sheet.Range("A20:A22");
                        range11.Value = "Т/р";
                        range11.Merge();
                        var range12 = sheet.Range("B20:E22");
                        range12.Value = "Ёзув мазмуни";
                        range12.Merge();
                        var range13 = sheet.Range("F20:F22");
                        range13.Value = "Қатор рақами";
                        range13.Merge();
                        var range14 = sheet.Range("G20:G22");
                        range14.Value = "Жами суммаси";
                        range14.Merge();
                        #endregion
                        int yearI = 8;
                        List<string> array = new List<string>();
                        array.Add("Testtt");
                        for (int i = 0; i <= totalYear; i++)
                        {
                            sheet.Cell(22, 8 + i).Value = (task.TaskStarted.Year + i) + " йил";
                            sheet.Column(8 + i).Width = 13;
                            double summ1 = 0;
                            double summ11 = 0;
                            double summ111 = 0;
                            int row1 = 0;
                            int row11 = 0;
                            int row111 = 0;
                            int first = 1;
                            int second = 1;
                            int third = 1;
                            int catRows = 23;

                            foreach (var cat1 in finReport.Where(i => i.EmployeeId == item).OrderBy(i => i.Category1Id).Select(s => s.Category1Id).Distinct())
                            {
                                var categ1 = _context.Category1s.FirstOrDefault(i => i.Category1Id == cat1);
                                var r = sheet.Range(catRows, 2, catRows, 5);
                                if (array.Count != 0)
                                {
                                    if (array.Contains(categ1.Name_C1))
                                    {

                                    }
                                    else
                                    {
                                        r.Value = categ1.Name_C1;
                                        r.Merge();
                                        array.Add(categ1.Name_C1);
                                    }

                                }
                                else
                                {
                                    array.Add(categ1.Name_C1);
                                    r.Value = categ1.Name_C1;
                                    r.Merge();
                                }
                                //**************************************************
                                sheet.Cell(catRows, 1).Value = first;
                                catRows++;

                                foreach (var cat2 in finReport.Where(i => i.EmployeeId == item).OrderBy(i => i.Category11Id).ToList().Where(i => i.Category1Id == cat1).Select(i => i.Category11Id).Distinct())
                                {
                                    var categ2 = _context.Category11s.FirstOrDefault(i => i.Category11Id == cat2);
                                    if (cat2 != null)
                                    {
                                        int row = catRows;
                                        r = sheet.Range(catRows, 2, catRows, 5);

                                        if (array.Contains(categ2.Name_C11))
                                        {

                                        }
                                        else
                                        {
                                            r.Value = categ2.Name_C11;
                                            r.Merge();
                                            sheet.Cell(catRows, 1).Value = first + "|" + second;
                                            array.Add(categ2.Name_C11);
                                        }


                                        catRows++;
                                        foreach (var cat3 in finReport.Where(i => i.EmployeeId == item).OrderBy(i => i.Category11Id).ToList().Where(i => i.Category11Id == cat2).Select(i => i.Category111Id).Distinct())
                                        {
                                            var categ3 = _context.Category111s.FirstOrDefault(i => i.Category111Id == cat3);
                                            if (cat3 != null)
                                            {
                                                r = sheet.Range(catRows, 2, catRows, 5);

                                                if (array.Contains(categ3.Name_C111))
                                                {

                                                }
                                                else
                                                {
                                                    r.Value = categ3.Name_C111;
                                                    r.Merge();
                                                    sheet.Cell(catRows, 1).Value = (first + "|" + second + "|" + third);
                                                    array.Add(categ3.Name_C111);
                                                }
                                                foreach (var t in finReport.ToList().Where(l => l.EmployeeId == item && l.Category111Id == cat3 && l.StartDate.Year <= (task.TaskStarted.Year + i) && (task.TaskStarted.Year + i) <= l.EndDate.Year))
                                                {
                                                    int dif = t.EndDate.Year - t.StartDate.Year;
                                                    int year = task.TaskStarted.Year + i;
                                                    if (dif >= 1)
                                                    {
                                                        if (t.EndDate.Year == year)
                                                        {
                                                            if (row111 == 0 || row111 != catRows)
                                                            {
                                                                summ111 = 0;
                                                            }
                                                            row111 = catRows;
                                                            summ111 += t.amount;


                                                            //***********************************
                                                            sheet.Cell(catRows, 8 + i).Value = summ111;

                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (row111 == 0 || row111 != catRows)
                                                        {
                                                            summ111 = 0;
                                                        }
                                                        row111 = catRows;
                                                        summ111 += t.amount;

                                                        //***********************************************
                                                        sheet.Cell(catRows, 8 + i).Value = summ111;


                                                    }
                                                }

                                                catRows++;

                                                third++;
                                            }
                                            else
                                            {

                                                foreach (var t in finReport.ToList().Where(l => l.EmployeeId == item && l.Category11Id == cat2 && l.StartDate.Year <= (task.TaskStarted.Year + i) && (task.TaskStarted.Year + i) <= l.EndDate.Year))
                                                {
                                                    int dif = t.EndDate.Year - t.StartDate.Year;
                                                    int year = task.TaskStarted.Year + i;
                                                    if (dif >= 1)
                                                    {
                                                        if (t.EndDate.Year == year)
                                                        {
                                                            if (row11 == 0 || row11 != (catRows - 1))
                                                            {
                                                                summ11 = 0;
                                                            }
                                                            row11 = catRows - 1;
                                                            summ11 += t.amount;
                                                            //********************************
                                                            sheet.Cell(catRows - 1, 8 + i).Value = summ11;

                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (row11 == 0 || row11 != (catRows - 1))
                                                        {
                                                            summ11 = 0;
                                                        }
                                                        row11 = catRows - 1;
                                                        summ11 += t.amount;

                                                        //*********************************************
                                                        sheet.Cell(catRows - 1, 8 + i).Value = summ11;

                                                    }
                                                }

                                            }



                                        }
                                        second++;
                                    }
                                    else
                                    {
                                        foreach (var t in finReport.ToList().Where(l => l.EmployeeId == item && l.Category1Id == cat1 && l.StartDate.Year <= (task.TaskStarted.Year + i) && (task.TaskStarted.Year + i) <= l.EndDate.Year))
                                        {
                                            int dif = t.EndDate.Year - t.StartDate.Year;
                                            int year = task.TaskStarted.Year + i;
                                            if (dif >= 1)
                                            {
                                                if (t.EndDate.Year == year)
                                                {

                                                    if (row1 == 0 || row1 != (catRows - 1))
                                                    {
                                                        summ1 = 0;
                                                    }
                                                    row1 = catRows - 1;
                                                    summ1 += t.amount;

                                                    //********************************************************
                                                    sheet.Cell(catRows - 1, 8 + i).Value = summ1;

                                                }

                                            }
                                            else
                                            {
                                                if (row1 == 0 || row1 != (catRows - 1))
                                                {
                                                    summ1 = 0;
                                                }
                                                row1 = catRows - 1;
                                                summ1 += t.amount;
                                                //***********************************
                                                sheet.Cell(catRows - 1, 8 + i).Value = summ1;


                                            }
                                        }
                                    }


                                }
                                first++;
                            }
                            for (int j = 23; j < catRows; j++)
                            {

                                sheet.Cell(j, 6).Value = j - 22;
                                sheet.Cell(j, 7).SetFormulaR1C1("=SUM(R" + j + "C8:R" + j + "C" + (8 + totalYear) + ")");
                                string r = sheet.Cell(j, 1).Value.ToString();
                                //sheet.Cell(j, 1).Value.ToString();
                                if (r == null || r == "")
                                {
                                    sheet.Cell(j, yearI).Value = "";

                                }
                            }
                            sheet.Rows(20, 22).Style.Font.Bold = true;
                            sheet.Row(22).Height = 20;
                            sheet.Columns("B").Width = 50;
                            sheet.Columns("F").Width = 15;
                            sheet.Columns("G").Width = 18;



                            yearI++;
                            sheet.SetShowGridLines(true);
                            var Bb = sheet.Range(20, 1, catRows, yearI - 1);
                            Bb.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            Bb.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                        }
                        var range1 = sheet.Range(1, 1, 1, yearI - 1);
                        range1.Value = "Ўрганиш натижасида аниқланган молиявий бузилиш, хато ва камчиликлар тўғрисида";
                        range1.Merge();

                        var range2 = sheet.Range(2, 1, 2, yearI - 1);
                        range2.Value = "МАЪЛУМОТ";
                        range2.Merge();
                        range2.Style.Font.Bold = true;
                        range2.Style.Font.FontSize = 10;
                        range2.Style.Font.SetFontName("Arial Cyr");


                        var range15 = sheet.Range(20, 8, 21, yearI - 1);
                        range15.Value = "жумладан йиллар кесимда";
                        range15.Merge();
                        var range16 = sheet.Range("G4:H4");
                        range16.Value = name.User.FullName;
                        range16.Merge();


                    }
                }

                //SPRAVKA
                var sheet1 = workbook.Worksheets.Add("spravka");
                //sheet1.Column("A").;
                sheet1.Column("A").Style.ToString();
                #region 
                sheet1.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                sheet1.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                sheet1.Columns("B").Width = 30;
                var Range3 = sheet1.Range("A4:F4");
                Range3.Value = "Ўрганиш ўтказган Молиявий ва комплаенс назорати департаменти ходимининг исм шарифи :";
                Range3.Merge();
                Range3.Style.Font.Bold = true;
                Range3.Style.Font.Underline = XLFontUnderlineValues.Single;
                var Range4 = sheet1.Range("A6:B6");
                Range4.Value = "Ўрганиш ўтказилган ташкилотнинг номи:";
                Range4.Merge();
                Range4.Style.Font.FontSize = 10;
                Range4.Style.Font.SetFontName("Arial Cyr");
                Range4.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range4.Style.Font.Bold = true;
                Range4.Style.Font.Underline = XLFontUnderlineValues.Single;
                var Range444 = sheet1.Range("C6:E6");
                Range444.Value = addrr;
                Range444.Merge();
                var Range5 = sheet1.Range("A8:C8");
                Range5.Value = "Юқори органнинг номи:";
                Range5.Merge();
                Range5.Style.Font.FontSize = 10;
                Range5.Style.Font.SetFontName("Arial Cyr");
                Range5.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range5.Style.Font.Bold = true;
                Range5.Style.Font.Underline = XLFontUnderlineValues.Single;

                var Range6 = sheet1.Range("A10:C10");
                Range6.Value = "Ўрганиш ўтказишга асос: \"Ўзбекнефтгаз\" АЖ нинг";
                Range6.Merge();
                Range6.Style.Font.FontSize = 10;
                Range6.Style.Font.SetFontName("Arial Cyr");
                Range6.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range6.Style.Font.Bold = true;
                Range6.Style.Font.Underline = XLFontUnderlineValues.Single;

                var Range7 = sheet1.Range("A12:B12");
                Range7.Value = "Ўрганиш бошланган сана";
                Range7.Merge();
                Range7.Style.Font.FontSize = 10;
                Range7.Style.Font.SetFontName("Arial Cyr");
                Range7.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range7.Style.Font.Bold = true;
                Range7.Style.Font.Underline = XLFontUnderlineValues.Single;

                var Range8 = sheet1.Range("A14:B14");
                Range8.Value = "Текшириш якунланган сана";
                Range8.Merge();
                Range8.Style.Font.FontSize = 10;
                Range8.Style.Font.SetFontName("Arial Cyr");
                Range8.Style.Font.Bold = true;
                Range8.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range8.Style.Font.Underline = XLFontUnderlineValues.Single;

                var Range9 = sheet1.Range("A16:B16");
                Range9.Value = "Ўрганиш даври:";
                Range9.Merge();
                Range9.Style.Font.FontSize = 10;
                Range9.Style.Font.SetFontName("Arial Cyr");
                Range9.Style.Font.Bold = true;
                Range9.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range9.Style.Font.Underline = XLFontUnderlineValues.Single;

                var Range10 = sheet1.Range("A18:C18");
                Range10.Value = "Ўрганишга ҳақиқий сарфланган вақт:";
                Range10.Merge();
                Range10.Style.Font.FontSize = 10;
                Range10.Style.Font.SetFontName("Arial Cyr");
                Range10.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                Range10.Style.Font.Bold = true;
                Range10.Style.Font.Underline = XLFontUnderlineValues.Single;

                var Range11 = sheet1.Range("A20:A22");
                Range11.Value = "Т/р";
                Range11.Merge();
                var Range12 = sheet1.Range("B20:E22");
                Range12.Value = "Ёзув мазмуни";
                Range12.Merge();
                var Range13 = sheet1.Range("F20:F22");
                Range13.Value = "Қатор рақами";
                Range13.Merge();
                var Range14 = sheet1.Range("G20:G22");
                Range14.Value = "Жами суммаси";
                Range14.Merge();
                #endregion
                int YearI = 8;

                for (int i = 0; i <= totalYear; i++)
                {
                    sheet1.Cell(22, 8 + i).Value = (task.TaskStarted.Year + i) + " йил";
                    sheet1.Column(8 + i).Width = 13;


                    int row1 = 0;
                    int row11 = 0;  
                    int first = 1;
                    
                    int catRows = 23;

                    foreach (var cat1 in _context.Category1s.ToList().OrderBy(i => i.Category1Id))
                    {
                        var r = sheet1.Range(catRows, 2, catRows, 5);
                        r.Value = cat1.Name_C1;
                        r.Merge();
                        row1 = catRows;
                        sheet1.Cell(catRows, 1).Value = first;
                        catRows++;
                        int second = 1;
                        List<int> cellIn = new List<int>();
                        foreach (var cat2 in _context.Category11s.ToList().Where(i => i.Category1Id == cat1.Category1Id).OrderBy(i => i.Category11Id))
                        {
                            if (cat2 != null)
                            {
                                //int row = catRows;
                                r = sheet1.Range(catRows, 2, catRows, 5);
                                r.Value = cat2.Name_C11;
                                cellIn.Add(catRows);
                                r.Merge();
                                sheet1.Cell(catRows, 1).Value = first + "l" + second;
                                row11 = catRows;
                                catRows++;
                                int third = 1;
                                foreach (var cat3 in _context.Category111s.ToList().Where(i => i.Category11Id == cat2.Category11Id).OrderBy(i => i.Category111Id))
                                {
                                    if (cat3 != null)
                                    {
                                        r = sheet1.Range(catRows, 2, catRows, 5);
                                        r.Value = cat3.Name_C111;
                                        r.Merge();
                                        sheet1.Cell(catRows, 1).Value = first + "l" + second + "l" + third;
                                        catRows++;

                                        int cCat11 = _context.Category111s.ToList().Where(i => i.Category11Id == cat2.Category11Id).Count();
                                        sheet1.Cell(row11, 8 + i).FormulaR1C1 = "=SUM(R" + (row11 + 1) + "C" + (8 + i) + ":R" + (row11 + cCat11) + "C" + (8 + i) + ")";

                                    }
                                    third++;
                                }
                            }
                            second++;


                        }
                        //*********************************************


                        //for (int ijk = 0; ijk < cellIn.Count(); ijk++)
                        //{
                        //double sum = (double)sheet1.Cell(row1, 8 + i).Value;
                        for (int ijk = cellIn.Count(); ijk <= 10; ijk++)
                        {
                            cellIn.Add(19);
                        }
                        sheet1.Cell(row1, 8 + i).FormulaR1C1 = "=(R" + cellIn[0] + "C" + (8 + i) + "+R" + cellIn[1] + "C" + (8 + i) + "+R" + cellIn[2] + "C" + (8 + i)
                        + "+R" + cellIn[3] + "C" + (8 + i) + "+R" + cellIn[4] + "C" + (8 + i) + "+R" + cellIn[5] + "C" + (8 + i) + "+R" + cellIn[6] + "C" + (8 + i) + "+R" + cellIn[7] + "C" + (8 + i) + "+R" + cellIn[8] + "C" + (8 + i)
                        + "+R" + cellIn[9] + "C" + (8 + i) + "+R" + cellIn[10] + "C" + (8 + i) + ")";

                        //}
                        first++;
                    }
                    sheet1.Rows(20, 22).Style.Font.Bold = true;
                    sheet1.Row(22).Height = 20;
                    sheet1.Columns("B").Width = 50;
                    sheet1.Columns("F").Width = 15;
                    sheet1.Columns("G").Width = 18;

                    for (int j = 23; j < catRows; j++)
                    {
                        sheet1.Cell(j, 6).Value = j - 22;
                        sheet1.Cell(j, 7).SetFormulaR1C1("=SUM(R" + j + "C8:R" + j + "C" + (8 + totalYear) + ")");

                    }

                    YearI++;
                    sheet1.SetShowGridLines(true);
                    var Bb = sheet1.Range(20, 1, catRows, YearI - 1);
                    Bb.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                    Bb.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                    //var nnnn1 = sheet1.Range(23, 4, catRows, 7);
                    //nnnn1.Style.NumberFormat.SetFormat("#,##0.00");
                    sheet1.Columns("G:L").Style.NumberFormat.SetFormat("#,##0.00");
                    
                }

                var Range1 = sheet1.Range(1, 1, 1, YearI - 1);
                Range1.Value = "Ўрганиш натижасида аниқланган молиявий бузилиш, хато ва камчиликлар тўғрисида";
                Range1.Merge();

                var Range2 = sheet1.Range(2, 1, 2, YearI - 1);
                Range2.Value = "МАЪЛУМОТ";
                Range2.Merge();
                Range2.Style.Font.Bold = true;
                Range2.Style.Font.FontSize = 10;
                Range2.Style.Font.SetFontName("Arial Cyr");


                var Range15 = sheet1.Range(20, 8, 21, YearI - 1);
                Range15.Value = "жумладан йиллар кесимда";
                Range15.Merge();


                #region

                // hisobot
                var jadval = workbook.Worksheets.Add("Хисобот жадвали");
                jadval.Row(6).Style.NumberFormat.SetFormat("#,##0.00");
                var tashkilot = jadval.Range("A4:A5");
                tashkilot.Value = "Ташкилот номи";
                tashkilot.Merge();
                tashkilot.Style.Alignment.WrapText = true;
                tashkilot.Style.Font.Bold = true;
                jadval.Column("A").Width = 40;
                jadval.Cell("A6").Value = "";
                var tekshiruvDavri = jadval.Range("B4:B5");
                tekshiruvDavri.Value = "Ўтказилган текшириш даври";
                tekshiruvDavri.Merge();
                tekshiruvDavri.Style.Alignment.WrapText = true;
                tekshiruvDavri.Style.Font.Bold = true;
                jadval.Column("B").Width = 25;
                jadval.Cell("A6").FormulaA1 = "=spravka!C6";
                jadval.Cell("A6").Style.Alignment.WrapText = true;
                if(totalYear > 0)
                {
                    jadval.Cell("B6").Value =  task.TaskStarted.Year + "й - " + task.TaskEnd.Year + "й";

                }
                else
                {
                    jadval.Cell("B6").Value = task.TaskStarted.ToLongDateString() + " - "  + task.TaskEnd.ToLongDateString();
                }
                jadval.Cell("B6").Style.Alignment.WrapText = true;
                int c = 0;
                int cc = 0;
                int plus = 0;
                int plus1 = 0;
                
                foreach (var cy1 in _context.Category1s.ToList().OrderBy(i => i.Category1Id))
                {
                    var cccc = jadval.Range(4, c + 3, 5, c + 3);
                    cccc.Value = cy1.Name_C1;
                    cccc.Merge();
                    cccc.Style.Alignment.WrapText = true;
                    cccc.Style.Font.Bold = true;
                    jadval.Column(c + 3).Width = 25;
                    int fa = 23 + c + plus; 
                    var temp = _context.Category11s.ToList().Where(i => i.Category1Id == cy1.Category1Id);
                    jadval.Cell(6, c + 3).FormulaR1C1 = "=spravka!R" + fa + "C7";
                    c++;
                    
                    if (temp != null && temp.Any())
                    {
                        plus1 = plus;
                        cc = c+3;
                        foreach(var cy2 in _context.Category11s.ToList().OrderBy(i => i.Category11Id).Where(i => i.Category1Id == cy1.Category1Id))
                        {

                            jadval.Cell(5, c + 3).Value = cy2.Name_C11;
                            jadval.Cell(5, c + 3).Style.Alignment.WrapText = true;
                            jadval.Cell(5, c + 3).Style.Font.Bold = true;
                            jadval.Column(c + 3).Width = 25;
                            int fa1 = 23 + c + plus1;
                            var temp1 = _context.Category111s.ToList().Where(i => i.Category11Id == cy2.Category11Id);
                            jadval.Cell(6, c + 3).FormulaR1C1 = "=spravka!R" + fa1 + "C7";
                            plus1 += temp1.Count();
                            //if (temp1.Any())
                            //{
                            //    check = 1;
                            //}
                            c++;
                            
                           
                        }

                        var shundan = jadval.Range(4, cc, 4, c + 2);
                        shundan.Value = "Шундан:";
                        shundan.Merge();
                        shundan.Style.Font.Bold = true;
                    }
                   
                        plus = plus1;
                     
                   
                    
                }
                var huquqniMuh = jadval.Range(4, c + 3, 5, c + 3);
                huquqniMuh.Value = "Тастиф";
                huquqniMuh.Merge();
                jadval.Cell(6, c + 3).Value = task.Comment;
                jadval.Cell(6, c + 3).Style.Alignment.WrapText = true;
                huquqniMuh.Style.Alignment.WrapText = true;
                huquqniMuh.Style.Font.Bold = true;
                jadval.Column(c + 3).Width = 25;

               
               
                jadval.Row(6).Height = 50;
                jadval.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                jadval.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                jadval.SetShowGridLines(true);
                var Bbjad = jadval.Range(4, 1, 6, c+3);
                Bbjad.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                Bbjad.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                var title = jadval.Range(1, 3, 1, c + 3);
                title.Value = "Молиявия ва комплаенс назорати департаменти томонидан \"Ўзбекнефтгаз\" АЖ тизимидаги корхона ва ташкилотларда 2020 йил якунига қадар " +
                    "ўтказилган ўрганишлар таҳлиллар натижасида аниқланган молиявий қонун бузилиши ҳолатлари ва амалга оширилган ишлар бўйича тезкор";
                title.Merge();
                title.Style.Alignment.WrapText = true;
                title.Style.Font.Bold = true;
                
                title.Style.Font.FontSize = 16;
                var title1 = jadval.Range(2, 3, 2, c + 3);
                title1.Value = "Маълумот";
                title1.Merge();
                title1.Style.Alignment.WrapText = true;
                title1.Style.Font.Bold = true;
                title1.Style.Font.FontSize = 16;
                jadval.Style.Font.SetFontName("Arial"); 
                jadval.SheetView.Freeze(5, 2);

                #endregion

                var sheet4 = workbook.Worksheets.Add("Хисобот");
                var hisobot1 = sheet4.Range("J1:L1");
                hisobot1.Value = "“Ўзбекнефтгаз” АЖ";
                hisobot1.Merge();
                
                sheet4.Rows(1, 5).Style.Font.Bold = true;
                
                var hisobot2 = sheet4.Range("J2:L2");
                hisobot2.Value = "Бошқарув раиси";
                hisobot2.Merge();

                var hisobot3 = sheet4.Range("J3:L3");
                hisobot3.Value = "М.Р. Абдуллаевга";
                hisobot3.Merge();

                #region
                var hisobot4 = sheet4.Range("A5:L5");
                hisobot4.Value = "Ҳурматли Мехриддин Раззоқович!";
                hisobot4.Merge();

                var hisobot5 = sheet4.Range("A7:L7");
                hisobot5.Value = "Молиявий ва комплаенс назорати департаменти томонидан жорий йилнинг ўтган даврида қуйидаги ишлар амалга оширилди.";
                hisobot5.Merge();

                var hisobot6 = sheet4.Range("A8:L8");
                hisobot6.Value = "Корруцияга қарши “комплаенс-назорат” тизимини белгиланган муддатларда ва самарали тарзда  ташкил этилишини таъминлаш борасида:";
                hisobot6.Merge();
                hisobot6.Style.Font.Bold = true;

                var hisobot7 = sheet4.Range("A9:L9");
                hisobot7.Value = "Ўзбекистон Республикаси Президентининг 2020 йил 4 апрелидаги ПҚ-4664-сон «Нефт-газ тармоғининг молиявий " +
                    "барқарорлигини ошириш бўйича биринчи навбатдаги чора-тадбирлар тўғрисида»ги Қарорининг 2-бандига асосан " +
                    "2020 йил 1 октябрга қадар нефть-газ тармоғидаги барча корхоналарда коррупцияга қарши «комплаенс-назорат» тизими жорий этилиши белгиланган.";
                hisobot7.Merge();
                hisobot7.Style.Alignment.WrapText = true;
                var hisobot8 = sheet4.Range("A10:L10");
                hisobot8.Value = "Жорий йил бошидан БМТ Тараққиёт дастурининг “Ўзбекистонда самарали, ҳисоб берувчи ва шаффоф бошқарув институтлари орқали коррупцияга қарши курашиш лойиҳаси доирасида" +
                    " “KPMG” халқаро аудиторлик консалтинг компанияси экспертлари ҳамкорлигида комплаенс тизимини жорий этилиши юзасидан 4 босқичда ишлар ташкил этилди.";
                hisobot8.Merge();

                sheet4.Rows(1, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                sheet4.Column("A").Width = 5;
                for(int i =1; i<= 19; i++)
                {
                    sheet4.Cell(11+i,1).Value = i;
                }
               
                var hisobot9 = sheet4.Range("B12:L12");
                hisobot9.Value = "Методология по идентификации и оценке коррупционных рисков";
                hisobot9.Merge();
                

                var hisobot10 = sheet4.Range("B13:L13");
                hisobot10.Value = "Политика АО «Узбекнефтегаз» по противодействию коррупции";
                hisobot10.Merge();

                var hisobot11 = sheet4.Range("B14:L14");
                hisobot11.Value = "Регламент формирования и предоставления отчетности  антикоррупционной комплаенс системы «Узбекнефтегаз   ";
                hisobot11.Merge();

                var hisobot12 = sheet4.Range("B15:L15");
                hisobot12.Value = "Методика мониторинга и контроля эффективности антикоррупционных процедур АО «Узбекнефтегаз»";
                hisobot12.Merge();

                var hisobot13 = sheet4.Range("B16:L16");
                hisobot13.Value = "Методические указания по организации антикоррупционного обучения работников АО «Узбекнефтегаз»";
                hisobot13.Merge();

                var hisobot14 = sheet4.Range("B17:L17");
                hisobot14.Value = "Положение о деловых мероприятиях и представительских расходах АО «Узбекнефтегаз»";
                hisobot14.Merge();

                var hisobot15 = sheet4.Range("B18:L18");
                hisobot15.Value = "Кодекс корпоративной этики АО «Узбекнефтегаз»";
                hisobot15.Merge();

                var hisobot16 = sheet4.Range("B19:L19");
                hisobot16.Value = "Рекомендации в отношении ключевых показателей эффективности сотрудников в сфере противодействия коррупции АО «Узбенефтегаз»";
                hisobot16.Merge();

                var hisobot17 = sheet4.Range("B20:L20");
                hisobot17.Value = "Инструкция по проверке контрагентов АО «Узбекнефтегаз»";
                hisobot17.Merge();

                var hisobot18 = sheet4.Range("B21:L21");
                hisobot18.Value = "Положение по управлению конфликтом интересов в АО «Узбекнефтегаз»";
                hisobot18.Merge();

                var hisobot19 = sheet4.Range("B22:L22");
                hisobot19.Value = "Положение О деловых подарках и знаках делового гостеприимства АО «Узбекнефтегаз»";
                hisobot19.Merge();

                var hisobot20 = sheet4.Range("B23:L23");
                hisobot20.Value = "Регламент приема и обработки сообщений, поступивших по каналам связи АО «Узбекнефтегаз»";
                hisobot20.Merge();

                var hisobot21 = sheet4.Range("B24:L24");
                hisobot21.Value = "Положение об оказании благотворительной и спонсорской помощи АО «Узбекнефтегаз»";
                hisobot21.Merge();

                var hisobot23 = sheet4.Range("B25:L25");
                hisobot23.Value = "Регламент Проведения служебных расследований АО «Узбекнефтегаз»";
                hisobot23.Merge();

                var hisobot24 = sheet4.Range("B26:L26");
                hisobot24.Value = "Инструкция по проверке кандидатов на работу АО «Узбекнефтегаз»";
                hisobot24.Merge();

                var hisobot25 = sheet4.Range("B27:L27");
                hisobot25.Value = "Правки в резюме кандидата";
                hisobot25.Merge();

                var hisobot26 = sheet4.Range("B28:L28");
                hisobot26.Value = "Положение Об отделе антикоррупционного комплаенс АО «Узбекнефтегаз»";
                hisobot26.Merge();

                var hisobot27 = sheet4.Range("B29:L29");
                hisobot27.Value = "Положение о комиссии по этике АО «Узбекнефтеrаз»";
                hisobot27.Merge();

                var hisobot28 = sheet4.Range("B30:L30");
                hisobot28.Value = "Антикоррупционная оговорка для включения в трудовые договоры с работниками АО «Узбекнефтегаз»";
                hisobot28.Merge();
               

                var hisobot29 = sheet4.Range("A32:L32");
                hisobot29.Value = "Молиявий назорат йўналишида ташкилотлар бўйича амалга оширилган ўрганиш ва таҳлиллар: ";
                hisobot29.Merge();
                hisobot29.Style.Font.Bold = true;

                var hisobot30 = sheet4.Range("A34:L34");
                hisobot30.Value = "“Ўзбекнефтгаз” АЖ да 2020 йил 27 майдаги йиғилиш баёнига мувофиқ " + addrr + " нинг фаолияти самарадорлигини ошириш, тўлов интизомини мустаҳкамлаш, ";
                hisobot30.Merge();
                
                var hisobot31 = sheet4.Range("A35:L35");
                hisobot31.Value = "дебитор ва кредитор қарзларни камайтириш ҳамда  тизимда турли талон тарожликларни олдини";
                hisobot31.Merge();

                var hisobot32 = sheet4.Range("A36:L36");
                hisobot32.Value = " олиш масалаларида амалий ёрдам кўрсатиш юзасидан ўрганиш ўтказилди.";
                hisobot32.Merge();
                #endregion
                int sss44 = 37;
                int last = 3;
                foreach (var cy1 in _context.Category1s.ToList().OrderBy(i => i.Category1Id))
                {

                    var r4444 = sheet4.Range(sss44, 2, sss44, 10);
                    r4444.Value = cy1.Name_C1;
                    r4444.Merge();
                    sheet4.Cell(sss44, 11).FormulaR1C1 = "=\'Хисобот жадвали\'!R6C" + last;
                    sheet4.Cell(sss44, 12).Value = "cум";
                    var temp = _context.Category11s.ToList().Where(i => i.Category1Id == cy1.Category1Id);
                    sss44++;
                    last++;
                    if(temp != null && temp.Any())
                    {
                        sheet4.Cell(sss44-1, 12).Value = "cум, Шундан";
                        //int insideC = last;
                        foreach (var cy2 in _context.Category11s.ToList().Where(i => i.Category1Id==cy1.Category1Id))
                        {
                            var r44441 = sheet4.Range(sss44, 2, sss44, 10);
                            r44441.Value = "- " + cy2.Name_C11;
                            r44441.Merge();

                            sheet4.Cell(sss44, 11).FormulaR1C1 = "=\'Хисобот жадвали\'!R6C" + last;
                            sheet4.Cell(sss44, 12).Value = "cум";
                            sss44++;
                            last++;
                        }
                    }
                }

                sheet4.Column("K").Style.NumberFormat.SetFormat("#,##0.00");
                sheet4.Rows(1, 40).Style.Alignment.WrapText= true;
              
                
               
                using (var stream = new MemoryStream())
                {   
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       //"application/vnd.ms-excel.sheet.macroEnabled.12",
                        "Spravka.xlsx"
                        );
                }
            }

        }



        [HttpGet]
        public async Task<IActionResult> Download_Config(int? FileId)
        {
            if (FileId == null) { return Content("Файл мавжуд эмас"); }
                
            var file = await _context.FileHistory.FirstOrDefaultAsync(w => w.FileId == FileId);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", file.FileUrl);
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch
            {
                return View("~/Views/Shared/_NotFound.cshtml", file.FileUrl);
            }

           

        }
        private string GetContentType(string path)
        {

            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain" },
                {".pdf", "application/pdf" },
                {".doc", "application/vnd.ms-word" },
                {".docx", "application/vnd.ms-word" },
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
        }

        // GET: Employee/Tasks
        public async Task<IActionResult> Index()
        {
            var EmpId = _context.Employees.Include(u => u.User).FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            var DepId = _context.Employees.Include(u => u.User).ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).User.DepartmentId;
            var applicationDbContext =_context.Tasks.Include(u => u.Employee).ThenInclude(u => u.User).Include(t => t.Department).Include(t => t.Task_Type)
                .Include(t => t.Task_Files).ThenInclude(t => t.FileHistory).Include(i => i.Task_Emples).ThenInclude(u => u.Employee)
                .ThenInclude(u => u.User).Where(r => r.DepartmentId == DepId && (r.Task_Emples.ToList().Select(i => i.TaskId).Count()==0 || r.Task_Emples.ToList().Select(i => i.EmplId).Contains(EmpId))).OrderByDescending(t => t.TaskId); 
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Index2()
        {
            var EmpId =  _context.Employees.Include(u => u.User).FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var DepId = _context.Employees.Include(t => t.User).ThenInclude(d => d.Department).FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).User.Department.DepartmentId;
            
            var applicationDbContext =await _context.Tasks.Include(u => u.Employee).ThenInclude(u => u.User)
                .Include(t => t.Department)
                .Include(t => t.Task_Type)
                .Include(t => t.Task_Files).ThenInclude(t => t.FileHistory)
                .Include(i => i.Task_Emples).ThenInclude(u => u.Employee).ThenInclude(u => u.User).Where(r => r.DepartmentId == DepId).ToListAsync();
            foreach(var i in applicationDbContext)
            {
                if (i.Task_Emples.Count() > 0)
                {
                    applicationDbContext = applicationDbContext.Where(r => r.Task_Emples.ToList().Select(i => i.EmplId).Contains(EmpId)).ToList();
                    
                }
                
            }
            return View(applicationDbContext.OrderByDescending(t => t.TaskId));
        }
        // GET: Employee/Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.Include(u => u.Employee).ThenInclude(u => u.User).Include(u => u.Task_Emples).ThenInclude(u => u.Employee).ThenInclude(u => u.User)
                .Include(t => t.Department)
                .Include(t => t.Task_Type)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", tasks);
        }

        // GET: Employee/Tasks/Create
        public IActionResult Create()
        {
            var ManId = _context.Employees.Include(m => m.Manager).ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).Manager.ManagerId;

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.FileHistory.Include(w => w.Task_Files).ThenInclude(w => w.Tasks).Where(w => w.FileFinished == false))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.Name + " #" + item.FileUrl,
                    Value = item.FileId.ToString(),

                };
                SelectListItem sel = selectListItem;
                items.Add(sel);
            }
            ViewBag.File = items;

            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType");
            return View();
        }

        // POST: Employee/Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,TaskName,SumLost,SumGain,Comment,File,Finished,TaskStarted,TaskEnd,DepartmentId,TaskTypeId")] Tasks tasks, [Bind("Task_FileId,TaskId,FileId")] Task_File task_file, [Bind("Task_EmpId", "TaskId", "EmplId")] Task_Empl task_emp, List<int> ListofFiles, List<int> ListofEmp)
        {
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            tasks.DepartmentId = DepId;

            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                foreach (var item in ListofFiles)
                {
                    Task_File ttt = new Task_File()
                    {
                        TaskId = tasks.TaskId,
                        FileId = item
                    };

                    _context.Task_Files.Add(ttt);
                    await _context.SaveChangesAsync();

                }
                foreach (var i in ListofEmp)
                {
                    Task_Empl eT = new Task_Empl()
                    {
                        TaskId = tasks.TaskId,
                        EmplId = i
                    };
                    _context.Task_Empls.Add(eT);
                    await _context.SaveChangesAsync();
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.FileHistory.Include(w => w.Task_Files).ThenInclude(w => w.Tasks).Where(w => w.FileFinished == false))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.Name + " #" + item.FileUrl,
                    Value = item.FileId.ToString(),

                };
                SelectListItem sel = selectListItem;
                items.Add(sel);
            }
            ViewBag.File = items;

            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }

        // GET: Employee/Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;

            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }

            var a = _context.Task_Files.Where(i => i.TaskId == id).ToList().Select(i => i.FileId);
            var enm = _context.Task_Empls.Where(i => i.TaskId == id).ToList().Select(i => i.EmplId);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.FileHistory.Include(w => w.Task_Files).ThenInclude(w => w.Tasks).Where(w => w.FileFinished == false))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.Name + " #" + item.FileUrl,
                    Value = item.FileId.ToString(),


                };
                SelectListItem sel = selectListItem;

                items.Add(sel);
            }
            ViewBag.File = items;

            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;

            tasks.selectedFiles = new List<int> { };
            foreach (int i in a.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedFiles.Add(i);
                }
            }

            tasks.selectedEmployees = new List<int> { };
            foreach (int i in enm.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedEmployees.Add(i);
                }
            }
            ViewBag.FileSelected = _context.FileHistory.Include(t => t.Task_Files).ThenInclude(i => i.Tasks).ToList().Select(i => i.Name);
            ViewBag.EmployeeSelected = _context.Employees.Include(u => u.User).Include(t => t.Task_Emples).ThenInclude(i => i.Tasks).ToList().Select(i => i.User.FullName);

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }

        // POST: Employee/Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskName,SumLost,SumGain,Comment,AllEmpl,File,Finished,TaskStarted,TaskEnd,DepartmentId,MasulEmplId,TaskTypeId")] Tasks tasks, List<int> ListofFiles, List<int> ListofEmpl, IFormFile file)
        {
            string prevFile = (await _context.Tasks.AsNoTracking().ToListAsync()).FirstOrDefault(i => i.TaskId == tasks.TaskId).File;
           
            if(prevFile != null && file != null)
            {
                if (System.IO.File.Exists(Path.Combine("wwwroot/files/end", prevFile)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Path.Combine("wwwroot/files/end", prevFile));
                }
            }
            
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;

            if (id != tasks.TaskId)
            {
                return NotFound();
            }
            if (file != null)
            {
                string type = Path.GetExtension(file.FileName);
                if ((type != ".docx") && (type != ".doc") && (type != ".pdf") && (type != ".xlsm") && (type != ".xlsx"))
                    return Content("Нотоғри файл тури танланди");
                //return View("~/Views/Shared/_UnsupportedMediatype.cshtml");
                tasks.File = file.FileName;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var task_file = _context.Task_Files.Where(w => w.TaskId == id).ToList();
                    if (task_file != null)
                    {
                        foreach (var i in task_file)
                        {
                            _context.Task_Files.Remove(i);
                        }
                    }

                    var task_emp = _context.Task_Empls.Where(w => w.TaskId == id).ToList();
                    if (task_emp != null)
                    {
                        foreach (var i in task_emp)
                        {
                            _context.Task_Empls.Remove(i);
                        }
                    }

                    foreach (var item in ListofFiles)
                    {
                        Task_File ttt = new Task_File()
                        {
                            TaskId = tasks.TaskId,
                            FileId = item
                        };

                        _context.Task_Files.Add(ttt);
                        await _context.SaveChangesAsync();

                    }

                    foreach (var it in ListofEmpl)
                    {
                        Task_Empl enT = new Task_Empl()
                        {
                            TaskId = tasks.TaskId,
                            EmplId = it,
                        };
                        _context.Task_Empls.Add(enT);
                        await _context.SaveChangesAsync();
                    }
                    if (file != null)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/end", file.FileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.FileHistory.Include(w => w.Task_Files).ThenInclude(w => w.Tasks).Where(w => w.FileFinished == false))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.Name + " #" + item.FileUrl,
                    Value = item.FileId.ToString(),

                };
                SelectListItem sel = selectListItem;
                items.Add(sel);
            }
            ViewBag.File = items;

            var a = _context.Task_Files.Where(i => i.TaskId == id).ToList().Select(i => i.FileId);
            var enm = _context.Task_Empls.Where(i => i.TaskId == id).ToList().Select(i => i.EmplId);

            tasks.selectedFiles = new List<int> { };
            foreach (int i in a.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedFiles.Add(i);
                }
            }
            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;


            tasks.selectedEmployees = new List<int> { };
            foreach (int i in enm.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedEmployees.Add(i);
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "TaskTypeID", tasks.TaskTypeId);
            return View(tasks);
        }

        // GET: Employee/Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Department)
                .Include(t => t.Task_Type)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Employee/Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
        [HttpGet]
        public async Task<IActionResult> Download_ConfigEnd(int? TaskId)
        {
            if (TaskId == null)
                return Content("Файл мавжуд эмас");
            var file = await _context.Tasks.FirstOrDefaultAsync(w => w.TaskId == TaskId);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/end", file.File);
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch
            {
                return View("~/Views/Shared/_NotFound.cshtml", file.File);
            }



        }
      

    }
}
