namespace App.Termination.PoC.Api.Services;

public class OperationService(ILogger<OperationService> _logger, GuidService _guidService)
{
    public async Task ExecuteOperation(Guid guid, OperationType type)
    {
        _guidService.StoreGuid(guid);
        var seconds = 10;
        do
        {
            _logger.LogInformation("({guid}) [OperationService.ExecuteOperation.{type}] Waiting for {seconds} seconds", guid, type, seconds);
            await Task.Delay(1000);
        }
        while (--seconds > 0);
        _logger.LogInformation("({guid}) [OperationService.ExecuteOperation.{type}] Done waiting!", guid, type);
        _guidService.DiscardGuid(guid);
    }
}

public enum OperationType
{
    Long,
    Back
}