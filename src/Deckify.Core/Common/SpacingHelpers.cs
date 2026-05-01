using System;

namespace Deckify.Common
{
    public static class SpacingHelpers
    {
        // 1 cm = 28.34646 points
        public const float PointsPerCm = 28.34646f;
        public const float DefaultSpacingStep = 0.01f; // 0.01 cm

        /// <summary>
        /// Calculates the offset to add to a shape's position to increase/decrease spacing.
        /// Formula: (Index - 1) * SpacingStep * PointsPerCm
        /// </summary>
        /// <param name="index">The 1-based index of the shape in the sorted list (logical position).</param>
        /// <param name="spacingStepInCm">The step size in cm (can be negative for decrease).</param>
        /// <returns>The offset in points.</returns>
        public static float CalculateSpacingOffset(int index, float spacingStepInCm)
        {
            // The original loop used 'i' from 2 to Count.
            // i=2 (second item) -> (2 - 1) * ...
            // We expect 'index' to be the 1-based rank passed from the service loop.
            return (float)((index - 1) * spacingStepInCm * PointsPerCm);
        }

        /// <summary>
        /// Calculates the position for an item to be strictly adjacent to the previous one.
        /// </summary>
        public static float CalculateAdjacentPosition(float previousPosition, float previousDimension)
        {
            return previousPosition + previousDimension;
        }
    }
}
