using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Test
{
    // Extension class
    public static class RollABallExtensions
    {
        private static readonly char[] Separators = new [] {' ', '.', ',', '!', '?', ':', ';', '\n', '\t'};

        /// <summary>
        /// Splits a string at known separators.
        /// </summary>
        /// <param name="someString">String to split.</param>
        /// <returns>Array of strings after splitting.</returns>
        public static IEnumerable<string> Separate(this string someString)
        {
            return someString.Split(Separators, StringSplitOptions.RemoveEmptyEntries); 
        }

        /// <summary>
        /// Returns number of characters in a string.
        /// </summary>
        /// <param name="word">Source string.</param>
        /// <param name="withSeparators">When set 'True', separators are treated as characters.</param>
        /// <returns>Number of characters in string.</returns>
        public static int GetCharacterCount(this string word, bool withSeparators)
        {
            return withSeparators ? word.Count() : word.Separate().Select(x => x.Length).Sum();
        }
        
        /// <summary>
        /// Returns number of occurrences of an element in collection.
        /// </summary>
        /// <param name="collection">Source collection.</param>
        /// <typeparam name="T">Type of element in collection.</typeparam>
        /// <returns>Sorted collection of tuples, where first value is element itself,
        /// and second is number of occurrences in collection.</returns>
        public static IEnumerable<Tuple<T,int>> GetElementsAndCount<T>(this IEnumerable<T> collection)
        {
            return 
                from item in collection
                group item by item
                into itemGroup
                orderby itemGroup.Count() descending, itemGroup.Key 
                select Tuple.Create(itemGroup.Key, itemGroup.Count());

            // return collection
            //     .GroupBy(item => item)
            //     .Select(group => Tuple.Create(group.Key, group.Count()))
            //     .OrderByDescending(t => t.Item2)
            //     .ThenBy(x => x);
        }

        // public static Dictionary<string, int> OrderBy01(this Dictionary<string, int> disc)
        // {
        //     
        // }
    }

    public class HomeworkLesson7Script : MonoBehaviour
    {
        #region Fields
        private string TestString => "Если тебе не нравится то, что ты получаешь, измени то, что ты даешь. Карлос Кастанеда";
        private delegate Dictionary<string,int> DictionarySorted(Dictionary<string,int> dictionary); 
        
        #endregion
        
        #region MonoBehaviour metods
              private void Start()
        {
            Debug.Log(TestString);
            
            Debug.Log("--- Number of characters per string ---");
            Debug.Log("--- Words ---");
            Debug.Log($"Character count with separators: {TestString.GetCharacterCount(true)}");
            Debug.Log($"Character count without separators: {TestString.GetCharacterCount(false)}");
            
            Debug.Log("--- Number elements in collection ---");
            Debug.Log("--- Strings ---");
            var words = TestString.Separate();
            var wordCounts = words.GetElementsAndCount();
            var wordCountsPrint = PrintCollection(wordCounts);
            Debug.Log(wordCountsPrint);

            Debug.Log("--- Numbers ---");
            var numbers = new[] { 12, 34, 56, 76, 12, -567, 0, 34, 12, 22, 11 };
            var numberCounts = numbers.GetElementsAndCount();
            var numberCountsPrint = PrintCollection(numberCounts);
            Debug.Log(numberCountsPrint);
            
            Debug.Log("--- Options for accessing OrderBy ---");
            var dict = new Dictionary<string, int>()
            {
                {"four", 4},
                { "one", 1},
                {"three", 3},
                {"two", 2}
            };
            
            // ---- 
            Debug.Log("* 1: Original version *");
            var d = dict.OrderBy(delegate(KeyValuePair<string,int> pair) { 
                return pair.Value; 
            });
            var dPrint = PrintCollection(d);
            Debug.Log(dPrint);
            
            // ---- 
            Debug.Log("* 2: Collapse call to OrderBy using lambda expression *");
            var d01 = dict.OrderBy(p => p.Value);
            var d01Print = PrintCollection(d01);
            Debug.Log(d01Print);
            
            // ---- 
            Debug.Log("* 3: Expand a call to OrderBy using a delegate *");
            
            DictionarySorted sorted = delegate(Dictionary<string, int> dictionary)
            {
                return dictionary
                    .OrderBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.Value);
            };

            var sortedPrint = PrintCollection(sorted(dict));
            Debug.Log(sortedPrint);
        }
        #endregion
        
        #region Functionality

        private string PrintCollection<T>(IEnumerable<T> collection)
        {
            return collection.Aggregate("\n", (current, item) => current + $"{item.ToString()}, ");
        }

        #endregion
    }
}