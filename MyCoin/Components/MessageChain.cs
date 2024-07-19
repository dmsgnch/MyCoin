namespace MyCoin.Components;

public class MessageChain
{
    public Message Message { get; set; }

    public bool DisplayToUser { get; set; }
    public bool Log { get; set; }
    public bool WriteToConsole { get; set; }
    
    public MessageChain(Message message, bool dtu, bool log, bool wtc)
    {
        Message = message;
        
        DisplayToUser = dtu;
        Log = log;
        WriteToConsole = wtc;
    }
}