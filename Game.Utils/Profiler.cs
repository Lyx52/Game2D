#define PROFILING
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Game.Utils {
    public static class Profiler {
        private static Dictionary<string, Stopwatch> timestamps;
        private static Dictionary<string, List<long>> results;
        private static Dictionary<string, long> memoryStart;
        static Profiler() {
            timestamps = new Dictionary<string, Stopwatch>();
            results = new Dictionary<string, List<long>>();
            memoryStart = new Dictionary<string, long>();
        }
        public static void EndSection(string name) {      
            #if PROFILING
            if (timestamps.ContainsKey(name)) {
                if (timestamps.TryGetValue(name, out Stopwatch stopwatch)) {
                    stopwatch.Stop();
                    if (results.ContainsKey(name)) {
                        results[name].Add(stopwatch.ElapsedTicks);
                    } else {
                        results.Add(name, new List<long>());
                    }
                    if (results[name].Count >= 10) {
                        Logger.Info($"Profiler::Section<{name}> took {Math.Round(results[name].Average() * 0.0001, 4)}ms...");
                        results.Remove(name);
                    }
                }    
            }
            timestamps.Remove(name);
            #endif
        }
        public static void StartSection(string name, bool printStarting=false) {
            #if PROFILING
            if (timestamps.ContainsKey(name))
                return;
            if (printStarting && !results.ContainsKey(name)) {
                Logger.Info($"Profiler::Starting<{name}>");
            }
            timestamps.Add(name, Stopwatch.StartNew());
            #endif
        }
        public static void EndMemoryTracking(string name, bool continueProfiling=false) {
            #if PROFILING
            if (memoryStart.ContainsKey(name)) {
                long bytes = System.GC.GetTotalMemory(false) - memoryStart[name];
                Logger.Info($"Profiler::Section<{name}> took {bytes}bytes of memory...");
                memoryStart.Remove(name);
            }
            #endif
        }
        public static void StartMemoryTracking(string name, bool continueProfiling=false) {
            #if PROFILING
            if (!memoryStart.ContainsKey(name)) {
                Logger.Info($"Profiler::TrakingMemory<{name}>");
                memoryStart.Add(name, System.GC.GetTotalMemory(false));

            } else if (continueProfiling) {
                memoryStart[name] = System.GC.GetTotalMemory(false);
                Logger.Info($"Profiler::ResetingMemoryTracking<{name}>");
            }
            #endif
        }
    }
}