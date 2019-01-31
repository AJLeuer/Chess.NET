using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using C5;
using SFML.System;

namespace Chess.Util
{
    public static class Util
    {
        public static Vector2f ConvertVector<N>(this Vec2<N> vector) where N : 
            struct, 
            IComparable, 
            IComparable<N>, 
            IConvertible, 
            IEquatable<N>, 
            IFormattable
        {
            return new Vector2f((float)vector[0].ToDouble(NumberFormatInfo.InvariantInfo), (float)vector[1].ToDouble(NumberFormatInfo.InvariantInfo));
        }
        
        public static object AsSingleParam(this object[] arg)
        {
            object returnValue = arg;
            return returnValue;
        }
        
                
        public static ArrayList< ArrayList<T> > CreateFrom2DArray<T>(T[][] arrays)
        {
            var arrayLists = new ArrayList< ArrayList<T> >();
            
            foreach (T[] array in arrays)
            {
                var arrayList = new ArrayList<T>();
                arrayList.AddAll(array);
                arrayLists.Add(arrayList);
            }

            return arrayLists;
        }
        
        public delegate object ConstructorDelegate(params object[] args);
        
        /* Code credit stackoverflow user J. van Langen: https://stackoverflow.com/questions/840261/passing-arguments-to-c-sharp-generic-new-of-templated-type*/
        public static ConstructorDelegate CreateConstructor(Type type, params Type[] parameters)
        {
            // Get the constructor info for these parameters
            var constructorInfo = type.GetConstructor(parameters);

            // define a object[] parameter
            var paramExpr = Expression.Parameter(typeof(Object[]));

            // To feed the constructor with the right parameters, we need to generate an array 
            // of parameters that will be read from the initialize object array argument.
            var constructorParameters = parameters.Select((paramType, index) =>
                                                              // convert the object[index] to the right constructor parameter type.
                                                              Expression.Convert(
                                                                  // read a value from the object[index]
                                                                  Expression.ArrayAccess(
                                                                      paramExpr,
                                                                      Expression.Constant(index)),
                                                                  paramType)).ToArray();

            // just call the constructor.
            if (constructorInfo != null)
            {
                var body = Expression.New(constructorInfo, constructorParameters);

                var constructor = Expression.Lambda<ConstructorDelegate>(body, paramExpr);
                return constructor.Compile();
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
    
    public static class Extensions
    {
        public static ICloneable[] DeepClone(this ICloneable[] array)
        {
            ConstructorInfo constructor = array.GetType().GetConstructors()[0];
            ICloneable[] cloneArray = (ICloneable[]) constructor.Invoke(new object[] {array.Length});

            for (uint i = 0; i < array.Length; i++)
            {
                var clone = (ICloneable) array[i].Clone();
                cloneArray[i] = clone;
            }

            return cloneArray;
        }
        
        public static ICloneable[][] DeepClone(this ICloneable[][] arrays)
        {
            ConstructorInfo constructor = arrays.GetType().GetConstructors()[0];
            ICloneable[][] cloneArrays = (ICloneable[][]) constructor.Invoke(new object[] {arrays.Length}); 

            for (uint i = 0; i < arrays.Length; i++)
            {
                ICloneable[] cloneArray = arrays[i].DeepClone();
                cloneArrays[i] = cloneArray;
            }

            return cloneArrays;
        }
        
        public static ICloneable[,] DeepClone(this ICloneable[,] array)
        {
            ConstructorInfo constructor = array.GetType().GetConstructors()[0];
            ICloneable[,] cloneArray = (ICloneable[,]) constructor.Invoke(new object[] {array.GetLength(0), array.GetLength(1)}); 

            for (uint i = 0; i < array.GetLength(0); i++)
            {
                for (uint j = 0; j < array.GetLength(1); j++)
                {
                    var original = (ICloneable) array[i, j];
                    var clone = (ICloneable) original.Clone();
                    cloneArray[i,j] = clone;
                }
            }

            return cloneArray;
        }

        public static T SelectElementAtRandom<T>(this System.Collections.Generic.IList<T> container) where T : class
        {
            if (container.Count == 0)
            {
                return null;
            }
            
            var randomizer = new Random();

            var randomIndex = randomizer.Next(0, container.Count - 1);

            T randomElement = container.ElementAt(randomIndex);

            return randomElement;
        }
        
        public static List<T> ExtractHighestValueSubset<T>(this List<T> list) where T : IComparable<T>
        {
            if (list.Count == 0)
            {
                return list;
            }
            
            var highestValueSubset = new List<T>();
            
            list.Sort();
            list.Reverse();

            T best = list[0];
            
            foreach (var item in list)
            {
                if (item.CompareTo(best) == 0)
                {
                    highestValueSubset.Add(item);
                }
                else
                {
                    break;
                }
            }

            return highestValueSubset;
        }
        
        public static T RetrieveHighestValueItem<T>(this List<T> list) where T : class, IComparable<T>
        {
            List<T> highestValuedItems = ExtractHighestValueSubset(list);
            return SelectElementAtRandom(highestValuedItems);
        }

        public static Size GetActualSize(this SFML.Graphics.Sprite sprite)
        {
            uint width = (uint)(sprite.Texture.Size.X * sprite.Scale.X);
            uint height = (uint)(sprite.Texture.Size.Y * sprite.Scale.Y);
            
            return new Size(width, height);
        }
        
        /// <summary>
        /// Calculates and sets the scaling value that will cause the the Sprite to be rendered at the size given by <paramref name="targetResolution"/>
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="targetResolution"></param>
        public static void SetResolution(this SFML.Graphics.Sprite sprite, Size targetResolution)
        {
            double scalingValueX  = (float)targetResolution.Width / (float)sprite.Texture.Size.X;
            double scalingValueY = (float)targetResolution.Height / (float)sprite.Texture.Size.Y;

            sprite.Scale = new Vec2<double>(scalingValueX, scalingValueY);
        }
        
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action.Invoke(element);
            }
        }
        
    }
    
    public delegate void CallBack();
}