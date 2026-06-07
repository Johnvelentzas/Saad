using CommunityToolkit.Mvvm.Messaging.Messages;
using Models;

namespace Producion_Line_Manager.Messages
{
    public class OpenEntityMessage : ValueChangedMessage<IEntity>
    {
        public OpenEntityMessage(IEntity value) : base(value) { }
    }
}