namespace PersonalAccountBookUWP
{
    class DetailHistory
    {
        private int id;
        private string historyId;
        private string name;
        private int price;

        public int Id => id;
        public string HistoryId => historyId;
        public string Name => name;
        public int Price => price;

        public DetailHistory(int id, string historyId, string name, int price)
        {
            this.id = id;
            this.historyId = historyId;
            this.name = name;
            this.price = price;
        }
    }
}
