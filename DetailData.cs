using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSql
{
    class DetailData: IAdd, ITex
    {
        public string Id { get => supplierId; set => supplierId = value; }
        public string Name { get => name; set => name = value; }
        public string Article { get => article; set => article = value; }
        public string Price { get => price; set => price = value; }

        public DetailData() { }
        public DetailData(params string[] TableData)
        {
            Id = TableData[0];
            Name = TableData[1];
            Article = TableData[2];
            Price = TableData[3];
        }
        public string toTabular()
        {
            var temp = new StringBuilder();
            temp.AppendFormat("{0};{1};{2};{3}", Id, Name, Article, Price);
            return temp.ToString();
        }

        public IAdd Create(params string[] TableData)
        {
            return new DetailData(TableData);
        }

        private string price;
        private string article;
        private string supplierId;
        private string name;
    }
}

