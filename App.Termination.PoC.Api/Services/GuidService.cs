using System;
using System.Collections.Concurrent;

namespace App.Termination.PoC.Api.Services;

public class GuidService(ILogger<GuidService> _logger)
{
    private readonly ConcurrentDictionary<Guid, DateTime> _guids = [];

    public void StoreGuid(Guid guid)
    {
        _logger.LogInformation("({guid}) [GuidService.StoreGuid] Storing the GUID", guid);
        if (_guids.TryAdd(guid, DateTime.UtcNow))
        {
            _logger.LogInformation("({guid}) [GuidService.StoreGuid] GUID stored successfully", guid);
        }
        else
        {
            _logger.LogWarning("({guid}) [GuidService.StoreGuid] GUID store failed", guid);
        }
    }

    public void DiscardGuid(Guid guid)
    {
        _logger.LogInformation("({guid}) [GuidService.DiscardGuid] Discarding the GUID", guid);
        if (_guids.TryRemove(guid, out _))
        {
            _logger.LogInformation("({guid}) [GuidService.DiscardGuid] GUID discarded successfully", guid);
        }
        else
        {
            _logger.LogInformation("({guid}) [GuidService.DiscardGuid] GUID discard failed", guid);
        }
    }

    public async Task WaitForAllGuids()
    {
        while (!_guids.IsEmpty)
        {
            _logger.LogInformation("[GuidService.WaitForAllTasks] Waiting 1 second for {guids} GUIDs", _guids.Count);
            await Task.Delay(1000);
        }
        _logger.LogInformation("[GuidService.WaitForAllTasks] Done waiting for {guids} GUIDs", _guids.Count);
    }
}
