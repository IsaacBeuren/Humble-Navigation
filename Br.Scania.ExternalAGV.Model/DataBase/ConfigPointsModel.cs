using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Br.Scania.ExternalAGV.Model.DataBase
{
    public partial class ConfigPointsModel
    {
        public int ID { get; set; }
        private string _Description { get; set; }
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_Description))
                {
                    return _Description;
                }
                return _Description.ToUpper().Trim();
            }
            set
            {
                _Description = value;
            }
        }

        public double Lat { get; set; }
        public double Lng { get; set; }
        public int Velocity { get; set; }
        public bool LeftLight { get; set; }
        public bool RightLight { get; set; }
        private string _icon { get; set; }
        public string icon
        {
            get
            {
                if (string.IsNullOrEmpty(_icon))
                {
                    return _icon;
                }
                return _icon.ToLower().Trim();
            }
            set
            {
                _icon = value;
            }
        }

        public bool OnStraight { get; set; }

    }
}
