using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model.DataBase
{
    public partial class CallsModel
    {
        public int ID { get; set; }
        public int IDAGV { get; set; }
        public int IDRoute { get; set; }
        public string initTime { get; set; }
        public string endTime { get; set; }
        private string _CarriedItem { get; set; }
        public string CarriedItem

        {
            get
            {
                if (string.IsNullOrEmpty(_CarriedItem))
                {
                    return _CarriedItem;
                }
                return _CarriedItem.Trim().ToUpper();
            }
            set
            {
                _CarriedItem = value;
            }
        }

        public int CUCode { get; set; }
    }
}
