using DirectorySizeWPFasync.Tree;

namespace DirectorySizeWPFasync.Observer
{
    internal interface ISubscriber<T>
    {
        void AddSize(T data);
        void AddAccessDenied();
        void AddChild(ISubject<T> subject);
    }
}