using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using DreadZitoTypes;


namespace DreadZitoTools
{
    public static class Utils
    {
        /**
         * GetPercentage - Devuelve el porcentage del numero seleccionado
         * @num: numero
         * @percent: string del porcentage, max 100%
         * ---------------------
         * Return: porcentaje en float o null si fallo
        */
        public static dynamic GetPercentage(float num, string percent)
        {
            if (percent == "")
                return 0;

            if (percent[percent.Length - 1] != '%') {
                Debug.LogError("GetPercentage: Usage <num> <string number + '%'> ~~~~~~~> example: GetPorcentage(10,\"50%\") = 5");
                return null;
            }

            if (percent == "100%")
                return num;

            List<char> newStr = new List<char>();
            newStr.AddRange(percent);
            newStr.RemoveAt(newStr.Count - 1); // Quito el '%'

            string strNum = "0." + new string(newStr.ToArray());
            float perc = float.Parse(strNum, CultureInfo.InvariantCulture.NumberFormat);

            return num * perc;
        }

        public static Vector3 Vector3Sqrt(Vector3 origin)
        {
            if (origin.x < 0)
                origin.x = (Mathf.Sqrt(Mathf.Abs(origin.x))) * -1;
            else 
                origin.x = Mathf.Sqrt(origin.x);
            if (origin.y < 0)
                origin.y = (Mathf.Sqrt(Mathf.Abs(origin.y))) * -1;
            else 
                origin.y = Mathf.Sqrt(origin.y);
            if (origin.z < 0)
                origin.z = (Mathf.Sqrt(Mathf.Abs(origin.z))) * -1;
            else 
                origin.z = Mathf.Sqrt(origin.z);
            return origin;
        }

        /**
         * ClearAllCoroutinesFromList - Detiene todas las corrutinas de una lista
         * @list: la lista de corrutinas
         * @inst: la instancia de MonoBehaviour que tiene esas corroutinas
         * -----------------------------------
        */
        public static void ClearAllCoroutinesFromList(DreList<Coroutine> list, MonoBehaviour inst)
        {
            foreach (var item in list) {
                inst.StopCoroutine(item);        
            }
            list.Clear();
        }

        public static void PrintList<T>(List<T> list)
        {
            foreach (var elem in list) {
                Debug.Log(elem);
            }
        }

        public static void PrintListOfLists<T>(List<List<T>> list)
        {
            foreach (var firstSet in list) {
                Debug.Log(">Element==========================");
                foreach (var elem in firstSet) {
                    Debug.Log(elem);
                }
                Debug.Log("===================================");
            }
        }

        public static string GetLast(this string source, int numberOfChars)
        {
            if(numberOfChars >= source.Length)
                return source;
            return source.Substring(source.Length - numberOfChars);
        }
    }

    class MonobehaviourLabRat : MonoBehaviour {}
}
