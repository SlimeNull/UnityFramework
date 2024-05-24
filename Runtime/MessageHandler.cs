using System;

namespace SlimeNull.UnityFramework
{
    public abstract class MessageHandler<T> : IMessageHandler
    {
        public Type MessageType => typeof(T);

        public abstract void HandleMessage(T message);
    }

    public interface IMessageHandler
    {
        public Type MessageType { get; }
    }
}