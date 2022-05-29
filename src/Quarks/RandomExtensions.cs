using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Codestellation.Quarks
{
    public static partial class RandomExtensions
    {
        private static readonly Func<Random, int> ReadINext;
        private static readonly Func<Random, int> ReadINextP;
        private static readonly Func<Random, int[]> ReadSeedArray;

        private static readonly Action<Random, int> WriteINext;
        private static readonly Action<Random, int> WriteINextP;

        private static readonly Action<Random> CheckCompatibility;

        static RandomExtensions()
        {
            PrepareFields(out ReadINext, out ReadINextP, out ReadSeedArray, out WriteINext, out WriteINextP, out CheckCompatibility);
        }

        public static byte[] GetState(this Random self)
        {
            EnsureCompatibleImplementation(self);

            var buffer = new byte[sizeof(int) * (1 + 1 + 56)];

            int inext = ReadINext(self);
            Write(inext, buffer, 0);

            int inextp = ReadINextP(self);
            Write(inextp, buffer, sizeof(int));

            int[] seedArray = ReadSeedArray(self);

            Buffer.BlockCopy(seedArray, 0, buffer, 2 * sizeof(int), seedArray.Length * sizeof(int));

            return buffer;
        }

        public static void RestoreState(this Random self, byte[] state)
        {
            EnsureCompatibleImplementation(self);

            int inext = Read(state, 0);
            WriteINext(self, inext);

            int inextp = Read(state, sizeof(int));
            WriteINextP(self, inextp);

            int[] seedArray = ReadSeedArray(self);

            Buffer.BlockCopy(state, 2 * sizeof(int), seedArray, 0, seedArray.Length * sizeof(int));
        }

        private static void EnsureCompatibleImplementation(Random self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            CheckCompatibility(self);
        }

        private static void Write(int value, byte[] buffer, int offset)
        {
            buffer[offset + 0] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        private static int Read(byte[] buffer, int offset) =>
            buffer[offset + 0] |
            (buffer[offset + 1] << 8) |
            (buffer[offset + 2] << 16) |
            (buffer[offset + 3] << 24);

        private static void PrepareFields(out Func<Random, int> readINext,
            out Func<Random, int> readINextP,
            out Func<Random, int[]> readSeedArray,
            out Action<Random, int> writeINext,
            out Action<Random, int> writeINextP,
            out Action<Random> checkCompatibility)
        {
            ParameterExpression randomParameter = Expression.Parameter(typeof(Random));

            checkCompatibility = GetCheckCompatibility(randomParameter);

            (MemberExpression iNextAccessor, MemberExpression iNextPAccessor, MemberExpression seedArrayAccessor) = GetFieldAccessors(randomParameter);

            readINext = CompileReader<int>(randomParameter, iNextAccessor);
            readINextP = CompileReader<int>(randomParameter, iNextPAccessor);
            readSeedArray = CompileReader<int[]>(randomParameter, seedArrayAccessor);

            ParameterExpression intParameter = Expression.Parameter(typeof(int));

            writeINext = CompileWriter(randomParameter, iNextAccessor, intParameter);
            writeINextP = CompileWriter(randomParameter, iNextPAccessor, intParameter);
        }

        private static Action<Random, int> CompileWriter(ParameterExpression randomParameter, MemberExpression accessor, ParameterExpression intParameter)
        {
            BinaryExpression assignField = Expression.Assign(accessor, intParameter);
            return Expression.Lambda<Action<Random, int>>(assignField, randomParameter, intParameter).Compile();
        }

        private static Func<Random, TValue> CompileReader<TValue>(ParameterExpression randomParameter, MemberExpression accessor)
            => Expression.Lambda<Func<Random, TValue>>(accessor, randomParameter).Compile();

        private static (MemberExpression iNextAccessor, MemberExpression iNextPAccessor, MemberExpression seedArrayAccessor) GetAccessors(Expression randomParameter)
        {
            FieldInfo GetField(string name)
            {
                return randomParameter.Type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            }

            FieldInfo inextFieldInfo = GetField("_inext")
                                       ?? GetField("inext")
                                       ?? throw new InvalidOperationException($"Cannot find field inext of {randomParameter.Type}");
            MemberExpression iNextAccessor = Expression.Field(randomParameter, inextFieldInfo);

            FieldInfo inextpFieldInfo = GetField("_inextp")
                                        ?? GetField("inextp")
                                        ?? throw new InvalidOperationException($"Cannot find field inextp of {randomParameter.Type}");
            MemberExpression iNextPAccessor = Expression.Field(randomParameter, inextpFieldInfo);

            FieldInfo seedArrayFieldInfo = GetField("_seedArray")
                                           ?? GetField("SeedArray")
                                           ?? throw new InvalidOperationException($"Cannot fine field SeedArray of {randomParameter.Type}");
            MemberExpression seedArrayAccessor = Expression.Field(randomParameter, seedArrayFieldInfo);

            return (iNextAccessor, iNextPAccessor, seedArrayAccessor);
        }
    }
}