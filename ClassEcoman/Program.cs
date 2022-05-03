using Newtonsoft.Json;
Ecoman.Init();
public static class Ecoman
{
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
            Console.WriteLine("Cards Read");
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to read the cards");
            for (int i = 1; i < 40; i++)
            {
                Console.WriteLine(i);
                Cards.Add(i, new PointCard(i));
            }
            WriteToJsonFile(CardPath, Cards);
        }
        Process();
    }
    public static void Process()
    {
        while (true)
        {
            try
            {
                string? what = Console.ReadLine();
                if(what.StartsWith("J"))
                {
                    var that = what.Split('-');
                    if(that != null)
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

                        }
                        catch (Exception)
                        {

                            
                        }
                        Console.WriteLine($"LINE POSITION = {Position}");
                        Console.WriteLine($"DAY PAY = {DayPay}");
                        Console.WriteLine($"TASK PAY = {TaskPay}");
                        Console.Write("Waiting for Point Card Scan : ");
                        var PointCard = Console.ReadLine();
                        if(PointCard != null)
                        {
                            PointCard = PointCard.Replace("ID-", "");
                            int.TryParse(PointCard, out int result);
                            if(Cards.TryGetValue(result,out PointCard Pnt))
                            {
                                if(!Pnt.ScannedOn)
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
                            }
                            else
                            {
                                Console.WriteLine("PointCard unreadable");

                            }


                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
[Serializable]
public class PointCard
{
    
    public int ID = 0;
    public double Points = 0;
    public bool ScannedOn = false;
    public PointCard(int ID)
    {
        ID = ID;
    }
}

