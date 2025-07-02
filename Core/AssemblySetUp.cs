using NUnit.Framework;

// Enable NUnit 3 Parallelisation.
[assembly: Parallelizable(ParallelScope.None)]
[assembly: LevelOfParallelism(0)]
[assembly: Timeout(600000)] //10 mins for long tests