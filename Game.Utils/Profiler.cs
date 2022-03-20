using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Game.Utils {
    public class Profiler {
        private Dictionary<string, Stopwatch> timestamps;
        private Dictionary<string, List<long>> results;
        private Dictionary<string, long> memoryStart;
        public bool isEnabled = true;
        public bool loggingEnabled = true;
        public Profiler() {
            this.timestamps = new Dictionary<string, Stopwatch>();
            this.results = new Dictionary<string, List<long>>();
            this.memoryStart = new Dictionary<string, long>();
        }
        public void EndSection(string name) {
            if (this.isEnabled) {
                if (this.timestamps.ContainsKey(name)) {
                    if (this.timestamps.TryGetValue(name, out Stopwatch stopwatch)) {
                        stopwatch.Stop();
                        if (this.results.ContainsKey(name)) {
                            this.results[name].Add(stopwatch.ElapsedTicks);
                        } else {
                            this.results.Add(name, new List<long>());
                        }
                        if (this.results[name].Count >= 100 && this.loggingEnabled) {
                            GameHandler.Logger.Info($"Profiler::Section<{name}> took {Math.Round(this.results[name].Average() * 0.0001, 4)}ms...");
                            this.results.Remove(name);
                        }
                    }    
                }
            }
            this.timestamps.Remove(name);
        }
        public void StartSection(string name, bool printStarting=false) {
            if (this.isEnabled) {
                if (this.timestamps.ContainsKey(name))
                    return;

                if (printStarting && this.loggingEnabled && !this.results.ContainsKey(name)) {
                    GameHandler.Logger.Info($"Profiler::Starting<{name}>");
                }
                this.timestamps.Add(name, Stopwatch.StartNew());
            }
        }
        public void EndMemoryTracking(string name, bool continueProfiling=false) {
            if (this.isEnabled) {
                if (this.memoryStart.ContainsKey(name)) {
                    if (this.loggingEnabled) {
                        long bytes = System.GC.GetTotalMemory(false) - this.memoryStart[name];
                        GameHandler.Logger.Info($"Profiler::Section<{name}> took {bytes}bytes of memory...");
                        this.memoryStart.Remove(name);
                    } 
                }
            }
        }
        public void StartMemoryTracking(string name, bool continueProfiling=false) {
            if (this.isEnabled) {
                if (!this.memoryStart.ContainsKey(name)) {
                    if (this.loggingEnabled) {
                        GameHandler.Logger.Info($"Profiler::TrakingMemory<{name}>");
                    }
                    this.memoryStart.Add(name, System.GC.GetTotalMemory(false));

                } else if (continueProfiling) {
                    this.memoryStart[name] = System.GC.GetTotalMemory(false);
                    if (this.loggingEnabled) {
                        GameHandler.Logger.Info($"Profiler::ResetingMemoryTracking<{name}>");
                    }
                }
            }    
        }
    }
}