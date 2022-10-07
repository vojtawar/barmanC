﻿using System.Diagnostics;
using System.Reflection.Metadata;

bool debug = true;
double[] teams = new double[]{0,0,0,0};

double score = 0;
string[] ingredients = new string[]
{
    "ananas", "tatarka", "kofola", "rum", "horka_voda", "most", "kysele_zeli", "led", 
};

IDictionary<string, int> translator = new Dictionary<string, int>();

int count = 0;
foreach (var thing in ingredients)
{
    translator.Add(thing, count);
    count++;
}


Combos comba = new Combos();


IDictionary<string, int> annanasovy_drink = new Dictionary<string, int>();
annanasovy_drink.Add("ananas", 10);
annanasovy_drink.Add("tatarka", 20);
annanasovy_drink.Add("kofola", 70);

comba.stuff.Add("annanasovy_drink", new Combo(300, annanasovy_drink, 20000));

IDictionary<string, int> tepla_kofola = new Dictionary<string, int>();

tepla_kofola.Add("kofola", 100);
tepla_kofola.Add("rum", 10);

comba.stuff.Add("tepla_kofola", new Combo(300, tepla_kofola, 20000));

IDictionary<string, int> apfel_kraut = new Dictionary<string, int>();
apfel_kraut.Add("kysele_zeli", 10);
apfel_kraut.Add("most", 30);
apfel_kraut.Add("horka_voda", 70);
apfel_kraut.Add("led", 30);

comba.stuff.Add("apfel_kraut", new Combo(300, apfel_kraut, 20000));

IDictionary<string, int> svarena_kofola = new Dictionary<string, int>();
svarena_kofola.Add("kofola", 140);
svarena_kofola.Add("horka_voda", 60);

comba.stuff.Add("svarena_kofola", new Combo(300, svarena_kofola, 20000));



Console.WriteLine();

while (true)
{ 
    Console.WriteLine("input team");
    int team_number = 0;
    bool correct_input = true;
    try
    {
      team_number = Int32.Parse(Console.ReadLine());
    }
    catch
    {
        correct_input = false;
        Console.WriteLine("Skip");
    }

    if (correct_input)
    {
        foreach (var thing in ingredients)
        {
            Console.Write(thing + " ");
        }

        Console.WriteLine();
        {
            int[] data = new int[] { };
            try
            {
                data = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            }
            catch
            {
                Console.WriteLine("Demente zadal jsi to spatne");
            }

            if (data.Length == 8)
            {
                IDictionary<string, int> drink = new Dictionary<string, int>();
                count = 0;
                foreach (var thing in data)
                {
                    drink.Add(ingredients[count], thing);
                    count++;
                }

                double ansver = comba.run(drink);
                double combo_value = comba.values.Max(x => x.Value);
                foreach (var VARIABLE in comba.values)
                {
                    if (VARIABLE.Value == combo_value)
                    {
                        Console.WriteLine("Combo: " + VARIABLE.Key + " " + combo_value);
                    }
                }
                
                teams[team_number] += ansver;
                Console.WriteLine(ansver);
                Console.WriteLine($"Tvůj team má score {teams[team_number]}");
                if (debug)
                {
                    foreach (var value in comba.values)
                     {
                        Console.WriteLine(value.Key + " " + value.Value);
                    }
                }
            }

        }
    }
}

record Combos
{
    public IDictionary<string, Combo> stuff = new Dictionary<string, Combo>();
    public IDictionary<string, double> values = new Dictionary<string, double>();

    double IceKooficient(IDictionary<string, int> drink)
    {
        double iceK = Math.Pow( (double) (drink["led"] * 3 - drink["horka_voda"]) /  (double) drink.Sum(x => x.Value) + 1, 3);
        if (true)
        {
            Console.WriteLine("IceKooficient: " +  iceK);   
        }
        return iceK ;
    }

    public double run(IDictionary<string, int> drink )
    {
        double ansver = 0;
        values = new Dictionary<string, double>();
        foreach (var thing in stuff)
        {
           values.Add(thing.Key, thing.Value.Grade(drink));
        }

        ansver = values.Sum(x => x.Value);
        
        return ansver * IceKooficient(drink);
    }
}

record Combo(double maxPoints, IDictionary<string, int> order, double maxToGive)
{
    private IDictionary<string, int> order = order;
    private double bonus = 30;
    private double maxPoints = maxPoints;
    private double maxToGive = maxToGive;
    private double givenPoints = 0;
    private string hlaska;

    float findFulfiment( IDictionary<string, int> drink)
    {
        float drinkSuma = drink.Sum(x => x.Value);
        float orderSuma = order.Sum(x => x.Value);
        float pomer = drinkSuma / orderSuma;
        float fulfilment = (float) order.Min(x => (float) drink[x.Key] / (float) x.Value) / pomer;

        
        return fulfilment;
    }
    public double Grade(IDictionary<string, int> drink)
    {
        double fulfilment = findFulfiment(drink);
        double points = Math.Pow(fulfilment, 2) * maxPoints ;
        points *= Math.Pow((maxToGive - givenPoints) / maxToGive, 2);
        int test = (int) fulfilment * 10;
        if (test > 9)
        {
            test = 9;
        }
        if (test < 0)
        {
            test = 0;
        }

        if (bonus > 0)
        {
            points *= Math.Pow(test, 1.3);
            bonus -= Math.Pow(test, 1.3);
        }
        givenPoints += points;
        return points;
    }
}