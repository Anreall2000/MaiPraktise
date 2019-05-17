using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSql
{
    class SupplierData : IAdd, ITex
    {

        public string Id { get => supplierId; set => supplierId = value; }
        public string Name { get => name; set => name = value; }
        public string Domain { get => domain; set => domain = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }

        public SupplierData(){}
        public SupplierData(params string[] TableData)
        {
            Id = TableData[0];
            Name = TableData[1];
            Domain = TableData[2];
            PhoneNumber = TableData[3];
        }
        public string toTabular()
        {
            var temp = new StringBuilder();
            temp.AppendFormat("{0};{1};{2};{3}", Id, Name, Domain, PhoneNumber);
            return temp.ToString();
        }

        public IAdd Create(params string[] TableData)
        {
            return new SupplierData(TableData);
        }

        private string phoneNumber;
        private string domain;
        private string supplierId;
        private string name;
    }
}
