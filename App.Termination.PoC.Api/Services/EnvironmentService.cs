namespace App.Termination.PoC.Api.Services;

public class EnvironmentService
{
    public string GetHostname() => Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME")!;
}
