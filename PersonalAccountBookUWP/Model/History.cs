using System;

namespace PersonalAccountBookUWP
{
    class History
    {
        private string id;
        private DateTime date;
        private int accountId;
        private int typeId;
        private string bankbook;
        private string cardbook = "";
        private int amount;

        public string Id => id;
        public DateTime Date => date;
        public int AccountId => accountId;
        public int TypeId => typeId;
        public string Bankbook => bankbook;
        public string Cardbook => cardbook;
        public int Amount => amount;
        
        public History(string id, DateTime date, int account, int type, string bankbook, string cardbook, int amount)
        {
            this.id = id;
            this.date = date;
            this.bankbook = bankbook;
            this.cardbook = cardbook;
            this.amount = amount;
            accountId = account;
            typeId = type;
        }
    }
}
