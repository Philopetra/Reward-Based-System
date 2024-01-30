using Microsoft.AspNetCore.Mvc;
using RYT.Models.ViewModels;
using RYT.Services.Payment;

namespace RYT.Controllers;

[Route("payments")]
public class PaymentController : Controller
{
    private readonly IPayments _payments;

    public PaymentController(IPayments payments)
    {
        _payments = payments;
    }
    
    public async Task<IActionResult> FundWallet([FromForm] FundWalletVM model)
    {
        // var userId = User.Claims.First(c => c.Type == "id").Value;
        var response = await _payments.Initialize(model, userId);

        if (!response.Item1)
        {
            ViewBag.Message = "Withdrawal failed";
            return BadRequest("Withdrawal failed");
        }

        return Redirect(response.Item2);
    }
    
    [HttpGet("callback")]
    public async Task<IActionResult> VerifyPayment([FromQuery] string reference)
    {
        // var userId = User.Claims.First(c => c.Type == "id").Value;
        
        var isSuccessful = await _payments.Verify(reference);
        
        if (!isSuccessful)
        {
            // redirect to failed 
            ViewBag.Message = "Payment verification failed";
            return BadRequest("Payment verification failed");
        }
        
        // redirect to success page
        ViewBag.Message = "Payment verification successful";
        return Ok("Payment verification successful");
    }

    [HttpGet("withdraw")]
    public async Task<IActionResult> Withdraw()
    {
        var model = new WithdrawVM
        {
            Banks = await _payments.GetBanks()
        };
        
        // return view to enter withdrawal details
        return View(model);
    }
    
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromForm] CreateWithdrawalVM model)
    {
        // var userId = User.Claims.First(c => c.Type == "id").Value;
        ViewBag.IsSuccessful = false;
        try
        {
            var response = await _payments.Withdraw(model, userId);
            if (!response)
            {
                // redirect to failed page
                ViewBag.Message = "Withdrawal failed";
                return BadRequest("Withdrawal failed");
            }
            
            // redirect to success page
            ViewBag.IsSuccessful = true;
            ViewBag.Message = "Withdrawal successful";
            return Ok("Withdrawal successful");
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    [HttpPost("transfer/{receiverId}")]
    public async Task<IActionResult> Transfer([FromRoute] string receiverId, [FromForm] SendRewardVM model)
    {
        // var userId = User.Claims.First(c => c.Type == "id").Value;
        ViewBag.IsSuccessful = false;
        try
        {
            var isSuccessful = await _payments.Transfer(userId, receiverId, model.Amount);

            if (!isSuccessful)
            {
                // redirect to failure page
                ViewBag.Message = "Transfer failed";
                return BadRequest("Transfer failed");
            }
            
            ViewBag.IsSuccessful = true;
            ViewBag.Message = "Transfer successful";
            return Ok("Transfer successful");
        }
        catch (InvalidOperationException e)
        {
            ViewBag.Message = e.Message;
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("banks")]
    public async Task<IActionResult> GetBanks()
    {
        var banks = await _payments.GetBanks();
        
        return Ok(banks);
    }
}