using System;
using System.Collections.Generic;

namespace ICAN.SIC.Abstractions
{
    public interface IHub
    {
        string Name { get; }

        List<string> GetAllHubNames();

        void PublishDownwards<T>(T message, TimeSpan? delay = null) where T : IMessage;

        void setMyParent(IHub hub);

        void Publish<T>(T message, TimeSpan? delay = null) where T : IMessage;

        void Subscribe<T>(Action<T> messageAction) where T : IMessage;

        void Unsubscribe<T>(Action<T> messageAction) where T : IMessage;

        void PassThrough(IHub hub);
    }
}