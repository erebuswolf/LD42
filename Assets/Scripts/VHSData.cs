using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VHSData {
    private List<Timestamp> timestamps = new List<Timestamp>();
    public VHSData() {

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
            if (!insertSet && timestamp.Start <= timestamps[i].Start) {
                insertIndex = i;
                insertSet = true;
            }

            // Check if this timestamp should be modified or removed.
            if (timestamp.Start <= timestamps[i].Start && timestamp.Stop >= timestamps[i].Stop) {
                RemoveTimestamps.Add(timestamps[i]);
            } else if (timestamp.Start < timestamps[i].Stop && timestamp.Start > timestamps[i].Start ||
                timestamp.Stop < timestamps[i].Stop && timestamp.Stop > timestamps[i].Start) {
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
    public float Start { get; private set; }
    public float Stop { get; private set; }
    public int Channel { get; private set; }

    public Timestamp(int channel, float start, float stop) {
        Start = start;
        Stop = stop;
        Channel = channel;
    }

    public Timestamp ClipBasedOnOverwrite(Timestamp timestamp) {
        // Check if the clip is bisected by the new clip

        if (Start < timestamp.Start && Stop > timestamp.Stop) {
            Timestamp newStamp = new Timestamp(Channel, timestamp.Stop, Stop);
            Stop = timestamp.Start;
            return newStamp;
        } else if (Start < timestamp.Start) {
            Stop = timestamp.Start;
        } else {
            Start = timestamp.Stop;
        }
        return null;
    }
}