namespace Stuff
{
    public static class CoalescingExtensions
    {
        /// <summary> "Null" coalescing function for all types </summary>        
        public static T CoalesceWith<T>(this T value, T anotherValue)
            => value == null || value.Equals(default(T))
                ? anotherValue
                : value;
    }
}
