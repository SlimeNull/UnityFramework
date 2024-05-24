using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SlimeNull.UnityFramework
{
    public class MessageCenter
    {
        private readonly Dictionary<Type, Delegate> _messageHandlers = new();

        static MessageCenter()
        {
            Type typeInterfaceMessageHandler = typeof(IMessageHandler);

            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (!typeInterfaceMessageHandler.IsAssignableFrom(type))
                    continue;
                if (type.GetCustomAttribute<MessageHandlerAttribute>() is null)
                    continue;

                var instance = (IMessageHandler)Activator.CreateInstance(type);
                var messageType = instance.MessageType;
                var typeMessageHandler = typeof(MessageHandler<>).MakeGenericType(messageType);

                if (!typeMessageHandler.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"Type '{type}' implements IMessageHandler, but not inherit 'MessageHandler<T>'");
                }

                var handlerMethod = type.GetMethod(nameof(MessageHandler<object>.HandleMessage));
                var handlerMethodDelegateType = typeof(Action<>).MakeGenericType(messageType);
                var handlerMethodAction = handlerMethod.CreateDelegate(handlerMethodDelegateType, instance);

                MessageCenter.Instance.CoreRegister(messageType, handlerMethodAction);
            }
        }

        public static MessageCenter Instance { get; } = new();

        private void CoreRegister(Type messageType, Delegate handler)
        {
            if (_messageHandlers.TryGetValue(messageType, out var existedHandler))
            {
                _messageHandlers[messageType] = Delegate.Combine(existedHandler, handler);
            }
            else
            {
                _messageHandlers[messageType] = handler;
            }
        }

        public void Register<T>(Action<T> handler)
        {
            CoreRegister(typeof(T), handler);
        }

        public void Unregister<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_messageHandlers.TryGetValue(type, out var existedHandler))
            {
                var remainHandler = Delegate.Remove(existedHandler, handler);

                if (remainHandler != null)
                {
                    _messageHandlers[type] = remainHandler;
                }
                else
                {
                    _messageHandlers.Remove(type);
                }
            }
        }

        public void Broadcast<T>(T message)
        {
            var type = typeof(T);
            if (_messageHandlers.TryGetValue(type, out var existedHandler) &&
                existedHandler is Action<T> action)
            {
                action.Invoke(message);
            }
        }
    }
}