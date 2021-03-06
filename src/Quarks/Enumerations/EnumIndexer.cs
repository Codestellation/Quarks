using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Codestellation.Quarks.Enumerations
{
    public class EnumIndexer<TEnum, TValue>
    {
        private static readonly int ArraySize;
        private static readonly Func<TEnum, int> ConvertToInt;
        private TValue[] _valueArray;
        private static readonly int Offset;

        static EnumIndexer()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("Type parameter TEnum must be an Enum");
            }

            IEnumerable<TEnum> members = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            int maxValue = members.Select(x => Convert.ToInt32(x)).Max();
            int minValue = members.Select(x => Convert.ToInt32(x)).Min();

            Offset = -minValue;
            ArraySize = maxValue - minValue + 1;

            ParameterExpression enumValue = Expression.Parameter(typeof(TEnum));
            UnaryExpression castToInt = Expression.Convert(enumValue, typeof(int));
            ConvertToInt = Expression.Lambda<Func<TEnum, int>>(castToInt, enumValue).Compile();
        }

        public EnumIndexer()
        {
            InitializeArray();
        }

        private void InitializeArray() => _valueArray = new TValue[ArraySize];

        public TValue this[TEnum index]
        {
            get
            {
                int intIndex = ConvertToInt(index) + Offset;
                return _valueArray[intIndex];
            }
            set
            {
                int intIndex = ConvertToInt(index) + Offset;
                _valueArray[intIndex] = value;
            }
        }

        public TValue[] GetValues() => _valueArray;
    }
}