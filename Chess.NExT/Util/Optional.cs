using System;

namespace Chess.Utility
{
    /**
     * Optional container class, similar to one provided in Java
     * 
     * Code credit stackoverflow: https://stackoverflow.com/questions/16199227/optional-return-in-c-net
     */
    public struct Optional<T> : IEquatable<Optional<T>> where T : class
    {
        public bool HasValue
        {
            get { return value != null; }
        }

        private T value;
        
        public T Object
        {
            get
            {
                if (HasValue)
                {
                    return value;
                }

                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public Optional(T value)
        {
            this.value = value;
        }

        public static Optional<T> Empty 
        {
            get { return new Optional<T>(null); }
        }

        public static explicit operator T(Optional<T> optional)
        {
            return optional.Object;
        }
        
        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static bool operator == (Optional<T> optional0, Optional<T> optional1)
        {
            if (optional0.HasValue && optional1.HasValue)
            {
                return Equals(optional0.value, optional1.value);
            }
            else
            {
                return optional0.HasValue == optional1.HasValue;
            }
        }

        public static bool operator != (Optional<T> optional0, Optional<T> optional1)
        {
            return !(optional0 == optional1);
        }

        public override bool Equals(object obj)
        {
            if (obj is Optional<T> optional)
            {
                return this.Equals(optional);
            }
            else
            {
                return false;
            }
        }
        
        public bool Equals(Optional<T> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}