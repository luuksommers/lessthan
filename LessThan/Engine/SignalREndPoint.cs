using System.Web;
using SignalR;

namespace LessThan.Models
{
    public class SignalREndPoint : PersistentConnection
    {
        protected override System.Threading.Tasks.Task OnConnectedAsync(HttpContextBase context, string clientId)
        {
            return Connection.Broadcast("Client " + clientId + " connected");
        }
    }
}