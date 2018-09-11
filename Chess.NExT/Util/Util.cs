using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using C5;
using SFML.System;

namespace Chess.Util
{
    public static class Util
    {
        public const ushort lowerCaseA = 97;
        
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
    }
    
    public static class Extensions
    {
        public static ICloneable[] DeepClone(this ICloneable[] array)
        {
            ConstructorInfo constructor = array.GetType().GetConstructors()[0];
            ICloneable[] cloneArray = (ICloneable[]) constructor.Invoke(new object[]{array.Length});

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

        public static T selectElementAtRandom<T>(this System.Collections.Generic.IList<T> container)
        {
            var randomizer = new Random();

            var randomIndex = randomizer.Next(0, container.Count - 1);

            T randomElement = container.ElementAt(randomIndex);

            return randomElement;
        }
        
        public static List<T> extractHighestValueSubset<T>(this List<T> list) where T : IComparable<T>
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
        
    }
    
    public delegate void CallBack();
}