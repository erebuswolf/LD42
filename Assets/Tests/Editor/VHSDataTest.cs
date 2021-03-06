﻿using System.Collections;
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
        Timestamp first = new Timestamp(0, 0, 4, 6);
        vHSData.AddTimestamp(first);
        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(1, timestamps.Count);
        Assert.AreEqual(first, timestamps[0]);

        Assert.AreEqual(6, first.AnimStart);
        Assert.AreEqual(10, first.AnimStop);
    }

    [Test]
    public void InsertTwoElements() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4, 8);
        Timestamp second = new Timestamp(0, 4, 6, 3);
        vHSData.AddTimestamp(first);
        vHSData.AddTimestamp(second);
        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);

        Assert.AreEqual(8, first.AnimStart);
        Assert.AreEqual(12, first.AnimStop);
        Assert.AreEqual(3, second.AnimStart);
        Assert.AreEqual(5, second.AnimStop);
    }

    [Test]
    public void InsertTwoElementsReversed() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4, 8);
        Timestamp second = new Timestamp(0, 4, 6, 3);

        vHSData.AddTimestamp(second);
        vHSData.AddTimestamp(first);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);
        
        Assert.AreEqual(8, first.AnimStart);
        Assert.AreEqual(12, first.AnimStop);
        Assert.AreEqual(3, second.AnimStart);
        Assert.AreEqual(5, second.AnimStop);
    }

    [Test]
    public void InsertTwoElementsOverlay() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4, 8);
        Timestamp second = new Timestamp(0, 3, 6, 3);

        vHSData.AddTimestamp(first);
        vHSData.AddTimestamp(second);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(2, timestamps.Count);
        Assert.AreEqual(first, timestamps[0]);
        Assert.AreEqual(second, timestamps[1]);
        Assert.AreEqual(3, first.TapeStop);

        Assert.AreEqual(8, first.AnimStart);
        Assert.AreEqual(11, first.AnimStop);
        Assert.AreEqual(3, second.AnimStart);
        Assert.AreEqual(6, second.AnimStop);
    }

    [Test]
    public void InsertTwoElementsOverlayReverse() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 4, 8);
        Timestamp second = new Timestamp(0, 3, 6, 3);

        vHSData.AddTimestamp(second);
        vHSData.AddTimestamp(first);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(timestamps.Count, 2);
        Assert.AreEqual(timestamps[0], first);
        Assert.AreEqual(timestamps[1], second);
        Assert.AreEqual(second.TapeStart, 4);
        
        Assert.AreEqual(8, first.AnimStart);
        Assert.AreEqual(12, first.AnimStop);
        Assert.AreEqual(4, second.AnimStart);
        Assert.AreEqual(6, second.AnimStop);
    }

    [Test]
    public void InsertTwoElementsInside() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 10, 8);
        Timestamp second = new Timestamp(0, 2, 3, 3);

        vHSData.AddTimestamp(first);
        vHSData.AddTimestamp(second);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(3, timestamps.Count);
        Assert.AreEqual(first, timestamps[0]);
        Assert.AreEqual(second, timestamps[1]);
        Assert.AreEqual(3, second.TapeStop);
        Assert.AreEqual(3, timestamps[2].TapeStart);
        Assert.AreEqual(10, timestamps[2].TapeStop);


        Assert.AreEqual(8, first.AnimStart);
        Assert.AreEqual(10, first.AnimStop);
        Assert.AreEqual(3, second.AnimStart);
        Assert.AreEqual(4, second.AnimStop);

        Assert.AreEqual(11, timestamps[2].AnimStart);
        Assert.AreEqual(18, timestamps[2].AnimStop);
    }

    [Test]
    public void InsertTwoElementsInsideOverlay() {
        VHSData vHSData = new VHSData();
        Timestamp first = new Timestamp(0, 0, 10, 8);
        Timestamp second = new Timestamp(0, 2, 3, 3);

        vHSData.AddTimestamp(second);
        vHSData.AddTimestamp(first);

        var timestamps = vHSData.getTimestamps();

        Assert.IsNotNull(timestamps);
        Assert.AreEqual(1, timestamps.Count);
        Assert.AreEqual(first, timestamps[0]);

        Assert.AreEqual(8, first.AnimStart);
        Assert.AreEqual(18, first.AnimStop);
    }
}
