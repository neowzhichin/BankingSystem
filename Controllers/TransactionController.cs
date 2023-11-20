using log4net;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TransactionController : ControllerBase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(TransactionController));

    private readonly ITransactionService _transactionService;
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public Task<IEnumerable<Transaction>> History([FromQuery] int limit, [FromQuery] int page)
    {
        string token = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);
        string topic = JwtTokenService.GetUniqueIdFromJWT(token);
        var r = _transactionService.GetTransactions(topic,limit,page);
        return r;
    }

    [HttpPost]
    public Transaction NewTransaction([FromBody] Transaction req)
    {
        string token = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);
        string id = JwtTokenService.GetUniqueIdFromJWT(token);
        req.Id = Guid.NewGuid().ToString();
        _transactionService.NewTransactions(id, req);
        return req;
    }

    [HttpPost]
    public string NewUserAccount()
    {
        string username = Request.Form["username"];
        return JwtTokenService.GenerateJwtToken(username);
    }

}

