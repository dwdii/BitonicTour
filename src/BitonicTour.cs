using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaCovMapGen
{
    /// <summary>
    /// C# Implementation of Bitonic Tour algorithm.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <seealso cref="http://www.researchgate.net/publication/228391119_The_Canadian_Airline_Problem_and_the_Bitonic_Tour_Is_This_Dynamic_Programming"/>
    class BitonicTour
    {
        List<List<double>> _distances = new List<List<double>>();
        Dictionary<int, double> _optimals = new Dictionary<int, double>();

        public void ComputeDistances(List<ISphereCoordinate> signal)
        {
            // Compute distances
            for (int i = 0; i < signal.Count; i++)
            {
                _distances.Add(new List<double>());
                for (int j = 0; j < signal.Count; j++)
                {
                    _distances[i].Add(SphereMath.DistanceKM(signal[i], signal[j]));
                }
            }
        }

        public List<int> Solution(int x)
        {
            List<int> result = new List<int>();

            result.Add(x - 1);

            while (x > 3)
            {
                int i = x - 1;
                while (i >= 3 && Optimal(x) != Optimal(i) + Penalty(i, x))
                {
                    i--;
                }

                Extend(result, i, x);
                x = i;
            }

            Complete(result);

            return result;
        }

        public double Penalty(int x, int y)
        {
            double result = _distances[x - 2][y - 1];
            for(int i = x; i < y; i++)
            {
                result += _distances[i - 1][i];
            }
            result -= _distances[x - 2][x - 1];

            return result;
        }

        private double CostBasic(int x)
        {
            double result = _distances[0][x - 1];
            for(int i = 1; i < x; i++)
            {
                var dist = _distances[i - 1][i];
                if(double.IsNaN(dist))
                {
                    Console.WriteLine("CostBasic: distance == NaN");
                }
                else
                {
                    result += dist;
                }
            }

            return result;
        }

        private double Optimal(int x)
        {
            double result;
            if (_optimals.ContainsKey(x))
            {
                result = _optimals[x];
            }
            else
            {
                result = CostBasic(x);
                for (int i = 3; i < x; i++)
                {
                    result = Min(result, Optimal(i) + Penalty(i, x));
                }

                // Add to cache
                _optimals.Add(x, result);
            }

            return result;
        }

        private double Min(double x, double y)
        {
            return x <= y ? x : y;
        }

        private void Extend(List<int> s, int x, int y)
        {
            if(y - 1 == s.Last())
            {
                s.Add(x - 2);
                while (s.First() > x - 1)
                {
                    s.Insert(0, s.First() - 1);
                }
            }
            else
            {
                s.Insert(0, x - 2);
                while(s.Last() > x-1)
                {
                    s.Add(s.Last() - 1);
                }
            }
        }



        private void Complete(List<int> s)
        {
            if(s.Last() != 0)
            {
                s.Add(0);
            }

            if(s.First() != 0)
            {
                s.Insert(0, 0);
            }
        }
    }
}
