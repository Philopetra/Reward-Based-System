using Microsoft.AspNetCore.Mvc;
using RYT.Commons;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IRepository _repository;
        public DashboardController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Overview()
        {
            return View();
        }

        public IActionResult Messages()

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

        public IActionResult EditProfile() 
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();  
        }

        public IActionResult Teachers()
        {
            return View();
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
    }

}
