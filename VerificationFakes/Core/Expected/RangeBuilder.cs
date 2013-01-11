using System.Diagnostics.Contracts;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Builder design pattern for creating <see cref="Range"/> objects.
    /// </summary>
    internal sealed class RangeBuilder
    {
        public RangeBuilder(int lowerBound, Range.BoundaryType lowerBoundType)
        {
            LowerBound = lowerBound;
            LowerBoundType = lowerBoundType;

            UpperBound = int.MaxValue;
            UpperBoundType = Range.BoundaryType.Open;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(LowerBound <= UpperBound);
        }

        public int LowerBound { get; private set; }
        public Range.BoundaryType LowerBoundType { get; private set; }

        public int UpperBound { get; private set; }
        public Range.BoundaryType UpperBoundType { get; private set; }


        public static RangeBuilder From(int value)
        {
            Contract.Requires(value >= 0);
            Contract.Ensures(Contract.Result<RangeBuilder>() != null);

            return new RangeBuilder(value, Range.BoundaryType.Close);
        }
        
        public static RangeBuilder FromOpen(int value)
        {
            Contract.Requires(value >= 0);
            Contract.Ensures(Contract.Result<RangeBuilder>() != null);

            return new RangeBuilder(value, Range.BoundaryType.Open);
        }

        public RangeBuilder To(int value)
        {
            Contract.Requires(value >= LowerBound);
            Contract.Ensures(Contract.Result<RangeBuilder>() != null);

            UpperBound = value;

            UpperBoundType = value != int.MaxValue ? Range.BoundaryType.Close : Range.BoundaryType.Open;

            return this;
        }
        
        public RangeBuilder ToOpen(int value)
        {
            Contract.Requires(value >= LowerBound);
            Contract.Ensures(Contract.Result<RangeBuilder>() != null);

            UpperBound = value;

            UpperBoundType = Range.BoundaryType.Open;

            return this;
        }

        public Range Value
        {
            get
            {
                return new Range(LowerBound, LowerBoundType, UpperBound, UpperBoundType);
            }
        }
    }
}