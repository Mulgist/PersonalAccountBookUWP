namespace PersonalAccountBookUWP
{
    class Account
    {
        private int id;
        private string bank;
        private string name;
        private string number;
        private int balance;

        public int Id => id;
        public string Bank => bank;
        public string Name => name;
        public string Number => number;
        public int Balance => balance;

        public Account(int id, string bank, string name, string number, int balance)
        {
            this.id = id;
            this.bank = bank;
            this.name = name;
            this.number = number;
            this.balance = balance;
        }
    }
}
