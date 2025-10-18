using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    [Test]
    public void Enqueue_AddsElementsInOrder()
    {
        var queue = new InventoryQueue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        var arr = queue.IQtoArray();

        Assert.AreEqual(new[] { 1, 2, 3 }, arr);
        Assert.AreEqual(3, queue.Count);
    }

    [Test]
    public void Dequeue_RemovesFirstElement()
    {
        var queue = new InventoryQueue<string>();
        queue.Enqueue("apple");
        queue.Enqueue("banana");
        queue.Enqueue("cherry");

        string first = queue.Dequeue();

        Assert.AreEqual("apple", first);
        Assert.AreEqual(2, queue.Count);

        var arr = queue.IQtoArray();
        Assert.AreEqual(new[] { "banana", "cherry" }, arr);
    }

    [Test]
    public void Dequeue_EmptyQueue_ThrowsException()
    {
        var queue = new InventoryQueue<int>();
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
    }

    [Test]
    public void Resize_ExpandsCapacityAndPreservesOrder()
    {
        var queue = new InventoryQueue<int>(2);

        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3); // Resize()

        Assert.AreEqual(3, queue.Count);
        var arr = queue.IQtoArray();
        Assert.AreEqual(new[] { 1, 2, 3 }, arr);
    }

    [Test]
    public void Dequeue_ClearsReference()
    {
        var queue = new InventoryQueue<string>();
        queue.Enqueue("Sword");
        queue.Enqueue("Shield");

        queue.Dequeue(); // delete "Sword"

        var arr = queue.IQtoArray();

        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual("Shield", arr[0]);
    }

    [Test]
    public void EnqueueDequeueMultipleTimes_WorksCorrectly()
    {
        var queue = new InventoryQueue<int>(3);

        queue.Enqueue(10);
        queue.Enqueue(20);
        queue.Enqueue(30);
        queue.Dequeue(); // delete 10
        queue.Enqueue(40);

        var arr = queue.IQtoArray();
        Assert.AreEqual(new[] { 20, 30, 40 }, arr);
    }
}
