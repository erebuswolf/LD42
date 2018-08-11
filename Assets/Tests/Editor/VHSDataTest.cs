using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

[TestFixture]
public class VHSDataTest {

    [SetUp]
    public void GetReady() {
    }

    [TearDown]
    public void Clean() {
    }
    
    [Test]
    public void InsertSingleElement() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4);
        vHSData.AddTimestamp(first);
        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 1);
        Assert.AreEqual(timestamps[0], first);
    }

    [Test]
    public void InsertTwoElements() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4);
        Timestamp second = new Timestamp(0, 4, 6);
        vHSData.AddTimestamp(first);
        vHSData.AddTimestamp(second);
        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);
    }

    [Test]
    public void InsertTwoElementsReversed() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4);
        Timestamp second = new Timestamp(0, 4, 6);

        vHSData.AddTimestamp(second);
        vHSData.AddTimestamp(first);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);
    }

    [Test]
    public void InsertTwoElementsOverlay() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4);
        Timestamp second = new Timestamp(0, 3, 6);

        vHSData.AddTimestamp(first);
        vHSData.AddTimestamp(second);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);
        Assert.AreEqual(first.Stop, 3);
    }

    [Test]
    public void InsertTwoElementsOverlayReverse() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4);
        Timestamp second = new Timestamp(0, 3, 6);

        vHSData.AddTimestamp(second);
        vHSData.AddTimestamp(first);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);
        Assert.AreEqual(second.Start, 4);
    }

    [Test]
    public void InsertTwoElementsInside() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 10);
        Timestamp second = new Timestamp(0, 2, 3);

        vHSData.AddTimestamp(first);
        vHSData.AddTimestamp(second);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(3, timestamps.Count);
        Assert.AreEqual(first, timestamps[0]);
        Assert.AreEqual(second, timestamps[1]);
        Assert.AreEqual(3, second.Stop);
        Assert.AreEqual(3, timestamps[2].Start);
        Assert.AreEqual(10, timestamps[2].Stop);
    }

    [Test]
    public void InsertTwoElementsInsideOverlay() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 10);
        Timestamp second = new Timestamp(0, 2, 3);

        vHSData.AddTimestamp(second);
        vHSData.AddTimestamp(first);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(1, timestamps.Count);
        Assert.AreEqual(first, timestamps[0]);
    }
}
