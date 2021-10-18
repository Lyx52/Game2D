using System.Diagnostics;
using System.Collections.Generic;

namespace Game.Utils {
    public class Profiler {
        private Dictionary<string, Stopwatch> timestamps;
        private Dictionary<string, long> results;
        public bool isEnabled = true;
        public bool loggingEnabled = true;
        public Profiler() {
            this.timestamps = new Dictionary<string, Stopwatch>();
            this.results = new Dictionary<string, long>();
        }
        public void EndSection(string name, bool continueProfiling=false) {
            if (this.isEnabled) {
                if (this.results.ContainsKey(name)) {
                    if (this.timestamps.ContainsKey(name)) {
                        if (this.timestamps.TryGetValue(name, out Stopwatch stopwatch)) {
                            stopwatch.Stop();
                            if (!this.results.TryAdd(name, stopwatch.ElapsedTicks)) {
                                this.results.Remove(name);
                                this.results.Add(name, stopwatch.ElapsedTicks);
                            }
                            if (this.loggingEnabled) {
                                GameHandler.Logger.Info($"Profiler::Section<{name}> took {stopwatch.Elapsed}ms...");
                            }
                        }    
                    }
                }
            }
        }
        public void StartSection(string name, bool continueProfiling=false) {
            if (this.isEnabled) {
                if (!this.timestamps.ContainsKey(name)) {
                    if (this.loggingEnabled) {
                        GameHandler.Logger.Info($"Profiler::Starting<{name}>");
                    }
                    this.timestamps.Add(name, Stopwatch.StartNew());

                } else if (continueProfiling) {
                    this.timestamps.TryGetValue(name, out Stopwatch stopwatch);
                    stopwatch.Restart();
                    if (this.loggingEnabled) {
                        GameHandler.Logger.Info($"Profiler::Reseting<{name}>");
                    }
                }
            }
        }
    }
}