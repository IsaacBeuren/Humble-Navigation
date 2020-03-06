using Br.Scania.ExternalAGV.Model;
using CoordinateSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Business
{
    public class FilterBusiness
    {
        static List<GGAModel> coordinates = new List<GGAModel>();

        public GGAModel ApplyFilter(GGAModel localCoordinates)
        {
            int LenghtArr = 3;

            double? MinLat = -9999999999;
            double? MaxLat = 0;
            double? MinLng = -9999999999;
            double? MaxLng = 0;
            double? DesvLat = 0;
            double? DesvLng = 0;

            if (coordinates.Count >= LenghtArr)
            {
                coordinates.RemoveAt(0);
            }

            if (localCoordinates != null)
            {
                coordinates.Add(localCoordinates);
            }

            double? Lat = 0;
            double? Lng = 0;

            foreach (var item in coordinates)
            {
                if (item.Latitude > MinLat)
                {
                    MinLat = item.Latitude;
                }
                if (item.Latitude < MaxLat)
                {
                    MaxLat = item.Latitude;
                }

                if (item.Longitude > MinLng)
                {
                    MinLng = item.Longitude;
                }
                if (item.Longitude < MaxLng)
                {
                    MaxLng = item.Longitude;
                }

                Lat = Lat + item.Latitude;
                Lng = Lng + item.Longitude;
            }

            DesvLat = MaxLat - MinLat;
            DesvLng = MaxLng - MinLng;
            GGAModel finalCoordinates = new GGAModel();

            if (coordinates.Count > 0)
            {
                finalCoordinates = coordinates[coordinates.Count - 1];
                finalCoordinates.Latitude = Lat / coordinates.Count;
                finalCoordinates.Longitude = Lng / coordinates.Count;
            }
            return finalCoordinates;
        }
    }
}
