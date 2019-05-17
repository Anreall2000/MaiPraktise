using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSql
{
    class DeliveryData : IAdd, ITex
    {
        public string SupId { get => supplierId; set => supplierId = value; }
        public string DetId { get => dataId; set => dataId = value; }
        public string Count { get => count; set => count = value; }
        public string Data { get => date; set => date = value; }

        public DeliveryData() { }
        public DeliveryData(params string[] TableData)
        {
            SupId = TableData[0];
            DetId = TableData[1];
            Count = TableData[2];
            Data = TableData[3];
        }
        public string toTabular()
        {
            var temp = new StringBuilder();
            temp.AppendFormat("{0};{1};{2};{3}", SupId, DetId, Count, Data);
            return temp.ToString();
        }

        public IAdd Create(params string[] TableData)
        {
            return new DeliveryData(TableData);
        }

        private string date;
        private string count;
        private string supplierId;
        private string dataId;
    }
}
