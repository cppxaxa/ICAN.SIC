using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ICAN.SIC.Abstractions;

namespace ICAN.SIC.PubSub
{
    public class Hub : IHub
    {
        private readonly ConcurrentDictionary<Type, IList> _subscribers = new ConcurrentDictionary<Type, IList>();
        private readonly ConcurrentBag<IHub> _hubs = new ConcurrentBag<IHub>();
        private readonly string name;
        private IHub parentHub;

        public string Name { get { return name; } }

        public Hub(string Name)
        {
            this.name = Name;
        }

        public void PublishDownwards<T>(T message, TimeSpan? delay = null) where T : IMessage
        {
            Task.Run(() => PublishMessage(message, delay));
        }

        private async Task PublishMessage<T>(T message, TimeSpan? delay = null) where T : IMessage
        {
            if (delay.HasValue)
            {
                await Task.Delay((int)delay.Value.TotalMilliseconds).ConfigureAwait(false);
            }

            var type = typeof(T);
            IList messageActions;
            if (_subscribers.TryGetValue(type, out messageActions))
            {
                InvokeActions(messageActions, message);
            }

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.BaseType != null && _subscribers.TryGetValue(typeInfo.BaseType, out messageActions))
            {
                InvokeActions(messageActions, message);
            }

            foreach (var t in typeInfo.ImplementedInterfaces)
            {
                if (_subscribers.TryGetValue(t, out messageActions))
                {
                    InvokeActions(messageActions, message);
                }
            }

            foreach (var hub in _hubs)
            {
                hub.PublishDownwards(message);
            }
        }

        private void InvokeActions<T>(IList messageActions, T message) where T : IMessage
        {
            foreach (Action<T> messageTask in messageActions)
            {
                Task.Run(() => messageTask.Invoke(message));
            }
        }

        public void Subscribe<T>(Action<T> messageAction) where T : IMessage
        {
            _subscribers.AddOrUpdate(typeof(T),
                new List<Action<T>> { messageAction },
                (type, list) =>
                {
                    list.Add(messageAction);
                    return list;
                });
        }

        public void UnsubscribeAll()
        {
            if (this.parentHub == null)
                this.UnsubscribeAllDownwards();
            else
                this.parentHub.UnsubscribeAll();
        }

        public void UnsubscribeAllDownwards()
        {
            _subscribers.Clear();

            foreach (var childHubs in _hubs)
            {
                childHubs.UnsubscribeAllDownwards();
            }
        }

        public void Unsubscribe<T>(Action<T> messageAction) where T : IMessage
        {
            IList messageActions;
            if (_subscribers.TryGetValue(typeof(T), out messageActions))
            {
                messageActions.Remove(messageAction);
            }
        }

        public void setMyParent(IHub hub)
        {
            this.parentHub = hub;
        }

        public void PassThrough(IHub hub) { _hubs.Add(hub); hub.setMyParent(this); }

        public List<string> GetAllHubNames()
        {
            List<string> result = new List<string>();

            foreach (var hub in _hubs)
            {
                result.Add(hub.Name);
            }

            return result;
        }

        public void Publish<T>(T message, TimeSpan? delay = null) where T : IMessage
        {
            if (this.parentHub == null)
                this.PublishDownwards<T>(message, delay);
            else
                this.parentHub.Publish<T>(message, delay);
        }
    }
}