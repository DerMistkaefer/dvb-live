using System;
using DerMistkaefer.DvbLive.Backend.Cache.Data;
using FluentAssertions;
using Xunit;

namespace DerMistkaefer.DvbLive.Backend.UnitTests.Cache.Data
{
    /// <summary>
    /// Unit Tests for <see cref="CachedTripStop"/>
    /// </summary>
    public class CachedTripStopTests
    {
        /// <summary>
        /// Check if the <see cref="CachedTripStop.TriasIdStopPoint"/> return the correct value.
        /// </summary>
        [Theory]
        [InlineData("de:14612:75:1:5:10", "de:14612:75")]
        [InlineData("de:14612:75:1:5", "de:14612:75")]
        [InlineData("de:14612:75:1", "de:14612:75")]
        [InlineData("de:14612:75", "de:14612:75")]
        [InlineData("de:14612", "de:14612")]
        public void Property_TriasIdStopPoint_Work(string input, string output)
        {
            var testClass = new CachedTripStop(input);

            var result = testClass.TriasIdStopPoint;

            result.Should().Be(output);
        }
    }
}