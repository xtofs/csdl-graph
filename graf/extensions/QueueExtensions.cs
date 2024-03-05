namespace graf;

public static class QueueExtensions
{
    public static void Add<T>(this Queue<T> queue, T item)
    {
        queue.Enqueue(item);
    }
}