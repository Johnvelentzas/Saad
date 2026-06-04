using CommunityToolkit.Mvvm.Messaging.Messages;
using Models;

namespace Producion_Line_Manager.Messages
{
    // This message carries an IEntity (like a new Order) to whoever is listening
    public class OpenNewEntityMessage : ValueChangedMessage<IEntity>
    {
        public OpenNewEntityMessage(IEntity value) : base(value) { }
    }
}