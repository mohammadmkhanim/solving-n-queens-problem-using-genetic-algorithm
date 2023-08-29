using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NQueensGeneticAlgorithmMVC.Models;

namespace NQueensGeneticAlgorithmMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMapper _mapper;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(GeneticAlgorithmViewModel geneticAlgorithmViewModel)
    {
        geneticAlgorithmViewModel.id = Guid.NewGuid().ToString();

        GeneticAlgorithm.populationSize = geneticAlgorithmViewModel.populationSize;
        GeneticAlgorithm.n = geneticAlgorithmViewModel.n;
        GeneticAlgorithm.generationLength = geneticAlgorithmViewModel.generationLength;
        GeneticAlgorithm.mutationRate = geneticAlgorithmViewModel.mutationRate;
        GeneticAlgorithm.rouletteMembersCount = geneticAlgorithmViewModel.rouletteMembersCount;
        GeneticAlgorithm.geneticAlgorithmWay = geneticAlgorithmViewModel.geneticAlgorithmWay;
        GeneticAlgorithm.geneticAlgorithmMutationWay = geneticAlgorithmViewModel.geneticAlgorithmMutationWay;
        GeneticAlgorithm.geneticAlgorithmCrossOverWay = geneticAlgorithmViewModel.geneticAlgorithmCrossOverWay;
        GeneticAlgorithm.geneticSelectionWay = geneticAlgorithmViewModel.geneticSelectionWay;
        Stopwatch timer = new Stopwatch();
        timer.Start();
        var result = GeneticAlgorithm.DoAlgorithm();
        timer.Stop();
        geneticAlgorithmViewModel.time = timer.ElapsedMilliseconds;
        geneticAlgorithmViewModel.resultPopulation = result;

        return View("resultPartalView", geneticAlgorithmViewModel);
    }
}


public class GeneticAlgorithm
{
    public static int populationSize = 100;
    public static int n = 4;
    public static int generationLength = 10000;
    public static double mutationRate = 0.3;
    public static int rouletteMembersCount = 10;
    public static int tournamentMembersCount = 10;
    public static GeneticAlgorithmWay geneticAlgorithmWay;
    public static GeneticAlgorithmMutationWay geneticAlgorithmMutationWay;
    public static GeneticAlgorithmCrossOverWay geneticAlgorithmCrossOverWay;
    public static GeneticSelectionWay geneticSelectionWay;

    // public static List<int[]> resultPopulation;

    public static int[] FindBestAnswer(List<int[]> population)
    {
        return population.MinBy(p => GeneticAlgorithm.CalculateFitness(p));
    }

    public static int[] ModifyMember(int[] member)
    {
        bool flag = true;
        Random r = new Random();
        for (int i = 0; i < member.Length; i++)
        {
            if (member.Where(b => b == member[i]).Count() > 1)
            {
                member[i] = r.Next(member.Length);
                flag = false;
            }
        }
        if (flag)
        {
            return member;
        }
        return ModifyMember(member);
    }

    public static bool IsCorrectPermutation(int[] member)
    {
        bool flag = true;
        for (int i = 0; i < member.Length; i++)
        {
            if (member.Where(b => b == member[i]).Count() > 1)
            {
                flag = false;
            }
        }
        return flag;
    }

    public static int[] GenerateMember()
    {
        Random random = new Random();
        var member = new int[n];
        if (geneticAlgorithmWay == GeneticAlgorithmWay.Standard)
        {
            for (int j = 0; j < n; j++)
            {
                member[j] = random.Next(n);
            }
        }
        if (geneticAlgorithmWay == GeneticAlgorithmWay.Permutation)
        {
            // Initialize the array with sequential numbers
            for (int i = 0; i < n; i++)
            {
                member[i] = i;
            }

            // Shuffle the array using Fisher-Yates algorithm
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = member[i];
                member[i] = member[j];
                member[j] = temp;
            }
        }
        return member;
    }

    public static List<int[]> GeneratePopulation()
    {
        var population = new List<int[]>();
        for (int i = 0; i < populationSize; i++)
        {
            var member = GenerateMember();
            population.Add(member);
        }
        return population;
    }

    public static int CalculateFitness(int[] member)
    {
        int fitness = 0;
        for (int j = 0; j < n; j++)
        {
            for (int k = j + 1; k < n; k++)
            {
                int[] c1 = new int[2] { j, member[j] };
                int[] c2 = new int[2] { k, member[k] };
                if (IsCollide(c1, c2))
                {
                    fitness++;
                }
            }
        }
        return fitness;
    }

    public static bool IsCollide(int[] c1, int[] c2)
    {
        var res = false;
        if (c1[1] == c2[1])
        {
            res = true;
        }
        else if (Math.Abs(c1[0] - c2[0]) == Math.Abs(c1[1] - c2[1]))
        {
            res = true;
        }
        return res;
    }

    public static List<int[]> Selection(List<int[]> population)
    {
        List<int[]> selectedPopulation = new List<int[]>();
        if (geneticSelectionWay == GeneticSelectionWay.Standard)
        {
            List<int[]> sortedPopulation = population.OrderBy(m => CalculateFitness(m)).ToList();
            selectedPopulation = sortedPopulation.Take(new Range(0, populationSize)).ToList();
        }
        if (geneticSelectionWay == GeneticSelectionWay.RouletteWheelSelection)
        {
            Random random = new Random();
            for (int k = 0; k < populationSize; k++)
            {
                var members = new List<int[]>();
                var roulette = new List<int[]>();
                for (int i = 0; i < rouletteMembersCount; i++)
                {
                    var randomMemberIndex = random.Next(population.Count);
                    int[] randomMember = population[randomMemberIndex];
                    members.Add(randomMember);
                }
                int sumFitness = members.Select(m => CalculateFitness(m)).Sum();
                for (int i = 0; i < rouletteMembersCount; i++)
                {
                    int memberFitness = CalculateFitness(members[i]);
                    for (int j = 0; j < sumFitness - memberFitness; j++)
                    {
                        roulette.Add(members[i]);
                    }
                }
                random = new Random();
                var selectedMemberIndex = random.Next(roulette.Count);
                var selectedMember = roulette[selectedMemberIndex];
                selectedPopulation.Add(selectedMember);
            }
        }
        if (geneticSelectionWay == GeneticSelectionWay.TournamentSelection)
        {
            Random random = new Random();
            for (int k = 0; k < populationSize; k++)
            {
                var members = new List<int[]>();
                for (int i = 0; i < tournamentMembersCount; i++)
                {
                    var randomMemberIndex = random.Next(population.Count);
                    int[] randomMember = population[randomMemberIndex];
                    members.Add(randomMember);
                }
                var selectedMember = members.MinBy(m => CalculateFitness(m));
                selectedPopulation.Add(selectedMember);
            }
        }

        return selectedPopulation;
    }

    public static List<int[]> Mutation(List<int[]> population)
    {
        for (int i = 0; i < populationSize * mutationRate; i++)
        {
            Random random = new Random();
            var randomMember = population[random.Next(populationSize)];
            if (geneticAlgorithmMutationWay == GeneticAlgorithmMutationWay.Standard)
            {
                int newValue = random.Next(n);
                int randomIndex = random.Next(n);
                randomMember[randomIndex] = newValue;
                if (geneticAlgorithmWay == GeneticAlgorithmWay.Permutation)
                {
                    if (!IsCorrectPermutation(randomMember))
                    {
                        randomMember = ModifyMember(randomMember);
                    }
                }

            }
            if (geneticAlgorithmMutationWay == GeneticAlgorithmMutationWay.ChangePosition)
            {
                int randomIndex1 = random.Next(n);
                int randomIndex2 = random.Next(n);
                int temp = randomMember[randomIndex1];
                randomMember[randomIndex1] = randomMember[randomIndex2];
                randomMember[randomIndex2] = temp;
            }
        }
        return population;
    }

    public static List<int[]> Crossover(int[] a, int[] b)
    {
        int half = (int)Math.Floor((decimal)n / 2);
        List<int[]> res = new List<int[]>();
        int[] child1 = new int[n];
        int[] child2 = new int[n];

        if (geneticAlgorithmCrossOverWay == GeneticAlgorithmCrossOverWay.SinglePoint)
        {
            for (int i = 0; i < n; i++)
            {
                if (i < half)
                {
                    child1[i] = a[i];
                    child2[i] = b[i];
                }
                else
                {
                    child1[i] = b[i];
                    child2[i] = a[i];
                }
            }
            if (geneticAlgorithmWay == GeneticAlgorithmWay.Permutation)
            {
                if (!IsCorrectPermutation(child1))
                {
                    child1 = ModifyMember(child1);
                }
                if (!IsCorrectPermutation(child2))
                {
                    child2 = ModifyMember(child2);
                }
            }
        }
        if (geneticAlgorithmCrossOverWay == GeneticAlgorithmCrossOverWay.Uniformity)
        {
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                child1[i] = a[i];
                child2[i] = b[i];
                int sign = random.Next(2);
                if (sign == 1)
                {
                    int temp = a[i];
                    child1[i] = b[i];
                    child2[i] = temp;
                }
            }
            if (geneticAlgorithmWay == GeneticAlgorithmWay.Permutation)
            {
                if (!IsCorrectPermutation(child1))
                {
                    child1 = ModifyMember(child1);
                }
                if (!IsCorrectPermutation(child2))
                {
                    child2 = ModifyMember(child2);
                }
            }
        }
        if (geneticAlgorithmCrossOverWay == GeneticAlgorithmCrossOverWay.OrderBasedUniformity)
        {
            Random random = new Random();
            List<int> aValues = new List<int>();
            List<int> bValues = new List<int>();
            List<int> indexes = new List<int>();
            for (int i = 0; i < n; i++)
            {
                int sign = random.Next(2);
                if (sign == 1)
                {
                    aValues.Add(a[i]);
                    bValues.Add(b[i]);
                    indexes.Add(i);
                }
            }
            List<int> newOrderAValues = new List<int>();
            List<int> newOrderBValues = new List<int>();
            for (int j = 0; j < b.Length; j++)
            {
                for (int i = 0; i < aValues.Count; i++)
                {
                    if (aValues[i] == b[j])
                    {
                        newOrderAValues.Add(aValues[i]);
                    }
                    if (bValues[i] == a[j])
                    {
                        newOrderBValues.Add(bValues[i]);
                    }
                }
            }
            a.CopyTo(child1, 0);
            b.CopyTo(child2, 0);
            for (int i = 0; i < indexes.Count; i++)
            {
                child1[indexes[i]] = newOrderAValues.First();
                newOrderAValues.RemoveAt(0);
                child2[indexes[i]] = newOrderBValues.First();
                newOrderBValues.RemoveAt(0);
            }

        }

        res.Add(child1);
        res.Add(child2);
        return res;
    }
    public static List<int[]> Crossover(List<int[]> population)
    {
        for (int i = 0; i < populationSize; i++)
        {
            Random random = new Random();
            int aIndex = random.Next(populationSize);
            int bIndex = random.Next(populationSize);
            var childs = Crossover(population[aIndex], population[bIndex]);
            population.AddRange(childs);
        }
        return population;
    }

    public static List<int[]> DoAlgorithm()
    {
        List<int[]> population = GeneratePopulation();
        for (int j = 0; j < generationLength; j++)
        {
            if (j != 0)
            {
                population = Mutation(population);
            }
            population = Crossover(population);
            population = Selection(population);
            var bestAnswer = FindBestAnswer(population);
            if (CalculateFitness(bestAnswer) == 0)
            {
                return population;
            }
        }
        // resultPopulation = population;
        return population;
    }

    public static List<int[]> DoAlgorithm(List<int[]> population)
    {
        for (int j = 0; j < generationLength; j++)
        {
            if (j != 0)
            {
                population = Mutation(population);
            }
            population = Crossover(population);
            population = Selection(population);
            var bestAnswer = FindBestAnswer(population);
            if (CalculateFitness(bestAnswer) == 0)
            {
                return population;
            }
        }
        // resultPopulation = population;
        return population;
    }

}
