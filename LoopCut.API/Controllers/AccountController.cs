using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new account", Description = "Creates a new user account with the provided details.")]
        public async Task<IActionResult> CreateAccount(AccountRequest account)
        {
            var result = await _accountService.CreateAccount(account);
            return Ok(ApiResponse<AccountResponse>.OkResponse(result,"Create account successful!","201"));
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get account by ID", Description = "Retrieves the account details for the specified account ID.")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var result = await _accountService.GetAccountById(id);
            return Ok(ApiResponse<AccountResponse>.OkResponse(result,"Get account successful!","200"));
        }
        [HttpGet()]
        [SwaggerOperation(Summary = "Get all accounts", Description = "Retrieves a list of all user accounts.")]
        public async Task<IActionResult> GetAllAccounts(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _accountService.GetAllAccounts(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<AccountResponse>>.OkResponse(result,"Get all accounts successful!","200"));
        }
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update account", Description = "Updates the details of an existing user account.")]
        public async Task<IActionResult> UpdateAccount(string id, AccountUpRequest account)
        {
            var result = await _accountService.UpdateAccount(id, account);
            return Ok(ApiResponse<AccountResponse>.OkResponse(result,"Update account successful!","200"));
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete account", Description = "Deletes the user account with the specified ID.")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            await _accountService.DeleteAccount(id);
            return NoContent();
        }
    }
}
