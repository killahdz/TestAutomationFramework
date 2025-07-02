using NUnit.Framework;

// Enable NUnit 3 Parallelisation.
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]
[assembly: Timeout(600000)] //10 mins for long tests

