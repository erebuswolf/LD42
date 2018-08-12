using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VHSData {
    private List<Timestamp> timestamps = new List<Timestamp>();
    public VHSData() {
        initSolutions();
    }

    public void Wipe() {
        timestamps = new List<Timestamp>();
    }

    public void FastLoad(string data) {
        timestamps.Clear();
        var stamps = data.Split('-');
        Debug.LogWarningFormat(" got {0}", stamps.Length);
        float Tapetime = 0;
        foreach (string s in stamps) {
            var stamp = s.Split(',');
            if (stamp.Length != 2) {
                Debug.LogWarningFormat("expected 2 elements, got {0} in {1}", stamp.Length, s);
                continue;
            }
            float AnimStart = float.Parse(stamp[0]);
            float AnimStop = float.Parse(stamp[1]);
            float TapeStart = Tapetime;
            AddTimestamp(new Timestamp(0, TapeStart, TapeStart + (AnimStop - AnimStart), AnimStart));
            Tapetime = TapeStart + (AnimStop - AnimStart);
        }
    }

    public void Load(string data) {
        timestamps.Clear();
        var stamps = data.Split('-');
        Debug.LogWarningFormat(" got {0}", stamps.Length);
        foreach (string s in stamps) {
            var stamp = s.Split(',');
            if (stamp.Length != 4) {
                Debug.LogWarningFormat("expected 4 elements, got {0} in {1}", stamp.Length, s);
                continue;
            }
            float AnimStart = float.Parse(stamp[0]);
            float AnimStop = float.Parse(stamp[1]);
            float TapeStart = float.Parse(stamp[2]);
            int Channel = int.Parse(stamp[3]);
            AddTimestamp(new Timestamp(Channel, TapeStart, TapeStart + (AnimStop - AnimStart), AnimStart));
        }
    }

    public Timestamp GetTimestampAtHead(float head) {
        foreach(Timestamp ts in timestamps) {
            if (head < ts.TapeStop && head >= ts.TapeStart) {
                return ts;
            }
        }
        return null;
    }

    public void Print() {
        foreach (Timestamp ts in timestamps) {
            Debug.LogWarning("" + ts.ToString());
        }
    }

    public void RemoveClipsSmallerThan(float length) {
        timestamps.RemoveAll(x => x.GetLength() < length && x.Channel != -1);
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
    public string SerializeToString() {
        string output = "";
        foreach (Timestamp ts in timestamps) {
            output += ts.SerializeToString() + "-";
        }
        return output;
    }

    private List<List<KeyValuePair<float, float>>> solutions = new List<List<KeyValuePair<float, float>>>();

    private void initSolutions() {
        List<KeyValuePair<float, float>> solution = new List<KeyValuePair<float, float>> {
            new KeyValuePair<float, float>(11, 16.8f),
            new KeyValuePair<float, float>(0, 6f)
        };
        solutions.Add(solution);

        solution = new List<KeyValuePair<float, float>> {
            new KeyValuePair<float, float>(27.8f, 30.1f),
            new KeyValuePair<float, float>(78.8f, 81.2f)
        };
        solutions.Add(solution);

        solution = new List<KeyValuePair<float, float>> {
            new KeyValuePair<float, float>(63.5f, 68f),
            new KeyValuePair<float, float>(57.5f, 61f)
        };
        solutions.Add(solution);
        
        solution = new List<KeyValuePair<float, float>> {
            new KeyValuePair<float, float>(38.1f, 40.84f),
            new KeyValuePair<float, float>(21.2f, 22.5f),
            new KeyValuePair<float, float>(50.77f, 52.5f)
        };
        solutions.Add(solution);
        
        solution = new List<KeyValuePair<float, float>> {
            new KeyValuePair<float, float>(38.1f, 40.84f),
            new KeyValuePair<float, float>(76.4f, 78.8f),
            new KeyValuePair<float, float>(76.4f, 78.8f),
            new KeyValuePair<float, float>(50.77f, 52.5f)
        };
        solutions.Add(solution);
        
        solution = new List<KeyValuePair<float, float>> {
            new KeyValuePair<float, float>(31f, 35.2f),
            new KeyValuePair<float, float>(46.07f, 48.2f),
            new KeyValuePair<float, float>(79f, 84f),
            new KeyValuePair<float, float>(56f, 59f)
        };
        solutions.Add(solution);
    }

    public int IsSolution() {
        for (int i = 0; i < solutions.Count; i++) {
            if (CheckSolution(solutions[i], i==4)) {
                return i;
            }
        }
        return -1;
    }
    private bool CheckSolution(List<KeyValuePair<float, float>> sol, bool allowOverlap) {
        int LastOverlap = allowOverlap ? 0: - 1;
        for (int j = 0; j < sol.Count; j++) {
            bool hasOverlap = false;
            for (int i = allowOverlap ? LastOverlap : LastOverlap + 1; i < timestamps.Count; i++) {
                if (timestamps[i].Overlaps(sol[j].Key, sol[j].Value)) {
                    LastOverlap = i;
                    hasOverlap = true;
                }
            }
            if (!hasOverlap) {
                return false;
            }
        }
        if (sol.Count > 0) {
            return true;
        }
        return false;
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

    public bool Overlaps(float animStart, float animEnd) {
        return !(animStart < this.AnimStart && animEnd < this.AnimStart 
            || animStart > this.AnimStop && animEnd > this.AnimStop);
    }

    public float GetLength() {
        return TapeStop - TapeStart;
    }

    public string SerializeToString() {
        return AnimStart + " , " + AnimStop + " , " + TapeStart + " , " + Channel;
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