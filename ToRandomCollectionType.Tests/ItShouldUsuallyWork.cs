using System;
using System.Collections.Generic;
using FluentAssertions;
using Nerdhold;
using NUnit.Framework;

namespace ToRandomCollectionType.Tests
{
    public class ItShouldUsuallyWork
    {
        [Test]
        public void TryingAFewTimes_SometimesWorks()
        {
            var inputSet = new List<int> {1, 2, 3};

            int successCount = 0;
            for (int i = 0; i < 10; i++)
            {
                var result = inputSet.ToRandomCollectionType();
                try
                {
                    inputSet.Should().BeEquivalentTo(result);
                    Console.WriteLine($"Got a {result.GetType()}");
                    successCount++;
                }
                catch
                {
                    Console.WriteLine("Looks like that one failed.");
                }
            }

            successCount.Should().BeGreaterOrEqualTo(5);
        }
    }
}
