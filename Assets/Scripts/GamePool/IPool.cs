public interface IPool<T> where T : class, IPoolable
{
    int CountActive { get; }
    int CountInactive { get; }
    T Get();
    void Return(T item);
}
