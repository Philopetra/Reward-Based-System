﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RYT.Commons;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.Enums;
using RYT.Models.ViewModels;
using RYT.Services.CloudinaryService;
using RYT.Services.Emailing;
using RYT.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IRepository _repository;
        private readonly IPhotoService _photoService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public DashboardController(IRepository repository, UserManager<AppUser> userManager, IPhotoService photoService,
            IEmailService emailService)
        {
            _repository = repository;
            _userManager = userManager;
            _photoService = photoService;
            _emailService = emailService;
        }

        public IActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public async Task <IActionResult> Messages(string searchTerm)

        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
            var messages = (await _repository.GetAsync<Message>()).ToList();
            if (messages != null && messages.Any())
            {
                var results = messages.GroupBy(x => x.MessageId);
                foreach (var messageThreads in results)
                {
                    var last = messageThreads.OrderBy(x => x.UpdatedOn).Last();
                    var user = await _userManager.FindByIdAsync(last.UserId);
                    messageViewModels.Add(new MessageViewModel
                    {
                        PhotoUrl = user.PhotoUrl,
                        LastText = last.Text,
                        MessageId = last.MessageId,
                        UserId = last.UserId,
                        Name = $"{user.FirstName} {user.LastName}",
                        TimeStamp = last.UpdatedOn,
                        ReadOn = last.ReadOn,
                        DeliverOn = last.DeliverOn
                    });
                }

                return View(messageViewModels);
            }

            ViewBag.Msg = "No messages found";

            return View();
        }
    

        [HttpPost]
        public async Task<IActionResult> CreateMessage(string ReceiverId, string message)
        {
            var user = await _userManager.GetUserAsync(User);
            var receiver = await _userManager.FindByIdAsync(ReceiverId);
            if (receiver == null)
            {
                return RedirectToAction("Messages");
            }
            else
            {
                var newMessage = new Message
                {
                    UserId = user.Id,
                    ReceiverId = receiver.Id,
                    Text = message,
                    TimeStamp = DateTime.UtcNow
                };

                await _repository.AddAsync<Message>(newMessage);
            }

            return RedirectToAction("Messages");
        }
    

        [HttpGet]
        public IActionResult SendReward()
        {
            return View();
        }

        public IActionResult SendReward(ListOfSchoolViewModel model, string searchString, int page = 1)
        {
            int pageSize = 5;

            IQueryable<string> schools = SeedData.Schools.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                schools = schools.Where(s => s.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == 0);
            }

            List<string> schoolsOnPage;
            int totalItems, totalPages;

            Pagination.UsePagination(schools, page, pageSize, out schoolsOnPage, out totalItems, out totalPages);

            model.Schools = schoolsOnPage;
            model.CurrentPage = page;
            model.TotalPages = totalPages;
            model.Count = totalItems;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile() //pass in you VM
        {
            var user = await _userManager.GetUserAsync(User);

            var editProfileViewModel = new EditProfileVM()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NameofSchool = user.NameofSchool
            };

            return View(editProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM editProfileVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                user.FirstName = editProfileVM.FirstName;
                user.LastName = editProfileVM.LastName;
                user.Email = editProfileVM.Email;
                user.PhoneNumber = editProfileVM.PhoneNumber;
                user.NameofSchool = editProfileVM.NameofSchool;

                await _repository.UpdateAsync<AppUser>(user);
            }

            return RedirectToAction("Overview");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        ViewBag.PasswordChangeSuccess = "Password change was successful!";
                        return View();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View(model);
                    }
                }

                ModelState.AddModelError("", "User not found");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Teachers(string schoolName, int page = 1)
        {
            int pageSize = 5;

            var listOfSchoolTaught = await _repository.GetAsync<SchoolsTaught>();

            var model = new TeacherListViewModel();

            if (listOfSchoolTaught != null && listOfSchoolTaught.Any())
            {
                var schoolsTaught = (await _repository.GetAsync<SchoolsTaught>())
                                    .Include(x => x.Teacher)
                                    .ThenInclude(x => x.User).ToList();

                var paginated = new List<SchoolsTaught>();
                int totalItems;
                int totalPages;

                Pagination.UsePagination(schoolsTaught.AsQueryable(), page, pageSize, out paginated, out totalItems, out totalPages);
                model.TeacherList = paginated.Select(x => x.Teacher).ToList();
                model.SchoolName = schoolName;
                model.CurrentPage = page;
                model.Count = totalItems;
                model.TotalPages = totalPages;

                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Teachers(TeacherListViewModel model, string schoolName, int page = 1)
        {
            int pageSize = 5;

            var listOfSchoolTaught = await _repository.GetAsync<SchoolsTaught>();
            if (listOfSchoolTaught != null && listOfSchoolTaught.Any())
            {
                var schoolsTaught = (await _repository.GetAsync<SchoolsTaught>())
                                    .Include(x => x.Teacher)
                                    .ThenInclude(x => x.User).ToList();

                if (!string.IsNullOrEmpty(model.SearchKeyword))
                {
                    schoolsTaught = schoolsTaught.Where(s => s.Teacher.User.FirstName.ToLower().Equals(model.SearchKeyword) ||
                                                             s.Teacher.User.LastName.ToLower().Equals(model.SearchKeyword)).ToList();
                }

                var paginated = new List<SchoolsTaught>();
                int totalItems;
                int totalPages;

                Pagination.UsePagination(schoolsTaught.AsQueryable(), page, pageSize, out paginated, out totalItems, out totalPages);

                model.TeacherList = paginated.Select(x => x.Teacher).ToList();
                model.SchoolName = model.CurrentSchool;
                model.CurrentPage = page;
                model.Count = totalItems;
                model.TotalPages = totalPages;

                return View(model);
            }

            return View(model);
        }


        public IActionResult Transfer()
        {
            return View();
        }

        public async Task<IActionResult> GetSentTransaction(string userId)
        {
            List<SentTransactionViewModel> sentTransactionViewModels = new List<SentTransactionViewModel>();
            var getTransactions = await _repository.GetAsync<Transaction>();
            List<Transaction> transactions = getTransactions.Where(transaction => transaction.SenderId == userId && transaction.TransactionType == "Sent").ToList();
            foreach (var transaction in transactions)
            {
                SentTransactionViewModel transactionsView = new SentTransactionViewModel()
                {
                    Amount = transaction.Amount,
                    timeOfTransaction = transaction.CreatedOn,
                    Description = transaction.Description
                };
                sentTransactionViewModels.Add(transactionsView);
            }
            OverviewViewModel overviewViewModel = new OverviewViewModel()
            {
                MySentTransactions = sentTransactionViewModels
            };

            return View(overviewViewModel);
        }

        public async Task<IActionResult> GetReceivedTransaction(string userId)
        {
            List<ReceivedTransactionsViewModel> receivedTransactionViewModels = new List<ReceivedTransactionsViewModel>();
            var GetTransactions = await _repository.GetAsync<Transaction>();
            List<Transaction> transactions = GetTransactions.Where(transaction => transaction.ReceiverId == userId && transaction.TransactionType == "Received").ToList();
            foreach (var transaction in transactions)
            {
                ReceivedTransactionsViewModel transactionsView = new ReceivedTransactionsViewModel()
                {
                    Amount = transaction.Amount,
                    timeOfTransaction = transaction.CreatedOn,
                    Description = transaction.Description
                };
                receivedTransactionViewModels.Add(transactionsView);
            }
            OverviewViewModel overviewViewModel = new OverviewViewModel()
            {
                MyReceivedTransactions = receivedTransactionViewModels
            };

            return View(overviewViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateImage(UploadImageVM model)
        {
            if (ModelState.IsValid)
            {
                //To get User
                var user = await _userManager.GetUserAsync(User);

                //To go to the database and get the AppUser properties so that we can confirm if the picture is  if (user.PhotoUrl == null)
                if (user != null)
                {
                    Dictionary<string, string> cloudinaryResponse =
                        await _photoService.UploadImage(model.Image, $"{user.LastName} {user.FirstName}");
                    if (cloudinaryResponse["Code"] == "200")
                    {
                        string photoUrl = cloudinaryResponse["Url"];
                        string publicId = cloudinaryResponse["PublicId"];
                        user.PhotoUrl = photoUrl;
                        user.PublicId = publicId;
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("UpdateImage");
                        }
                        else
                        {
                            foreach (var err in result.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                        }
                    }

                    ModelState.AddModelError("", cloudinaryResponse["Message"]);
                }
            }

            return View();
        }
       
        [HttpGet]
        public async Task<IActionResult> TransferHistory(string userId)
        {
            FundAndTransferCombinedViewModel fundAndTransferCombinedViewModel = new FundAndTransferCombinedViewModel();

            IQueryable<Transaction> GetTransactions = await _repository.GetAsync<Transaction>();
        
            List<Transaction> transactions = GetTransactions
                .Where(transaction => transaction.TransactionType == TransactionTypes.Transfer
                .ToString() && userId == transaction.SenderId).ToList();

            foreach (Transaction transaction in transactions)
            {
                var getTeacher = await _repository.GetAsync<Teacher>();

                var getUser = await _repository.GetAsync<AppUser>();

                AppUser receiver = await _repository.GetAsync<AppUser>(transaction.ReceiverId);
                Teacher teacher = (await _repository.GetAsync<Teacher>()).FirstOrDefault(t => t.UserId == receiver.Id);

                var school = teacher.SchoolsTaughts.OrderByDescending(x => x.CreatedOn).FirstOrDefault().School;

                TransferTransactionHistoryViewModel transferTransactionHistoryViewModel
                    = new TransferTransactionHistoryViewModel()
                    {
                        NameOfTeacher = receiver.FirstName + " " + receiver.LastName,
                        Amount = transaction.Amount,
                        dateTime = transaction.CreatedOn,
                        School = school,
                    };

                fundAndTransferCombinedViewModel.TransferTransactions.Add(transferTransactionHistoryViewModel);
            }        

            return View(fundAndTransferCombinedViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FundingHistory(string userId)
        {
            FundAndTransferCombinedViewModel fundAndTransferCombinedViewModel = new FundAndTransferCombinedViewModel();

            IQueryable<Transaction> GetTransactions = await _repository.GetAsync<Transaction>();
                    
            List<Transaction> fundTransactions = GetTransactions.Where(transaction => transaction
            .SenderId == userId && transaction.TransactionType == TransactionTypes.Funding.ToString()).ToList();

            foreach (var transaction in fundTransactions)
            {
                FundingTransactionHistoryViewModel transactionsView = new FundingTransactionHistoryViewModel()
                {
                    Amount = transaction.Amount,
                    CreatedOn = transaction.CreatedOn,
                    Description = transaction.Description
                };
                fundAndTransferCombinedViewModel.FundingTransactions.Add(transactionsView);
            }

            return View(fundAndTransferCombinedViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> EditTeacherProfile()
        {
            var loggedInUsedr = await _userManager.GetUserAsync(User);

            var teacher = ((await _repository.GetAsync<Teacher>())
                .Include(x => x.User)
            ).FirstOrDefault(x => x.UserId == loggedInUsedr.Id);

            if(teacher != null)
            {
                var editProfileViewModel = new EditTeacherProfileVM()
                {
                    TeacherId = teacher.Id,
                    FirstName = $"{teacher.User.FirstName}",
                    LastName = $"{teacher.User.LastName}",
                    Email = teacher.User.Email,
                    PhoneNumber = teacher.User.PhoneNumber,
                    SchoolType = teacher.SchoolType,
                    YearsOfTeaching = teacher.YearsOfTeaching
                };
                return View(editProfileViewModel);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditTeacherProfile(EditTeacherProfileVM model)
        {
            if (ModelState.IsValid)
            {
                var loggedInUsedr = await _userManager.GetUserAsync(User);
                var teacher = ((await _repository.GetAsync<Teacher>())
                   .Include(x => x.User)
                   .Include(x => x.TeacherSubjects)
                   .Include(x => x.SchoolsTaughts)
               ).FirstOrDefault(x => x.UserId == loggedInUsedr.Id);

                if (teacher != null) 
                {
                    teacher.User.FirstName = model.FirstName;
                    teacher.User.LastName = model.LastName;
                    teacher.User.PhoneNumber = model.PhoneNumber;
                    teacher.YearsOfTeaching = model.YearsOfTeaching;
                    teacher.SchoolType = model.SchoolType;

                    using var startTransaction = _repository._ctx.Database.BeginTransaction();
                    try
                    {
                        await _repository.UpdateAsync(teacher);

                        foreach (var item in teacher.SchoolsTaughts.ToList())
                        {
                            await _repository.DeleteAsync(item);
                        }

                        foreach (var item in teacher.TeacherSubjects.ToList())
                        {
                            await _repository.DeleteAsync(item);
                        }

                        await _repository.AddAsync(new SchoolsTaught { TeacherId = model.TeacherId, School = model.SelectedSchool });
                        await _repository.AddAsync(new SubjectsTaught { TeacherId = model.TeacherId, Subject = model.SelectedSubject });
                        
                        await startTransaction.CommitAsync();

                        return RedirectToAction("overview", "dashboard");
                    }
                    catch
                    {
                        await startTransaction.RollbackAsync();
                        ModelState.AddModelError("", "Edit teacher failed.");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Could not get loggedin user record.");

            }

            return View(model);

        }

        //[HttpPut]
        //public async Task<IActionResult> EditTeacherProfile(EditTeacherProfileVM model, string userId)
        //{
        //    var teacher = (await _repository.GetAsync<Teacher>()).Where(s => s.UserId == userId).Select(s => s.User).FirstOrDefault();

        //    IList<SubjectsTaught> teachersSubject = (await _repository.GetAsync<SubjectsTaught>()).Where(s => s.TeacherId == userId).ToList();

        //    var photo = await _photoService.UploadImage(model.photo, model.FolderName);

        //    if (teacher is not null)
        //    {
        //        teacher.FirstName = model.FullName;
        //        teacher.LastName = model.FullName;
        //        teacher.NameofSchool = model.SchoolType;
        //        teacher.PhoneNumber = model.PhoneNumber;
        //        teacher.Email = model.Email;
        //        teacher.PhotoUrl = photo["Url"];

        //        var updateTeacher = await _repository.UpdateAsync<AppUser>(teacher);

        //        if (updateTeacher != 0)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        return View(model);
        //    }
        //    return View(model);
        //}
    }
}

