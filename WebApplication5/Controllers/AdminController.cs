﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication5.Models;
using System.DirectoryServices;
using Microsoft.EntityFrameworkCore;

namespace WebApplication5.Controllers
{
    public class AdminController : Controller
    {
        //public ImportUsersFromAdExecute()
        [HttpPost]
        public IActionResult ClickButton()
        {
            //ImportUsersFromAdExecute();
            ViewData["Logs"] = "След. пользователи были импортированы:";
            return View("AdminMain12");
        }

        List<User> usersNewList;
        public IActionResult ImportUsersFromAD()
        {
            usersNewList = new List<User>();
            List<User> usersUpdateList = new List<User>();
            var usersFromAD = GetUsersFromAD();
            CompareUsers(usersFromAD, ref usersNewList, ref usersUpdateList); // Можно обработать обновление пользователей, также вывести пользователю для подтверждения
            //context.SaveChanges();
            //ImportUsersFromAdExecute();      

            return View(usersNewList);
        }

        

        public IActionResult ImportUsersPost(List<User> users)
        {
            var userListRet= users.Where(x => x.NeedToImport == false).ToList();
            var userList = users.Where(x => x.NeedToImport == true);
            foreach (var user in userList)
            {
                if (user.Department.Id != 0)
                {
                    var departId = user.Department.Id;
                    user.Department = context.Departments.Where(x => x.Id == user.Department.Id).First();
                }
                else
                {
                    user.Department = null;
                }
                string[] fullNameSplited= user.FullName.Split(' ');
                user.LastName = fullNameSplited[0];
                user.FirstName = fullNameSplited[1];
                user.MiddleName = fullNameSplited[2];
                context.Users.Add(user); //Add department
            }
            context.SaveChanges();
            ViewData["Information"] = String.Format("Импортировано пользователей - {0}", userList.Count() );
            return View("ImportUsersFromAD", userListRet);
        }

        [HttpGet]
        public IActionResult AdminMain()
        {
            return View("AdminMain");
        }

        AppDbContext context;
        public AdminController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        public IActionResult DropDownList()
        {
            return PartialView(context.Users);
        }

        public IActionResult CreateNewProject()
        {
            ViewData["ActiveProjects"] = context.ProjectSet.Where(x => x.ShowInMenuBar == true); //new Project() { InternalNum = "2603" }; 
                                                                                                 //var user = new User() { LastName = "Ведерникова", FirstName = "Альбина", FullName = "Ведерникова Альбина" };
                                                                                                 // var userList = new List<User>();
                                                                                               //userList.Add(user);
            if (context.Users.Count() == 0) AddDefaultUsers(context);
            ViewData["Users"] = context.Users.ToList();//userList;
            
            return View();
        }

        public List<Statuses> GetStatuses()
        {
            var statuses = new List<Statuses>();
            statuses.Add(new Statuses() { Id = 0, StatusName = "Новое" });
            statuses.Add(new Statuses() { Id = 1, StatusName = "Исправлено исполнителем" });
            statuses.Add(new Statuses() { Id = 2, StatusName = "Проверено BIM-координатором" });
            statuses.Add(new Statuses() { Id = 3, StatusName = "Снято" });
            statuses.Add(new Statuses() { Id = 4, StatusName = "Повторное" });
            return statuses;
        }

        public IActionResult DeleteProjects()
        {
            var listStat = GetStatuses();
            SelectList selectListItems = new SelectList(listStat, "Id", "StatusName", listStat[1]);
            ViewBag.Statuses = selectListItems;
            ViewData["ActiveProjects"] = context.ProjectSet.Where(x => x.ShowInMenuBar == true);
            ViewData["Context"] = context;
            var projectSet = context.ProjectSet.Where(x => x.IsDeleted == false).Include(x => x.Manager).ToList();
            return View(projectSet);
        }

       
       public IActionResult DeleteSelectedProject(int? Id)
        {
           context.ProjectSet.First(x => x.Id == Id).IsDeleted=true;
            context.SaveChanges();
            var projectSet= context.ProjectSet.Where(x => x.IsDeleted == false).ToList();
            return View("DeleteProjects", projectSet);
        }

        public void AddDefaultUsers(AppDbContext appDbContext)
        {
            List<User> userList = new List<User>()
            {
                new User(){FirstName ="Альбина", LastName="Ведерникова", FullName="Альбина Ведерникова" },
                 new User(){FirstName ="Тимофей", LastName="Беликов", FullName="Тимофей Беликов" }
            };
            appDbContext.Users.AddRange(userList);
            appDbContext.SaveChanges();
        }

        [HttpPost]
        public IActionResult CreateNewProject(Project project)
        {
            // var selUserStr = (string)selectedUser;
            string selectedUser = Request.Form["selectedUser"].ToString();
            ViewData["Users"] = context.Users.ToList();
            if (ModelState.IsValid)
            {
                var user= context.Users.Where(x => x.FullName == selectedUser).First();
                project.Manager = user;
                context.ProjectSet.Add(project);
                context.SaveChanges();
                return RedirectToAction("Index", "Home");//(@"~/Views/Home/Index.cshtml", context);
            }
            else
            {
                return View();
            }
        }

        private List<User> GetUsersFromAD()
        {
            string userGrName = "OU=Users";
            var root = new DirectoryEntry("LDAP://" + "oilpro");
            var department = root.Children.Find("OU=Departments");
            List<DirectoryEntry> listGrUsers = new List<DirectoryEntry>();
            foreach (var depart in department.Children)
            {
                var dep = depart as DirectoryEntry;
                var userGrFound = GetGroupFromDepart(dep, userGrName);
                if (userGrFound != null)
                {
                    listGrUsers.Add(userGrFound);
                }
            }
            return GetUserNames(listGrUsers);
        }

            public DirectoryEntry GetGroupFromDepart(DirectoryEntry depart, string groupName)
        {
            var depChildList = depart.ChildrenToList();
            if (depChildList.Count > 0)
            {
                var listFound = depChildList.Where(x => x.Name == groupName);
                if (listFound.Count() == 0)
                {
                    foreach (var dep in depChildList)
                    {
                        var d = dep as DirectoryEntry;
                        GetGroupFromDepart(d, groupName);
                    }
                }
                else
                {
                    return listFound.First();
                }
            }
            return null;
        }

        public List<User> GetUserNames(List<DirectoryEntry> listGrUsers)
        {
            List<User> userList = new List<User>();
            foreach (var userGr in listGrUsers)
            {
                foreach (var user in userGr.Children)
                {
                    var us = user as DirectoryEntry;
                    var fullName = GetProperty("displayName", us);
                    var fullNameSplited = fullName.Split(' ').ToList();
                    if (fullNameSplited.Count == 3)
                    {
                        userList.Add(new User()
                        {
                            LastName = fullNameSplited[0],
                            FirstName = fullNameSplited[1],
                            MiddleName = fullNameSplited[2],
                            AD_GUID = us.NativeGuid,
                            Email = GetProperty("mail", us),
                            Login = GetProperty("mailNickname", us),
                            Department = FindOrCreateDepartment(GetProperty("department", us)),                            
                            FullName = String.Format("{0} {1} {2}", fullNameSplited[0], fullNameSplited[1], fullNameSplited[2])
                        }) ;
                    }
                }
            }
            return userList;
        }

        public Department FindOrCreateDepartment(string departmentName)
        {
           var departSet= context.Departments.Where(x => x.Name == departmentName);
            if (departSet.Count() > 0)
            {
                return departSet.First();
            }
            else
            {
                Department newDepart = new Department() { Name = departmentName };
                context.Departments.Add(newDepart);
                context.SaveChanges();
                return newDepart;
            }
        }

        public string GetProperty(string propName, DirectoryEntry directoryEntry)
        {
            try
            {
                var propert = directoryEntry.Properties[propName];
                return propert.Value as string;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void CompareUsers(List<User> usersFromAD, ref List<User> usersNewList, ref List<User> usersUpdateList)
        {
            foreach (var userFromAD in usersFromAD)
            {
                var userSet = context.Users.Where(x =>x.AD_GUID == userFromAD.AD_GUID);
                if (userSet.Count() > 0)
                {
                    var userFromDB = userSet.First();
                    CompareUpdateAllProps(ref userFromDB, userFromAD);
                }
                else
                {
                    usersNewList.Add(userFromAD);
                }
            }
        }

        public void CompareUpdateAllProps(ref User userFromDB, User userFromAD)
        {
            if (userFromDB.FirstName != userFromAD.FirstName)
            {
                userFromDB.FirstName = userFromAD.LastName;
            }
                 // Остальные свойства      

        }



    }
}