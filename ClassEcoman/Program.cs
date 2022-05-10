using Newtonsoft.Json;
Ecoman.Init();
public static class Ecoman
{

    public const int Version = 1;
    public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
    {
        TextWriter writer = null;
        try
        {
            var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
            writer = new StreamWriter(filePath, append);
            writer.Write(contentsToWriteToFile);
        }
        finally
        {
            if (writer != null)
                writer.Close();
        }
    }

    public static Dictionary<int,PointCard> Cards = new Dictionary<int, PointCard>();
    public static string Path = "";
    public static string CardPath = "Cards.datastuff";
    public static T ReadFromJsonFile<T>(string filePath) where T : new()
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(filePath);
            var fileContents = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(fileContents);
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }
    }
    public static void Init()
    {
        Path = Directory.GetCurrentDirectory();
        CardPath = $"{Path}/{CardPath}";
        Console.WriteLine(CardPath);
        try
        {
            Cards = ReadFromJsonFile<Dictionary<int, PointCard>>(CardPath);
            if(Cards.TryGetValue(1,out PointCard results))
            {
                if(results.Version != Version)
                {
                    Console.WriteLine("Cards not in correct version!!");
                    new Exception("unable to read the cards");
                }
            }
            Console.WriteLine("Cards Read");
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to read the cards");
            for (int i = 1; i < 40; i++)
            {
                Console.WriteLine(i);
                Cards.Add(i, new PointCard(i));
                WriteToJsonFile(CardPath, Cards);
            }
        }
        Process();
    }
    public static void Process()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("___________________________");
                string? what = Console.ReadLine();
                JobProcess(what);
                AddPointsProcess(what);
                ResetAllCardsProcess(what);
                ReadCardProcess(what);
                StartDayProcess(what);
                LotteryEmulateProcess(what);
                WriteToJsonFile(CardPath, Cards); // save cards after each process!
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    public static void LotteryEmulateProcess(string what)
    {
        if (what == "Emulate")
        {
            Console.Write("Num of Tickets ");
            var val = Console.ReadLine();
            int.TryParse(val, out int number);
            LotteryProcess("LOTTERY", number);
            Console.WriteLine($"MW = {LotteryStats.MegaWinners} NM = {LotteryStats.NormalWinners} L = {LotteryStats.Losses}");
        }
    }
    public static class LotteryStats
    {
        public static int MegaWinners = 0;
        public static int NormalWinners = 0;
        public static int Losses = 0;
    }
    public static void LotteryProcess(string what, int mock = 0)
    {
        if(what == "LOTTERY")
        {
            var counter = 0;

            while (true)
            {
                Console.Write("Ticket Number ");
                string val = "0";
                if (mock == 0)
                {
                    val = Console.ReadLine();
                }
                else
                {
                   val = "mock";
                    counter++;
                    if(counter > mock)
                    {
                        break;
                    }
                }
                Console.Write(" is a ");
                if(val == "CANCEL")
                {
                    break;
                }
                if (Random.Shared.NextSingle() < 0.05f)
                {
                    Console.WriteLine("MEGA WINNER! CHOOSE A JACKPOT");
                    LotteryStats.MegaWinners += 1;
                }
                else if (Random.Shared.NextSingle() < 0.15f)
                {
                    Console.WriteLine("NORMAL WINNER! Choose a snack!");
                    LotteryStats.NormalWinners += 1;
                }
                else
                {
                    Console.WriteLine("Sorry! Not a winner");
                    LotteryStats.Losses += 1;
                }
            }
        }
    }
    public static void ReadCardProcess(string what)
    {
        if(what == "READOUT")
        {
            var pnt = GetCard();
            Console.WriteLine("===============");
            Console.WriteLine($"ID | {pnt.ID}");
            Console.WriteLine($"POINTS | {pnt.Points}");
            Console.WriteLine("===============");
        }
    }
    public static void ResetAllCardsProcess(string what)
    {
        if(what.Contains("RESETCARDS"))
        {
            Console.WriteLine("Confirm? Scan Again |");
            string val = Console.ReadLine();
            if(val == "RESETCARDS")
            {
                Console.WriteLine("RESETTING ALL CARDS");
                foreach (var item in Cards)
                {
                    item.Value.Points = 0;
                }
            }
        }
    }
    public static void StartDayProcess(string what)
    {
        if(what.Contains("STARTDAY"))
        {
            foreach (var item in Cards)
            {
                item.Value.ScannedOn = false;
            }
            Console.WriteLine("DAY STARTING");
        }
    }
    public static void AddPointsProcess(string what)
    {
        string val = "";
        bool active = false;
        int AddNum = 0;
        if (what == "ADD10")
        {
            active = true;
            AddNum = 10;
        }
        if (what == "ADD5")
        {
            active = true;
            AddNum = 5;

        }
        if (what == "ADD1")
        {
            active = true;
            AddNum = 1;
        }
        while (active)
        {
            Console.WriteLine($"ADD {AddNum} POINTS TO CARD");
            try
            {
                var pnt = GetCard();
                pnt.Points += AddNum;
                Console.WriteLine($"Added {AddNum} to ID {pnt.ID}");
                Console.WriteLine($"| ID {pnt.ID} | now has {pnt.Points}");
            }
            catch (Exception)
            {
                active = false;
                break;
            }
        }
    }
    public static PointCard GetCard()
    {
        Console.Write("Scan ID Card | ");
        var PointCard = Console.ReadLine();
        if (PointCard != null)
        {
            PointCard = PointCard.Replace("ID-", "");
            int.TryParse(PointCard, out int result);
            double pointsPrev = 0;
            if (Cards.TryGetValue(result, out PointCard Pnt))
            {
                return Pnt;
            }
        }
        throw new Exception("ID unreadable!");
        return null;
    }
    public static int RequireInput()
    {
        string val = "";
        while (true)
        {
            string input = Console.ReadLine();
            if(input == "ENTER")
            {
                Console.WriteLine($"FINAL VALUE = {val} ");
                break;
            }
            val += input;
            Console.WriteLine($">>> {val} <<<");
        }
        if(int.TryParse(val, out int result))
        {
            return result;
        }
        else
        {
            Console.WriteLine("ERROR! invalid number");
            return 0;
        }
    }
    public static void JobProcess(string what)
    {
        if (what.StartsWith("J"))
        {
            var that = what.Split('-');
            if (that != null)
            {
                string Position = "";

                try
                {
                    Position = (string)that.GetValue(0);
                }
                catch (Exception)
                {


                }
                string DayPay = "";
                try
                {
                    DayPay = (string)that.GetValue(1);
                }
                catch (Exception)
                {

                }

                string TaskPay = "";
                try
                {
                    TaskPay = (string)that.GetValue(2);
                    if (TaskPay.Contains("%"))
                    {
                        TaskPay = "0";
                    }
                }
                catch (Exception)
                {


                }
                Console.WriteLine($"LINE POSITION = {Position}");
                Console.WriteLine($"DAY PAY = {DayPay}");
                Console.WriteLine($"TASK PAY = {TaskPay}");
                Console.Write("Waiting for Point Card Scan : ");
                var PointCard = Console.ReadLine();
                if (PointCard != null)
                {
                    PointCard = PointCard.Replace("ID-", "");
                    int.TryParse(PointCard, out int result);
                    double pointsPrev = 0;
                    if (Cards.TryGetValue(result, out PointCard Pnt))
                    {
                        pointsPrev = Pnt.Points;
                        if (!Pnt.ScannedOn)
                        {
                            Pnt.ScannedOn = true;
                            var val = DayPay.Replace("D", "");
                            Pnt.Points += int.Parse(val);
                            Console.WriteLine($"Day Pay | {val}");
                        }
                        else
                        {
                            var val = TaskPay.Replace("T", "");
                            Pnt.Points += int.Parse(val);
                            Console.WriteLine($"Task Pay | {val}");
                        }
                        Console.WriteLine($"Previously had {pointsPrev}");
                        Console.WriteLine($"Card now has {Pnt.Points}");
                    }
                    else
                    {
                        Console.WriteLine("PointCard unreadable");

                    }


                }
            }
        }
    }
}
[Serializable]
public class PointCard
{
    
    public int ID = 0;
    public int Version = 0;
    public double Points = 0;
    public bool ScannedOn = false;
    public PointCard(int ID)
    {
        this.ID = ID;
        this.Version = Ecoman.Version;
    }
}

