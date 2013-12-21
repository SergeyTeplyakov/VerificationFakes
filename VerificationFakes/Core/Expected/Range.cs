using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace VerificationFakes.Core
{
    /// <summary> 
    /// Represents a value range.
    /// </summary> 
    /// <remarks> 
    /// This class is used for xml serialization/deserialization. This means that we can't use any 
    /// argument validation in property setters because this could lead to crashes during data 
    /// deserialization. 
    /// </remarks> 
    internal sealed class Range : IEquatable<Range>
    {
        public enum BoundaryType
        {
            Open,
            Close
        };

        public int LowerBound { get; private set; }

        public BoundaryType LowerBoundType { get; set; }

        public int UpperBound { get; private set; }

        public BoundaryType UpperBoundType { get; set; }

        public bool IsSingleValue
        {
            get
            {
                return LowerBound == UpperBound && LowerBoundType == BoundaryType.Close &&
                       UpperBoundType == BoundaryType.Close;
            }
        }

        public Range(int lowerBound, BoundaryType lowerBoundType, 
                     int upperBound, BoundaryType upperBoundType)
        {
            if (!IsValid(lowerBound, lowerBoundType, upperBound, upperBoundType))
                throw new ArgumentException("Invalid range. Please check the values.");
            Contract.EndContractBlock();

            LowerBound = lowerBound;
            LowerBoundType = lowerBoundType;
            UpperBound = upperBound;
            UpperBoundType = upperBoundType;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(LowerBound <= UpperBound,
                "Upper bound should not be greater than lower bound.");
        }

        /// <summary> 
        /// Parses specified <paramref name="range"/> and returns a valid <see cref="Range"/>. 
        /// </summary> 
        /// <remarks> 
        /// This method is used for testing purposes only and <paramref name="range"/> should be 
        /// in following format: [-Inf,2), (-Inf,+Inf), (12,14] 
        /// </remarks> 
        public static Range Parse(string range)
        {
            Contract.Requires(range != null, "range should not be null.");

            string pattern =
                @"(?<left_bound_type>[\(\[])" +
                @"((?<left_bound>-?\d+)|(?<left_bound>[+-]?Inf))" +
                @",\s*" +
                @"((?<right_bound>-?\d+)|(?<right_bound>[+-]?Inf))" +
                @"(?<right_bound_type>[\)\]])";

            var match = Regex.Match(range, pattern);
            if (!match.Success)
                throw new FormatException("Range format is invalid. Please check specified range string.");

            string leftBoundType = match.Groups["left_bound_type"].Value;
            string leftBound = match.Groups["left_bound"].Value;

            string rightBoundType = match.Groups["right_bound_type"].Value;
            string rightBound = match.Groups["right_bound"].Value;


            try
            {
                return new Range(ParseRangeValue(leftBound), ParseBoundaryType(leftBoundType),
                                    ParseRangeValue(rightBound), ParseBoundaryType(rightBoundType));
            }
            catch (ArgumentException e)
            {
                throw new FormatException(e.Message, e);
            }
        }

        public static bool TryParse(string range, out Range result)
        {
            try
            {
                result = Parse(range);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

        /// <summary> 
        /// Returns true if arguments are valid values for creating value range. 
        /// </summary> 
        [Pure]
        public static bool IsValid(int lowerBound, BoundaryType lowerBoundType,
                                   int upperBound, BoundaryType upperBoundType)
        {
            if (lowerBound > upperBound)
                return false;

            // Lower bound and upper bound could be equals only when lower bound or upper bound 
            // are closed. 
            if (lowerBound == upperBound &&
                (lowerBoundType == BoundaryType.Open || upperBoundType == BoundaryType.Open))
                return false;

            return true;
        }

        /// <summary> 
        /// Returns true if the <paramref name="other"/> overlaps current Range object. 
        /// </summary> 
        public bool Overlaps(Range other)
        {
            Contract.Requires(other != null, "other should not be null.");

            return CurrentRange.OverlapsWith(other.CurrentRange);
        }

        /// <summary> 
        /// Returns true if the <paramref name="value"/> is inside current range object. 
        /// </summary> 
        public bool Contains(int value)
        {
            // value is belong to current range when current range overlaps 
            // with [value, value] range. 
            var valueRange = new RangeImpl(new Bound(value), new Bound(value));
            return CurrentRange.OverlapsWith(valueRange);
        }

        public override string ToString()
        {
            // Single value range we should treat separately
            // and print "1", for example.
            if (IsSingleValue)
            {
                return LowerBound.ToString();
            }

            var sb = new StringBuilder();
            sb.Append((LowerBoundType == BoundaryType.Open ? "(" : "["));

            var lowerBound = double.IsNegativeInfinity(LowerBound)
                                 ? "-Inf."
                                 : LowerBound.ToString(CultureInfo.InvariantCulture);
            var upperBound = double.IsPositiveInfinity(UpperBound)
                                 ? "Inf."
                                 : UpperBound.ToString(CultureInfo.InvariantCulture);

            sb.AppendFormat("{0} - {1}", lowerBound, upperBound);
            sb.Append(UpperBoundType == BoundaryType.Open ? ")" : "]");
            return sb.ToString();
        }

        #region Inner classes

        /// <summary> 
        /// Helper struct that represents left or right range bound. 
        /// </summary> 
        private struct Bound
        {
            public readonly int Value;
            public readonly bool Closed;

            public Bound(int value, bool closed = true)
            {
                Value = value;
                Closed = closed;
            }
        }

        /// <summary> 
        /// Represents simple range of values 
        /// </summary> 
        private struct RangeImpl
        {
            public RangeImpl(Bound left, Bound right)
            {
                Left = left;
                Right = right;
            }

            public readonly Bound Left;
            public readonly Bound Right;

            public static Bound LeastUpperBound(Bound lhs, Bound rhs)
            {
                // First we checking equality, because AreEquals uses 
                // some precision (like 0.0001) 
                if (lhs.Value == rhs.Value)
                {
                    // Closed bound is greater than open bound. 
                    // That is, "1]" is greater than "1)". 
                    if (lhs.Closed)
                        return rhs;

                    return lhs;
                }

                // Bounds are not equal, using simple comparison 
                if (lhs.Value < rhs.Value)
                    return lhs;

                return rhs;
            }

            public static Bound MostLowerBound(Bound lhs, Bound rhs)
            {
                // First we checking equality
                if (lhs.Value == rhs.Value)
                {
                    // Closed bound is greater than open bound. 
                    // That is, "[1" is greater than "(1". 
                    if (lhs.Closed)
                        return rhs;

                    return lhs;
                }

                // Bounds are not equal, using simple comparison 
                if (lhs.Value > rhs.Value)
                    return lhs;

                return rhs;
            }

            public static bool IsValid(Bound left, Bound right)
            {
                // Lower bound and upper bound could be equals only when lower bound or upper bound 
                // are closed. 
                if (left.Value == right.Value)
                {
                    return (left.Closed && right.Closed);
                }

                return left.Value < right.Value;
            }

            /// <summary> 
            /// Returns true when <paramref name="other"/> overlaps with current range instance. 
            /// </summary> 
            public bool OverlapsWith(RangeImpl other)
            {
                return IsValid(MostLowerBound(Left, other.Left), LeastUpperBound(Right, other.Right));
            }
        }

        #endregion Inner classes


        private RangeImpl CurrentRange
        {
            get { return new RangeImpl(LeftBound, RightBound); }
        }

        private Bound LeftBound
        {
            get { return new Bound(LowerBound, LowerBoundType == BoundaryType.Close); }
        }

        private Bound RightBound
        {
            get { return new Bound(UpperBound, UpperBoundType == BoundaryType.Close); }
        }

        private static BoundaryType ParseBoundaryType(string boundaryType)
        {
            if (boundaryType == "(" || boundaryType == ")")
                return BoundaryType.Open;
            return BoundaryType.Close;
        }

        private static int ParseRangeValue(string range)
        {
            if (range == "-Inf")
                return int.MinValue;
            if (range == "Inf" || range == "+Inf")
                return int.MaxValue;

            return int.Parse(range);
        }

        public bool Equals(Range other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return LowerBound.Equals(other.LowerBound) 
                && LowerBoundType.Equals(other.LowerBoundType) 
                && UpperBound.Equals(other.UpperBound) 
                && UpperBoundType.Equals(other.UpperBoundType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            
            return Equals((Range)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // TODO: we're not using a bound type yet!
                int hashCode = LowerBound.GetHashCode();
                hashCode = (hashCode * 397) ^ UpperBoundType.GetHashCode();
                return hashCode;
            }
        }
    }
}