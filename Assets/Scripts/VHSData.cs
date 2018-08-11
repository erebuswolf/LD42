using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VHSData {
    private List<Timestamp> timestamps = new List<Timestamp>();
    public VHSData() {

    }

    public void Print() {
        foreach (Timestamp ts in timestamps) {
            Debug.LogWarning("" + ts.ToString());
        }
    }

    public void RemoveClipsSmallerThan(float length) {
        timestamps.RemoveAll(x => x.GetLength() < length);
    }

    public List<Timestamp> getTimestamps() {
        return timestamps;
    }

    public void AddTimestamp(Timestamp timestamp) {
        int insertIndex = timestamps.Count;
        bool insertSet = false;
        List<Timestamp> ModifyTimeStamps = new List<Timestamp>();
        List<Timestamp> RemoveTimestamps = new List<Timestamp>();

        // Step 1 determine the index to insert at.
        // The index at which new timestamp.start is <= index.start
        // Step 2 create list of timestamps that need removal.
        // Step 3 Create a list of timestamps that need modifying.
        for (int i = 0; i < timestamps.Count; i++) {
            // Check if we insert at this position.
            if (!insertSet && timestamp.TapeStart <= timestamps[i].TapeStart) {
                insertIndex = i;
                insertSet = true;
            }

            // Check if this timestamp should be modified or removed.
            if (timestamp.TapeStart <= timestamps[i].TapeStart && timestamp.TapeStop >= timestamps[i].TapeStop) {
                RemoveTimestamps.Add(timestamps[i]);
            } else if (timestamp.TapeStart < timestamps[i].TapeStop && timestamp.TapeStart > timestamps[i].TapeStart ||
                timestamp.TapeStop < timestamps[i].TapeStop && timestamp.TapeStop > timestamps[i].TapeStart) {
                ModifyTimeStamps.Add(timestamps[i]);
            }
        }
        timestamps.Insert(insertIndex, timestamp);

        foreach(Timestamp ts in RemoveTimestamps) {
            timestamps.Remove(ts);
        }
        
        foreach(Timestamp ts in ModifyTimeStamps) {
            var newStamp = ts.ClipBasedOnOverwrite(timestamp);
            if (newStamp != null) {
                AddTimestamp(newStamp);
            }
        }
    }
}

public class Timestamp {
    public float TapeStart { get; private set; }
    public float TapeStop { get; private set; }
    //TODO: Impliment the anim stuff.
    public float AnimStart { get; private set; }
    public float AnimStop { get; private set; }
    public int Channel { get; private set; }

    public Timestamp(int channel, float tapeStart, float tapeStop, float animStart) {
        TapeStart = tapeStart;
        TapeStop = tapeStop;
        Channel = channel;
        AnimStart = animStart;
        AnimStop = animStart + GetLength();
    }

    public float GetLength() {
        return TapeStop - TapeStart;
    }

    public override string ToString() {
        return "Timestamp " + TapeStart + ", " + TapeStop + ", " + AnimStart + ", " + AnimStop + ", " + Channel;
    }

    public Timestamp ClipBasedOnOverwrite(Timestamp timestamp) {
        // Check if the clip is bisected by the new clip

        if (TapeStart < timestamp.TapeStart && TapeStop > timestamp.TapeStop) {
            float oldStop = TapeStop;
            TapeStop = timestamp.TapeStart;
            AnimStop = AnimStart + GetLength();
            Timestamp newStamp = new Timestamp(Channel, timestamp.TapeStop, oldStop, AnimStop + timestamp.GetLength());
            return newStamp;
        } else if (TapeStart < timestamp.TapeStart) {
            TapeStop = timestamp.TapeStart;
            AnimStop = AnimStart + GetLength();
        } else {
            TapeStart = timestamp.TapeStop;
            AnimStart = AnimStop - GetLength();
        }
        return null;
    }
}