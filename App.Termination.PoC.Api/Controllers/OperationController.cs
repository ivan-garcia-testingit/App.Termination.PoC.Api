using App.Termination.PoC.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Termination.PoC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperationController(ILogger<OperationController> _logger, OperationService _operationService, GuidService _guidService, EnvironmentService _environmentService) : ControllerBase
{
    [HttpPost("long")]
    public async Task<IActionResult> LongOperation()
    {
        // creates a guid for the request
        var guid = Guid.NewGuid();
        _logger.LogInformation("({guid}) [OperationController.LongOperation] Received a request", guid);

        // runs the task in the current thread and removes the guid when finished
        await _operationService.ExecuteOperation(guid, OperationType.Long);
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
                
        // runs the task in the background and removes the guid when finished
        _ = Task.Run(async () => await _operationService.ExecuteOperation(guid, OperationType.Back));
        _logger.LogInformation("({guid}) [OperationController.BackOperation] Done executing", guid);
        
        // returns the hostname to verify the load balancer is working as expected
        return Ok(_environmentService.GetHostname());
    }
}
