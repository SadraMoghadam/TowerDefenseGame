using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GeneralTools.Randomize
{
    public class Data
    {
        public object resource;
        public float chance;
    }


    public static class RandomPick
    {
        internal static System.Random rand = new System.Random();

        public static T Pick<T>(List<Data> resources)
        {
            float selector = 0;
            return (T)GetRandomItemBySelector(resources, out selector).resource;
        }

        static Data GetRandomItemBySelector(List<Data> resouceList, out float selector)
        {
            
            //Get our random from 0 to 1
            double double_rnd = rand.NextDouble();
            float rnd = Convert.ToSingle(double_rnd);
            selector = rnd;

            //Initialise our cumulative percentage
            float cumulativeChance = 0;
            //iterate over our resources
            for (var i = 0; i < resouceList.Count; i++)
            {

                //Include current resource
                cumulativeChance += resouceList[i].chance;

                if (rnd < cumulativeChance)
                    return resouceList[i];
            }

            return null;
        }

        public static T Pick<T>(T firstItem, float firstChance, T secondItem)
        {
            if (firstChance != 0)
            {
                var res = new List<Data>();

                res.Add(new Data()
                {
                    resource = firstItem,
                    chance = firstChance
                });

                res.Add(new Data()
                {
                    resource = secondItem,
                    chance = 1 - firstChance
                });

                try
                {

                    return Pick<T>(res);
                }
                catch
                {
                    return (T)res[0].resource;
                }
            }
            else
            {
                return secondItem;
            }
        }

        // List Shuffle
        public static List<T> Shuffle<T>(this List<T> list)
        {
            System.Random rng = new System.Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }


    }
}
