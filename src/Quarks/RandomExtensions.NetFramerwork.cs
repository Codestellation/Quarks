#if !NET6_0_OR_GREATER
using System;
using System.Linq.Expressions;

namespace Codestellation.Quarks;

public partial class RandomExtensions
{
    private static (MemberExpression, MemberExpression, MemberExpression) GetFieldAccessors(ParameterExpression randomParameter) => GetAccessors(randomParameter);

    private static Action<Random> GetCheckCompatibility(ParameterExpression _) => _ => { };
}
#endif