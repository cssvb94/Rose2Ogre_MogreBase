using System.Runtime.CompilerServices;

namespace Math3d
{
    internal static class HashCodeHelper
    {
        /// <summary>
        /// Combines two hash codes, useful for combining hash codes of individual vector elements
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombineHashCodes(int h1, int h2) => (((h1 << 5) + h1) ^ h2);
    }
}
