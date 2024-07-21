using MyCoin.Services.Abstract;
using MyCoin.Components;

namespace MyCoin.Services;

public class LogMessageHandler : MessageHandlerBase
{
    public override async Task HandleAsync(MessageChain messageChain)
    {
        if (messageChain.DisplayToUser)
        {
            throw new NotImplementedException();
        }

        if (Successor is not null)
        {
            await Successor.HandleAsync(messageChain);
        }
    }
}