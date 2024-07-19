using MyCoin.Services.Abstract;
using MyCoin.Components;

namespace MyCoin.Services;

public class WriteToConsoleMessageHandler : MessageHandlerBase
{
    public override async Task HandleAsync(MessageChain messageChain)
    {
        if (messageChain.DisplayToUser)
        {
            Console.WriteLine($"{messageChain.Message.Caption}. {messageChain.Message.MessageText}");
        }

        if (Successor is not null)
        {
            await Successor.HandleAsync(messageChain);
        }
    }
}
