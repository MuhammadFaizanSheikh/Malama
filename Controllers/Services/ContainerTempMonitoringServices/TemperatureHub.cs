using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Malama.Controllers.Services.ContainerTempMonitoringServices
{
    public class TemperatureHub : Hub
    {
        // Called automatically when a user connects
        public override async Task OnConnectedAsync()
        {
            var isEventAssignedToStaff = Context.User?.FindFirst("IsEventAssignedToStaff")?.Value;
            var eventId = Context.User?.FindFirst("EventIdString")?.Value;

            if (!string.IsNullOrEmpty(isEventAssignedToStaff) && isEventAssignedToStaff.Equals("true",StringComparison.OrdinalIgnoreCase)  && !string.IsNullOrEmpty(eventId))
            {
                // Add user connection to a group based on EventId
                await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
            }

            await base.OnConnectedAsync();
        }
    }
}
