using App.Termination.PoC.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Termination.PoC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperationController(IHostApplicationLifetime _hostLifetime, ILogger<OperationController> _logger, OperationService _operationService, GuidService _guidService, EnvironmentService _environmentService) : ControllerBase
{
    [HttpPost("long")]
    public async Task<IActionResult> LongOperation(CancellationToken requestCt)
    {
        var effectiveCt = CancellationTokenSource.CreateLinkedTokenSource(requestCt, _hostLifetime.ApplicationStopping).Token;

        // creates a guid for the request
        var guid = Guid.NewGuid();
        _logger.LogInformation("({guid}) [OperationController.LongOperation] Received a request", guid);

        // stores the guid for future references
        _guidService.StoreGuid(guid);

        // runs the task in the current thread and removes the guid when finished
        await _operationService.ExecuteOperation(guid, OperationType.Long).ContinueWith(_ => _guidService.DiscardGuid(guid));
        _logger.LogInformation("({guid}) [OperationController.LongOperation] Done executing", guid);

        // returns the hostname to verify the load balancer is working as expected
        return Ok(_environmentService.GetHostname());
    }
    
    [HttpPost("back")]
    public IActionResult BackOperation()
    {
        // creates a guid for the request
        var guid = Guid.NewGuid();
        _logger.LogInformation("({guid}) [OperationController.BackOperation] Received a request", guid);
        
        // stores the guid for future references
        _guidService.StoreGuid(guid);
        
        // runs the task in the background and removes the guid when finished
        _ = Task.Run(async () => await _operationService.ExecuteOperation(guid, OperationType.Back).ContinueWith(_ => _guidService.DiscardGuid(guid)));
        _logger.LogInformation("({guid}) [OperationController.BackOperation] Done executing", guid);
        
        // returns the hostname to verify the load balancer is working as expected
        return Ok(_environmentService.GetHostname());
    }
}
