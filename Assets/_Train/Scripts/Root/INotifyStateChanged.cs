using System;

namespace _Train.Scripts.Root
{
    public interface INotifyStateChanged
    {
        public event Action OnChange;
    }
}