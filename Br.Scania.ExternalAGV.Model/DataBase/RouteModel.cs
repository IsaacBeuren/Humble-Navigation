using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Br.Scania.ExternalAGV.Model.DataBase
{
    public partial class RouteModel
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
        public string Routes { get; set; }
        public string PickUpPoint { get; set; }
        public string DropPoint { get; set; }
    }
}
