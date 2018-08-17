using System;

namespace Chess.Util
{
    /**
     * Optional container class, similar to one provided in Java
     * 
     * Code credit stackoverflow: https://stackoverflow.com/questions/16199227/optional-return-in-c-net
     */
    public struct Optional<T>
    {
        public bool HasValue
        {
            get { return value != null; }
        }

        private T value;
        
        public T Value
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

        public static explicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }
        
        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Optional<T>)
            {
                return this.Equals((Optional<T>) obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Optional<T> other)
        {
            if (HasValue && other.HasValue)
            {
                return Equals(value, other.value);
            }
            else
            {
                return HasValue == other.HasValue;
            }
        }
    }
}