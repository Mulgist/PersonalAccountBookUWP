using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountBookUWP
{
    class HistoryListCell
    {
        private string historyId;
        private string accountName;
        private string typeName;
        private string book;
        private string balance;
        private string amount;
        private string date;

        public string HistoryId => historyId;
        public string AccountName => accountName;
        public string TypeName => typeName;
        public string Book => book;
        public string Balance => balance;
        public string Amount => amount;
        public string Date => date;

        public HistoryListCell(string historyId, string accountName, string typeName, string book, string balance, string amount, string date)
        {
            this.historyId = historyId;
            this.accountName = accountName;
            this.typeName = typeName;
            this.book = book;
            this.balance = balance;
            this.amount = amount;
            this.date = date;
        }
    }
}
