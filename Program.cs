class ATM
{
    private static List<(string username, int pin, decimal balance, List<string> transactions)> bankAccounts;
    private static string dbPath = "bank.txt";
    private static (string username, int pin, decimal balance, List<string> transactions) currentUser;
    private static bool isLoggedIn = false;

    static void Main(string[] args)
    {
        Console.WriteLine("welcome to the ATM");

        loadBankCustomers();
        if (validateUser())
        {
            isLoggedIn = true;
            Console.WriteLine($"\nWelcome back, {currentUser.username}!");

            while (isLoggedIn)
            {
                ShowMenu();
            }
        }
        else
        {
            Console.WriteLine("login failed");
        }
    }

    static void loadBankCustomers()
    {
        bankAccounts = new List<(string, int, decimal, List<string>)>();

        try
        {
            string[] lines = File.ReadAllLines(dbPath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string username = parts[0];
                int pin = int.Parse(parts[1]);
                decimal balance = decimal.Parse(parts[2]);

                List<string> transactions = new List<string>();
                if (parts.Length > 3)
                {
                    string[] trans = parts[3].Split(':');
                    transactions.AddRange(trans);
                }

                bankAccounts.Add((username, pin, balance, transactions));
            }

            Console.WriteLine("loaded success");
        }
        catch (Exception e)
        {
            Console.WriteLine($"errer {e.Message}");
        }
    }

    static bool validateUser()
    {
        int attempts = 0;

        while (attempts < 3)
        {
            Console.Write("\nusrnmae ");
            string username = Console.ReadLine();

            Console.Write("pin ");
            string pinInput = Console.ReadLine();

            if (!int.TryParse(pinInput, out int pin))
            {
                Console.WriteLine("wronf formta");
                attempts++;
                continue;
            }

            // check if user exists
            foreach (var account in bankAccounts)
            {
                if (account.username == username && account.pin == pin)
                {
                    currentUser = account;
                    return true;
                }
            }

            attempts++;
            Console.WriteLine($"Invalid credentials. {3 - attempts} attempts remaining.");
        }

        return false;
    }

    static void ShowMenu()
    {
        Console.WriteLine("\noptions");
        Console.WriteLine("1. Check Balance");
        Console.WriteLine("2. Withdraw");
        Console.WriteLine("3. Deposit");
        Console.WriteLine("4. Display Last 5 Transactions");
        Console.WriteLine("5. Quick Withdraw $40");
        Console.WriteLine("6. Quick Withdraw $100");
        Console.WriteLine("7. End Session");
        Console.Write("Select an option: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                checkBalance();
                break;
            case "2":
                withdrawMoney();
                break;
            case "3":
                depositMoney();
                break;
            case "4":
                displayTransactions();
                break;
            case "5":
                quickWithdraw(40);
                break;
            case "6":
                quickWithdraw(100);
                break;
            case "7":
                saveBankCustomers();
                isLoggedIn = false;
                Console.WriteLine("done");
                break;
            default:
                Console.WriteLine("Invalid");
                break;
        }
    }

    static void checkBalance()
    {
        Console.WriteLine($"\nCurrent Balance: ${currentUser.balance:F2}");
    }

    static void withdrawMoney()
    {
        Console.Write("\nEnter amount");
        string input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal amount))
        {
            Console.WriteLine("Invalid");
            return;
        }

        if (amount > currentUser.balance)
        {
            Console.WriteLine("not wnough");
            return;
        }

        currentUser.balance -= amount;
        currentUser.transactions.Add($"withdraw ${amount:F2}");
        updateAccount();

        Console.WriteLine($"Withdrew ${amount:F2}. New balance ${currentUser.balance:F2}");
    }




    static void depositMoney()
    {
        Console.Write("\nEnter amount to deposit: $");
        string input = Console.ReadLine();



        if (!decimal.TryParse(input, out decimal amount))
        {
            Console.WriteLine("Invalid amount!");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Amount must be positive!");
            return;
        }

        currentUser.balance += amount;
        currentUser.transactions.Add($"Deposit ${amount:F2}");
        updateAccount();

        Console.WriteLine($"Deposited ${amount:F2}. New balance: ${currentUser.balance:F2}");
    }

    static void quickWithdraw(int amount)
    {



        if (amount > currentUser.balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }

        currentUser.balance -= amount;


        currentUser.transactions.Add($"Quick withdraw ${amount}");
        updateAccount();

        Console.WriteLine($"Withdrew ${amount}. new ${currentUser.balance:F2}");

        
    }

    static void updateAccount()
    {
        // Update the account in the list
        for (int i = 0; i < bankAccounts.Count; i++)
        {
            if (bankAccounts[i].username == currentUser.username)
            {
                bankAccounts[i] = currentUser;
                break;
            }
        }
    }

    static void saveBankCustomers()
    {
        try
        {
            List<string> lines = new List<string>();

            foreach (var account in bankAccounts)
            {


                string transactionString = string.Join(":", account.transactions);
                string line = $"{account.username},{account.pin},{account.balance},{transactionString}";
                lines.Add(line);
            }

            File.WriteAllLines(dbPath, lines);
            Console.WriteLine("saved");
        }
        catch (Exception e)
        {


            Console.WriteLine($"errer {e.Message}");
        }
    }
}
