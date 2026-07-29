using Microsoft.Extensions.Hosting;
using RealTimeCollaboration.Modules.Invitation.Interfaces;

namespace RealTimeCollaboration.Modules.Invitation;

public class InvitationCleanupService : IHostedService, IDisposable
{
    private readonly IInvitationRepository _invitationRepository;
    private Timer? _timer;

    public InvitationCleanupService(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // run every 5 minutes
        _timer = new Timer(async _ => await Cleanup(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private async Task Cleanup()
    {
        try
        {
            await _invitationRepository.DeleteExpiredAsync(DateTime.UtcNow);
        }
        catch
        {
            // swallow - cleanup should be best-effort
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
