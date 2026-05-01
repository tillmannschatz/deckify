using System;
using Xunit;
using Deckify.Common;

namespace Deckify.Tests
{
    public class SpacingHelpersTests
    {
        [Fact]
        public void CalculateSpacingOffset_Index1_ReturnsZero()
        {
            // First item (index 1) should have offset 0 ( (1-1) * step )
            float result = SpacingHelpers.CalculateSpacingOffset(1, 0.01f);
            Assert.Equal(0f, result);
        }

        [Fact]
        public void CalculateSpacingOffset_Index2_ReturnsOneStep()
        {
            // Second item (index 2) -> (2-1) * 0.01 * 28.34646
            // 0.01 * 28.34646 = 0.2834646
            float result = SpacingHelpers.CalculateSpacingOffset(2, 0.01f);
            Assert.Equal(0.2834646, (double)result, 5); // Cast to double for precision overload
        }

        [Fact]
        public void CalculateSpacingOffset_Index3_ReturnsTwoSteps()
        {
            // Third item (index 3) -> (3-1) * 0.01 * 28.34646
            float result = SpacingHelpers.CalculateSpacingOffset(3, 0.01f);
            Assert.Equal(0.5669292, (double)result, 5);
        }

        [Fact]
        public void CalculateAdjacentPosition_SumsCorrectly()
        {
            float prevPos = 100f;
            float prevDim = 50f;
            float result = SpacingHelpers.CalculateAdjacentPosition(prevPos, prevDim);
            Assert.Equal(150f, result);
        }
        
        [Fact]
        public void CalculateAdjacentPosition_NegativePreviousPosition_SumsCorrectly()
        {
            // Check off-slide calculation works
            float prevPos = -50f;
            float prevDim = 50f;
            float result = SpacingHelpers.CalculateAdjacentPosition(prevPos, prevDim);
            Assert.Equal(0f, result);
        }

        [Fact]
        public void CalculateAdjacentPosition_ZeroDimension_SumsCorrectly()
        {
            float prevPos = 100f;
            float prevDim = 0f;
            float result = SpacingHelpers.CalculateAdjacentPosition(prevPos, prevDim);
            Assert.Equal(100f, result);
        }
    }
}
