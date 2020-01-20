using DirectorySizeWPFasync.UI;

namespace DirectorySizeWPFasync.Observer
{
    internal interface ISubject<T>
    {
        T Data { get; }
        ISubscriber<T> Subscriber { set; }
    }
}