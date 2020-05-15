using _ActionPoint;
using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Traversal;

namespace _AI._DecisionTree._Algorithm
{
    /// <summary>
    /// Interface implemented by any <see cref="ADecisionNode"/> that involves the creation of event that consumes <see cref="ActionPoint"/>.
    /// </summary>
    public interface IAtionPointPredictable
    {
        /// <summary>
        /// Every <see cref="ActionPoint"/> prodiction is represented by an entry in <paramref name="p_actionPointPredictions"/>.
        /// A single <see cref="IAtionPointPredictable"/> can push multiple predictions as the node can leads to multiple unit events.
        /// An item in the return list means that a future <see cref="_EventQueue.AEvent"/> will be created and it's execution will consume the amount of <see cref="ActionPoint"/>.
        /// </summary>
        void AddActionPointPrediction(List<float> p_actionPointPredictions);
    }

    public static class ActionPointPrediction
    {
        /// <summary>
        /// Build a list of <see cref="ActionPoint"/> consmption predictions.
        /// <see cref="ActionPoint"/> consmption predictions are represented by etries in the return list.
        /// See <see cref="IAtionPointPredictable.AddActionPointPrediction(List{float})"/> for more details.
        /// </summary>
        public static List<float> predictActionPointConsumptions(ref AIDecisionTreeChoice p_choice)
        {
            List<float> l_return = new List<float>();
            for (int i = 0; i < p_choice.DecisionNodesChoiceOrdered.Length; i++)
            {
                ADecisionNode l_decisionNode = p_choice.DecisionNodesChoiceOrdered[i];
                if (l_decisionNode is IAtionPointPredictable)
                {
                    ((IAtionPointPredictable)l_decisionNode).AddActionPointPrediction(l_return);
                }
            }
            return l_return;
        }
    }
}
