using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NQueensGeneticAlgorithmMVC.Controllers;

namespace NQueensGeneticAlgorithmMVC.Models
{
    public enum GeneticAlgorithmWay
    {
        Standard,
        Permutation
    }
    public enum GeneticAlgorithmCrossOverWay
    {
        SinglePoint,
        Uniformity,
        OrderBasedUniformity
    }
    public enum GeneticSelectionWay
    {
        Standard,
        TournamentSelection,
        RouletteWheelSelection
    }
    public enum GeneticAlgorithmMutationWay
    {
        Standard,
        ChangePosition
    }
    public class GeneticAlgorithmViewModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public long time { get; set; }

        [Display(Name = "population")]
        public int populationSize { get; set; }
        [Display(Name = "sample size")]
        public int n { get; set; }
        [Display(Name = "generation progress count")]
        public int generationLength { get; set; }
        [Display(Name = "mutation rate (0-1)")]
        public double mutationRate { get; set; }
        [Display(Name = "roulette wheel size (number of roulette wheel members)")]
        public int rouletteMembersCount { get; set; }
        [Display(Name = "tournament size")]
        public int tournamentMembersCount { get; set; }
        [Display(Name = "algorithm method")]
        public GeneticAlgorithmWay geneticAlgorithmWay { get; set; }
        [Display(Name = "mutation method")]
        public GeneticAlgorithmMutationWay geneticAlgorithmMutationWay { get; set; }
        [Display(Name = "crossover method")]
        public GeneticAlgorithmCrossOverWay geneticAlgorithmCrossOverWay { get; set; }
        [Display(Name = "selection method")]
        public GeneticSelectionWay geneticSelectionWay { get; set; }
        public List<int[]>? resultPopulation { get; set; }
        public List<int[]>? comparePopulation { get; set; }
    }
}
