using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountBookUWP
{
    class Account
    {
        private int id;
        private string bank;
        private string name;
        private string number;
        private int balance;

        public int Id
        {
            get
            {
                return id;
            }
        }
        public string Bank
        {
            get
            {
                return bank;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string Number
        {
            get
            {
                return number;
            }
        }
        public int Balance
        {
            get
            {
                return balance;
            }
        }

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
