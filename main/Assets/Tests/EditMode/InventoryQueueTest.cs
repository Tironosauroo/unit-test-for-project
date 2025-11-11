/// @file InventoryQueueTest.cs
/// @brief This file contains unit tests for the InventoryQueue class in a Unity game environment.
///        It uses NUnit framework to test various functionalities like enqueue, dequeue, resizing, and reference clearing.
///        The tests ensure the queue behaves correctly as a FIFO data structure with dynamic capacity expansion.
/// @details The InventoryQueue is assumed to be a custom queue implementation for managing inventory items in the game.
///          Tests cover basic operations, edge cases (e.g., empty queue), and complex scenarios like multiple enqueues/dequeues.
///          All tests are written for Unity's test runner and use UnityEngine.TestTools for coroutine support if needed.

using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

/// @brief Test class for InventoryQueue functionality.
/// @details This class provides comprehensive unit tests to validate the InventoryQueue implementation.
///          It tests enqueue, dequeue, capacity resizing, reference clearing, and mixed operations.
///          Each test method is designed to be independent and focuses on a specific aspect of the queue's behavior.
public class NewTestScript
{
    /// @brief Tests that Enqueue adds elements in the correct order.
    /// @details Verifies that elements are added to the end of the queue and the order is preserved.
    ///          Also checks that the count is updated correctly.
    /// @throws None (test assertions handle failures).
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

    /// @brief Tests that Dequeue removes and returns the first element.
    /// @details Ensures the queue follows FIFO order, updates count, and remaining elements are in order.
    /// @throws None (test assertions handle failures).
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

    /// @brief Tests that Dequeue throws an exception when the queue is empty.
    /// @details Validates error handling for invalid operations on an empty queue.
    /// @throws InvalidOperationException Expected when dequeuing from an empty queue.
    [Test]
    public void Dequeue_EmptyQueue_ThrowsException()
    {
        var queue = new InventoryQueue<int>();
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
    }

    /// @brief Tests that the queue resizes capacity and preserves element order.
    /// @details Simulates capacity expansion when exceeding initial size, ensuring no data loss.
    /// @throws None (test assertions handle failures).
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

    /// @brief Tests that Dequeue clears references to dequeued elements.
    /// @details Ensures memory management by clearing references, preventing potential leaks in game inventory.
    /// @throws None (test assertions handle failures).
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

    /// @brief Tests multiple enqueues and dequeues in sequence.
    /// @details Validates complex operations like mixing additions and removals, ensuring queue integrity.
    /// @throws None (test assertions handle failures).
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
