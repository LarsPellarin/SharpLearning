﻿using SharpLearning.Containers.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLearning.DecisionTrees.ImpurityCalculators
{
    /// <summary>
    /// Regression impurity calculator using variance and friedmans
    /// calculation for impurity improvement.
    /// </summary>
    public sealed class RegressionImpurityCalculator
    {
        Interval1D m_interval;
        int m_currentPosition;

        double m_weightedTotal = 0.0;
        double m_weightedLeft = 0.0;
        double m_weightedRight = 0.0;

        double m_meanLeft = 0.0;
        double m_meanRight = 0.0;
        double m_meanTotal = 0.0;
               
        double m_sqSumLeft = 0.0;
        double m_sqSumRight = 0.0;
        double m_sqSumTotal = 0.0;
               
        double m_varRight = 0.0;
        double m_varLeft = 0.0;
               
        double m_sumLeft = 0.0;
        double m_sumRight = 0.0;
        double m_sumTotal = 0.0;
        
        double[] m_targets;
        double[] m_weights;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueTargets"></param>
        /// <param name="targets"></param>
        /// <param name="weights"></param>
        /// <param name="interval"></param>
        public RegressionImpurityCalculator(double[] targets, double[] weights, Interval1D interval)
        {
            if (targets == null) { throw new ArgumentException("targets"); }
            if (weights == null) { throw new ArgumentException("weights"); }
            m_targets = targets;
            m_weights = weights;
            m_interval = interval;

            var w = 1.0;
            var weightsPresent = m_weights.Length != 0;

            for (int i = m_interval.FromInclusive; i < m_interval.ToExclusive; i++)
            {
                if (weightsPresent)
                    w = weights[i];

                var targetValue = targets[i];
                var wTarget = w * targetValue;
                m_sumTotal += wTarget;
                m_sqSumTotal += wTarget * targetValue;

                m_weightedTotal += w;
            }

            m_meanTotal = m_sumTotal / m_weightedTotal;

            m_currentPosition = m_interval.FromInclusive;
            this.Reset();
        }

        /// <summary>
        /// Resets impurity calculator
        /// </summary>
        public void Reset()
        {
            m_currentPosition = m_interval.FromInclusive;

            m_weightedLeft = 0.0;
            m_weightedRight = m_weightedTotal;

            m_meanRight = m_meanTotal;
            m_meanLeft = 0.0;
            m_sumRight = m_sqSumTotal;
            m_sqSumLeft = 0.0;

            m_varRight = (m_sqSumRight / m_weightedTotal -
                m_meanRight * m_meanRight);
            m_varLeft = 0.0;

            m_sumRight = m_sumTotal;
            m_sumLeft = 0.0;
        }

        /// <summary>
        /// Updates impurity calculator with new split index
        /// </summary>
        /// <param name="newPosition"></param>
        public void Update(int newPosition)
        {
            if (m_currentPosition > newPosition)
            {
                throw new ArgumentException("New position: " + newPosition +
                    " must be larget than current: " + m_currentPosition);
            }

            var weightsPresent = m_weights.Length != 0;
            var w = 1.0;
            var w_diff = 0.0;
            
            for (int i = m_currentPosition; i < newPosition; i++)
            {
                if (weightsPresent)
                    w = m_weights[i];

                var targetValue = m_targets[i];
                var wTarget = w * targetValue;

                m_sumLeft += wTarget;
                m_sumRight -= wTarget;

                var wTargetSq = wTarget * targetValue;

                m_sqSumLeft += wTargetSq;
                m_sqSumRight -= wTargetSq;

                w_diff += w;
            }

            m_weightedLeft += w_diff;
            m_weightedRight -= w_diff;

            m_meanLeft = m_sumLeft / m_weightedLeft;
            m_meanRight = m_sumRight / m_weightedRight;

            m_varLeft = (m_sqSumLeft / m_weightedLeft -
                m_meanLeft * m_meanLeft);

            m_varRight = (m_sqSumRight / m_weightedRight -
                m_meanRight * m_meanRight);

            m_currentPosition = newPosition;
        }

        /// <summary>
        /// Calculate the node impurity
        /// </summary>
        /// <returns></returns>
        public double NodeImpurity()
        {
            var impurity = (m_sqSumTotal / m_weightedTotal -
                m_meanTotal * m_meanTotal);

            return impurity;
        }

        /// <summary>
        /// Calculates child impurities with current split index
        /// </summary>
        /// <returns></returns>
        public ChildImpurities ChildImpurities()
        {
            var impurityLeft = (m_sqSumLeft / m_weightedLeft -
                m_meanLeft * m_meanLeft);

            var impurityRight = (m_sqSumRight / m_weightedRight -
                m_meanRight * m_meanRight); ;
            
            return new ChildImpurities(impurityLeft, impurityRight);
        }

        /// <summary>
        /// Calculates the impurity improvement at the current split index
        /// </summary>
        /// <param name="impurity"></param>
        /// <returns></returns>
        public double ImpurityImprovement(double impurity)
        {
            var diff = ((m_sumLeft / m_weightedLeft) -
                (m_sumRight / m_weightedRight));

            var improvement = (m_weightedLeft * m_weightedRight * diff * diff /
                (m_weightedLeft + m_weightedRight));

            return improvement;
        }

        /// <summary>
        /// Calculates the weighted leaf value
        /// </summary>
        /// <returns></returns>
        public double LeafValue()
        {
            return m_meanTotal;
        }
    }
}