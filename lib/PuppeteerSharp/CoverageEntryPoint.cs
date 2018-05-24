﻿using System;
using static PuppeteerSharp.Messaging.ProfilerTakePreciseCoverageResponse;

namespace PuppeteerSharp
{
    internal class CoverageEntryPoint : IComparable<CoverageEntryPoint>
    {
        public int Offset { get; internal set; }
        public int Type { get; internal set; }
        public ProfilerTakePreciseCoverageResponseRange Range { get; internal set; }

        public int CompareTo(CoverageEntryPoint other)
        {
            // Sort with increasing offsets.
            if (Offset != other.Offset)
            {
                return Offset - other.Offset;
            }

            // All "end" points should go before "start" points.
            if (Type != other.Type)
            {
                return Type - other.Type;
            }

            var aLength = Range.EndOffset - Range.StartOffset;
            var bLength = other.Range.EndOffset - other.Range.StartOffset;
            // For two "start" points, the one with longer range goes first.
            if (Type == 0)
            {
                return bLength - aLength;
            }
            // For two "end" points, the one with shorter range goes first.
            return aLength - bLength;
        }
    }
}