using System;
using SFML.System;


namespace System
{
    public interface IIndexable<N>
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

        public N X { get; set; } 
        public N Y { get; set; } 

        public Vec2(Vec2<N> other) :
            this(other.X, other.Y)
        {
            
        }

        public Vec2(N x, N y)
        {
            this.X = x;
            this.Y = y;
        }
        
        public static implicit operator Vec2<N>(Vector2f sfmlVector)
        {
            return new Vec2<N> {X = (dynamic) sfmlVector.X, Y = (dynamic) sfmlVector.Y};
        }
        
        public static implicit operator Vec2<N>(Vector2i sfmlVector)
        {
            return new Vec2<N> {X = (dynamic) sfmlVector.X, Y = (dynamic) sfmlVector.Y};
        }
        
        public static implicit operator Vec2<N>(Vector2u sfmlVector)
        {
            return new Vec2<N> {X = (dynamic) sfmlVector.X, Y = (dynamic) sfmlVector.Y};
        }
        
        public static implicit operator Vector2f(Vec2<N> vector)
        {
            return new Vector2f {X = (dynamic) vector.X, Y = (dynamic) vector.Y};
        }
        
        public static implicit operator Vector2i(Vec2<N> vector)
        {
            return new Vector2i {X = (dynamic) vector.X, Y = (dynamic) vector.Y};
        }
        
        public static implicit operator Vector2u(Vec2<N> vector)
        {
            return new Vector2u {X = (dynamic) vector.X, Y = (dynamic) vector.Y};
        }

        public Vec2<OtherNumericType> ConvertMemberType <OtherNumericType>() where OtherNumericType : 
            struct, 
            IComparable, 
            IComparable<OtherNumericType>, 
            IConvertible, 
            IEquatable<OtherNumericType>, 
            IFormattable
        {
            return new Vec2<OtherNumericType>((OtherNumericType) (dynamic) X, (OtherNumericType) (dynamic) Y);
        }

        public N this[uint index]
        {
            get
            {
                if (index == 0)
                {
                    return X;
                }
                else if (index == 1)
                {
                    return Y;
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
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public static Boolean operator == (Vec2<N> vector0, Vec2<N> vector1)
        {
            return ((dynamic) vector0.X == (dynamic) vector1.X) &&
                   ((dynamic) vector0.Y == (dynamic) vector1.Y);
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
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static Vec2<N> operator + (Vec2<N> vector0, Vec2<N> vector1)
        {
            return new Vec2<N>(checked((dynamic)vector0.X + (dynamic)vector1.X), checked((dynamic) vector0.Y + (dynamic) vector1.Y));
        }
        
        public static Vec2<long> operator + (Vec2<N> vector0, Vec2<short> vector1)
        {
            Vec2<int> v0 = vector0.ConvertMemberType<int>();
            return new Vec2<long>(v0.X + vector1.X, v0.Y + vector1.Y);
        }

        public static Vec2<N> operator - (Vec2<N> vector0, Vec2<N> vector1)
        {
            return new Vec2<N>(checked((dynamic)vector0.X - (dynamic)vector1.X), checked((dynamic) vector0.Y - (dynamic) vector1.Y));
        }
        
        public static Vec2<N> operator * (Vec2<N> vector, N n)
        {
            N x = (N) ((dynamic) vector.X * n);
            N y = (N) ((dynamic) vector.Y * n);
            
            return new Vec2<N>(x, y);
        }
        
        public static Vec2<N> operator / (Vec2<N> vector, N n)
        {
            N x = (N) ((dynamic) vector.X / n);
            N y = (N) ((dynamic) vector.Y / n);
            
            return new Vec2<N>(x, y);
        }

        public static double Distance(Vec2<N> point0, Vec2<N> point1)
        {
            Vec2<double> p0 = point0.ConvertMemberType<Double>();
            Vec2<double> p1 = point1.ConvertMemberType<Double>();

            return distance(p0, p1);
        }


        public static double Distance<M>(Vec2<N> point0, Vec2<M> point1) where M : 
            struct, 
            IComparable, 
            IComparable<M>, 
            IConvertible, 
            IEquatable<M>, 
            IFormattable
        {
            Vec2<double> p0 = point0.ConvertMemberType<double>();
            Vec2<double> p1 = point1.ConvertMemberType<double>();

            return distance(p0, p1);
        }

        private static double distance(Vec2<double> point0, Vec2<double> point1)
        {
            double xDifference = point1.X - point0.X;
            double yDifference = point1.Y - point0.Y;

            xDifference = Math.Abs(xDifference);
            yDifference = Math.Abs(yDifference);

            double sum = Math.Pow(xDifference, 2) + Math.Pow(yDifference, 2);

            double result = Math.Sqrt(sum);

            return result;
        }

    }
    
    
}