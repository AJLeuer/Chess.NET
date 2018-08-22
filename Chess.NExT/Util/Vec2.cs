using System;
using SFML.System;

namespace System
{
    interface IIndexable<N>
    {
        N this[uint index] {get; set;}
    }
}

namespace Chess.Util
{
    public struct Vec2<N> : IEquatable<Vec2<N>>, IIndexable<N> where N:
        struct,
        IComparable, 
        IComparable<N>, 
        IConvertible, 
        IEquatable<N>, 
        IFormattable
    {

        public N x { get; set; } 
        public N y { get; set; } 

        public Vec2(Vec2<N> other) :
            this(other.x, other.y)
        {
            
        }

        public Vec2(N x, N y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static implicit operator Vec2<N>(Vector2f sfmlVector)
        {
            return new Vec2<N> {x = (dynamic) sfmlVector.X, y = (dynamic) sfmlVector.Y};
        }
        
        public static implicit operator Vec2<N>(Vector2i sfmlVector)
        {
            return new Vec2<N> {x = (dynamic) sfmlVector.X, y = (dynamic) sfmlVector.Y};
        }
        
        public static implicit operator Vec2<N>(Vector2u sfmlVector)
        {
            return new Vec2<N> {x = (dynamic) sfmlVector.X, y = (dynamic) sfmlVector.Y};
        }
        
        public static implicit operator Vector2f(Vec2<N> vector)
        {
            return new Vector2f {X = (dynamic) vector.x, Y = (dynamic) vector.y};
        }
        
        public static implicit operator Vector2i(Vec2<N> vector)
        {
            return new Vector2i {X = (dynamic) vector.x, Y = (dynamic) vector.y};
        }
        
        public static implicit operator Vector2u(Vec2<N> vector)
        {
            return new Vector2u {X = (dynamic) vector.x, Y = (dynamic) vector.y};
        }

        public Vec2<OtherNumericType> ConvertMemberType <OtherNumericType>() where OtherNumericType : 
            struct, IComparable, IComparable<OtherNumericType>, IConvertible, IEquatable<OtherNumericType>, IFormattable
        {
            return new Vec2<OtherNumericType>((OtherNumericType) (dynamic) x, (OtherNumericType) (dynamic) y);
        }

        public N this[uint index]
        {
            get
            {
                if (index == 0)
                {
                    return x;
                }
                else if (index == 1)
                {
                    return y;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                if (index == 0)
                {
                    x = value;
                }
                else if (index == 1)
                {
                    y = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public static Boolean operator == (Vec2<N> vector0, Vec2<N> vector1)
        {
            return ((dynamic) vector0.x == (dynamic) vector1.x) &&
                   ((dynamic) vector0.y == (dynamic) vector1.y);
        }

        public static bool operator != (Vec2<N> vector0, Vec2<N> vector1)
        {
            return !(vector0 == vector1);
        }

        public override Boolean Equals(object @object)
        {
            if (@object?.GetType() == this.GetType())
            {
                return this.Equals((Vec2<N>) @object);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool Equals(Vec2<N> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }

        public static Vec2<N> operator + (Vec2<N> vector0, Vec2<N> vector1)
        {
            return new Vec2<N>((dynamic) vector0.x + vector1.x, (dynamic) vector0.y + vector1.y);
        }
        
        public static Vec2<long> operator + (Vec2<N> vector0, Vec2<short> vector1)
        {
            return new Vec2<long>((dynamic)vector0.x + vector1.x, (dynamic)vector0.y + vector1.y);
        }

        public static Vec2<N> operator - (Vec2<N> vector0, Vec2<N> vector1)
        {
            return new Vec2<N>((dynamic) vector0.x - vector1.x, (dynamic) vector0.y - vector1.y);
        }
        
        public static Vec2<N> operator * (Vec2<N> vector, uint n)
        {
            return new Vec2<N>((dynamic) vector.x * n, (dynamic) vector.y * n);
        }
        
        public static Vec2<N> operator / (Vec2<N> vector, N n)
        {
            return new Vec2<N>((dynamic) vector.x / n, (dynamic) vector.y / n);
        }

    }
    
    
}