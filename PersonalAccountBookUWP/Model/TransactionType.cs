namespace PersonalAccountBookUWP
{
    class TransactionType
    {
        private int id;
        private string name;
        private bool isEarn;

        public int Id => id;
        public string Name => name;
        public bool IsEarn => isEarn;

        public TransactionType(int id, string name, int isEarn)
        {
            this.id = id;
            this.name = name;
            if (isEarn == 1)
            {
                this.isEarn = true;
            }
            else
            {
                this.isEarn = false;
            }
        }
    }
}
