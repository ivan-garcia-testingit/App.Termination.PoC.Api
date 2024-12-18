namespace App.Termination.PoC.Api.Services;

public class OperationService(ILogger<OperationService> _logger)
{
    public async Task ExecuteOperation(Guid guid, OperationType type)
    {
        var seconds = 10;
        do
        {
            _logger.LogInformation("({guid}) [OperationService.ExecuteOperation.{type}] Waiting for {seconds}/10 seconds", guid, type, seconds);
            await Task.Delay(1000);
        }
        while (--seconds > 0);
        _logger.LogInformation("({guid}) [OperationService.ExecuteOperation.{type}] Done waiting!", guid, type);
    }
}

public enum OperationType
{
    Long,
    Back
}