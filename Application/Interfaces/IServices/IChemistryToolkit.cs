using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Bộ công cụ hóa học - AI sẽ dùng để phân tích
    /// </summary>
    public interface IChemistryToolkit
    {
        Task<EquationBalanceResult> BalanceEquationAsync(string equation);
        Task<MolecularAnalysisResult> AnalyzeMoleculeAsync(string formula);
        Task<ReactionCheckResult> CheckReactionFeasibilityAsync(string reactant1, string reactant2);
    }

    public class EquationBalanceResult
    {
        public bool Success { get; set; }
        public string BalancedEquation { get; set; } = string.Empty;
        public List<int> Coefficients { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class MolecularAnalysisResult
    {
        public string Formula { get; set; } = string.Empty;
        public double MolarMass { get; set; }
        public string CommonName { get; set; } = string.Empty;
        public List<string> FunctionalGroups { get; set; } = new();
        public string StateAtRoomTemp { get; set; } = string.Empty;
    }

    public class ReactionCheckResult
    {
        public bool CanReact { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string? PredictedProduct { get; set; }
    }
}
