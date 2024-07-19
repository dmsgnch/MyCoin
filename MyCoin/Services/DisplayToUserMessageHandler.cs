using System.Net.Http;
using System.Windows;
using MyCoin.Services.Abstract;
using MyCoin.Components;

namespace MyCoin.Services;

public class DisplayToUserMessageHandler : MessageHandlerBase
{
    public override async Task HandleAsync(MessageChain messageChain)
    {
        if (messageChain.DisplayToUser)
        {
            MessageBox.Show(messageChain.Message.MessageText, messageChain.Message.Caption);
        }

        if (Successor is not null)
        {
            await Successor.HandleAsync(messageChain);
        }
    }
}