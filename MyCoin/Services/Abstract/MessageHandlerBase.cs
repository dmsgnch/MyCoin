using MyCoin.Components;

namespace MyCoin.Services.Abstract;

public abstract class MessageHandlerBase
{
    public MessageHandlerBase? Successor { get; set; }

    public abstract Task HandleAsync(MessageChain messageChain);
}