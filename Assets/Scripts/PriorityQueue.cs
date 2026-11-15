using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly List<(T item, int priority)> heap = new();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        SiftUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        T top = heap[0].item;

        int last = heap.Count - 1;
        heap[0] = heap[last];
        heap.RemoveAt(last);

        if (heap.Count > 0)
            SiftDown(0);

        return top;
    }

    private void SiftUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].priority >= heap[parent].priority)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void SiftDown(int index)
    {
        int count = heap.Count;

        while (true)
        {
            int left = 2 * index + 1;
            if (left >= count)
                break;

            int right = left + 1;
            int smallest = right < count && heap[right].priority < heap[left].priority ? right : left;

            if (heap[index].priority <= heap[smallest].priority)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        (heap[a], heap[b]) = (heap[b], heap[a]);
    }
}
