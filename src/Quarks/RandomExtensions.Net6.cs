#if NET6_0_OR_GREATER

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Codestellation.Quarks;

public partial class RandomExtensions
{
    private const string CompatibleImplementationTypeName = "Net5CompatSeedImpl";

    private static (MemberExpression, MemberExpression, MemberExpression) GetFieldAccessors(ParameterExpression randomParameter)
    {
        MemberExpression implField = Expression.Field(randomParameter, "_impl");
        Type[] randomNestedTypes = typeof(Random).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
        Type compatibleRandomType = randomNestedTypes.Single(x => x.Name == CompatibleImplementationTypeName);
        UnaryExpression castImpl = Expression.Convert(implField, compatibleRandomType);
        MemberExpression prngField = Expression.Field(castImpl, "_prng");

        return GetAccessors(prngField);
    }

    private static Action<Random> GetCheckCompatibility(ParameterExpression randomParameter)
    {
        MemberExpression implField = Expression.Field(randomParameter, "_impl");
        MethodCallExpression getTypeCall = Expression.Call(implField, nameof(GetType), null, null);

        Func<Random, Type> getImplType = Expression.Lambda<Func<Random, Type>>(getTypeCall, randomParameter).Compile();


        return CreateCompatibilityCheck(getImplType);
    }

    private static Action<Random> CreateCompatibilityCheck(Func<Random, Type> getImplType)
        => random =>
        {
            Type randomType = random.GetType();

            if (randomType == typeof(Random)
                && string.Equals(getImplType(random).Name, CompatibleImplementationTypeName, StringComparison.Ordinal))
            {
                return;
            }

            const string message = "This method works with compatible Random class implementation. Use constructor Random(int seed) to create compatible objects";
            // ReSharper disable once NotResolvedInText
            throw new ArgumentException(message, "self");
        };
}
#endif