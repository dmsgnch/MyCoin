using Microsoft.Extensions.Logging;
using MyCoin.Services.Abstract;
using MyCoin.Components;

namespace MyCoin.Services;

public class WriteToConsoleMessageHandler : MessageHandlerBase
{
    private readonly ILogger _logger;
    
    public WriteToConsoleMessageHandler(ILogger consoleLogger)
    {
        _logger = consoleLogger;
    }
    
    public override async Task HandleAsync(MessageChain messageChain)
    {
        if (messageChain.DisplayToUser)
        {
            _logger.LogError($"{messageChain.Message.Caption}. {messageChain.Message.MessageText}");
        }

        if (Successor is not null)
        {
            await Successor.HandleAsync(messageChain);
        }
    }
}
