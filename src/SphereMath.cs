using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaCovMapGen
{
    public class SphereMath
    {
        const double RadiansToDegreesConversionFactor = 57.2957795129;
        const double DE2RA = 0.01745329252;
        const double RA2DE = RadiansToDegreesConversionFactor;
        const double AVG_ERAD = 6371.0;

        /// <summary>
        /// 1 Km = 0.62 miles
        /// </summary>
        const double KilometersToMiles = 0.62137119;

        //public static double Azimuth(ISphereCoordinate c1, ISphereCoordinate c2)
        //{
        //    var longitudinalDifference = c1.Longitude - c2.Longitude;
        //    var latitudinalDifference = c1.Latitude - c2.Latitude;
        //    var azimuth = (Math.PI * .5d) - Math.Atan(latitudinalDifference / longitudinalDifference);
        //    if (longitudinalDifference > 0) return azimuth;
        //    else if (longitudinalDifference < 0) return azimuth + Math.PI;
        //    else if (latitudinalDifference < 0) return Math.PI;
        //    return 0d;
        //}

        public double AzimuthDegrees(ISphereCoordinate c1, ISphereCoordinate c2)
        {
            return RadiansToDegreesConversionFactor * Azimuth(c1, c2);
        }

        /// <summary>
        /// Calculates the distance between the 2 earth points (lat/longs) in Kilometers.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.codeguru.com/cpp/cpp/algorithms/article.php/c5115/Geographic-Distance-and-Azimuth-Calculations.htm"/>
        public static double DistanceKM(ISphereCoordinate c1, ISphereCoordinate c2)
        {
            double lat1 = c1.Latitude * DE2RA;
            double lon1 = c1.Longitude * DE2RA;
            double lat2 = c2.Latitude * DE2RA;
            double lon2 = c1.Longitude * DE2RA;
            double result = 0;

            if(!c1.Equals(c2))
            {
                double d = Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2);
                if(double.IsNaN(d))
                {
                    Console.WriteLine("Nan!");
                }
                else
                {
                    d = Math.Round(d, 5);
                }

                double acd = Math.Acos(d);
                if (double.IsNaN(acd))
                {
                    Console.WriteLine("Math.Acos(d); == Nan!");
                }

                result = AVG_ERAD * acd;
                if (double.IsNaN(result))
                {
                    Console.WriteLine("AVG_ERAD * acd == Nan!");
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the distance between the 2 earth points (lat/longs) in Miles.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.codeguru.com/cpp/cpp/algorithms/article.php/c5115/Geographic-Distance-and-Azimuth-Calculations.htm"/>
        public static double DistanceMiles(ISphereCoordinate c1, ISphereCoordinate c2)
        {
            var distance = KilometersToMiles * DistanceKM(c1, c2);

            return distance;
        }

        /// <summary>
        /// The azimuth is how many degrees clockwise from North you have to rotate in order to face Point B when standing at Point A. 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.codeguru.com/cpp/cpp/algorithms/article.php/c5115/Geographic-Distance-and-Azimuth-Calculations.htm"/>
        /// <seealso cref="http://cosinekitty.com/compass.html"/>
        public static double Azimuth(ISphereCoordinate c1, ISphereCoordinate c2)
        {
            double result = 0.0;
            double lat1 = c1.Latitude * DE2RA;
            double lon1 = c1.Longitude * DE2RA;
            double lat2 = c2.Latitude * DE2RA;
            double lon2 = c2.Longitude * DE2RA;

            if(c1.Latitude == c2.Latitude && c1.Longitude == c2.Longitude)
            {
                // No Op - Same points
            }
            else if(c1.Longitude == c2.Longitude)
            {
                if(c1.Latitude > c2.Latitude)
                {
                    result = 180.0;
                }
            }
            else
            {
                //var A = Math.Asin( Math.Sin (90 - c2.Latitude) * Math.Sin(c2.Longitude - c1.Longitude) / Math.Sin(DistanceRaw(c1, c2);

                double c = Math.Acos(Math.Sin(lat2)*Math.Sin(lat1) +
                                 Math.Cos(lat2)*Math.Cos(lat1)*Math.Cos((lon2-lon1)));
                double A = Math.Asin(Math.Cos(lat2)*Math.Sin((lon2-lon1))/Math.Sin(c));
                result = (A * RA2DE);
 
                if ((lat2 > lat1) && (lon2 > lon1))
                {
                }
                else if ((lat2 < lat1) && (lon2 < lon1))
                {
                    result = 180.0 - result;
                }
                else if ((lat2 < lat1) && (lon2 > lon1))
                {
                    result = 180.0 - result;
                }
                else if ((lat2 > lat1) && (lon2 < lon1))
                {
                    result += 360.0;
                }

            }

            // Return
            return result;
        }

        public static double AddDegrees(double azi, double d)
        {
            var n = azi + d;
            if(n < 0)
            {
                n = 360 + n;
            }
            else if(n > 360)
            {
                n = n - 360;
            }

            return n;
        }

        //public static bool IsDegreeGT(double azi, double d)
        //{

        //}
    }

    public interface ISphereCoordinate : IEquatable<ISphereCoordinate>
    {
        float Latitude { get; }
        float Longitude  { get; }
    }

    public class SphereCoordinate : ISphereCoordinate
    {
        public static SphereCoordinate CenterOfUsa = new SphereCoordinate(39.50f, -98.35f);
        public static SphereCoordinate NorthPole = new SphereCoordinate(90, 0);

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public SphereCoordinate(float lat, float lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }

        public bool Equals(ISphereCoordinate other)
        {
            return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
        }
    }
}
