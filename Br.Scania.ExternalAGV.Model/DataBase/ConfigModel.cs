using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model.DataBase
{
    public partial class ConfigModel
    {
        public int ID { get; set; }
        private string _Prefix { get; set; }
        public string Prefix

        {
            get
            {
                if (string.IsNullOrEmpty(_Prefix))
                {
                    return _Prefix;
                }
                return _Prefix.Trim().ToUpper();
            }
            set
            {
                _Prefix = value;
            }
        }

        public int IDAGV { get; set; }
        public bool Start { get; set; }
    }
}
