namespace MyCoin.Components;

public class Message
{
    public string MessageText { get; set; }
    public string Caption { get; set; }
    
    public Message(string messageText, string caption = "")
    {
        MessageText = messageText;
        Caption = caption;
    }
}