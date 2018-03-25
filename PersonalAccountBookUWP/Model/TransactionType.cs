namespace PersonalAccountBookUWP
{
    class TransactionType
    {
        private int id;
        private string name;

        public int Id => id;
        public string Name => name;

        public TransactionType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
