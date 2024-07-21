namespace MyCoin.Components;

/// <summary>
/// A class for managing message operations using the “chain of responsibilities” pattern
/// </summary>
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