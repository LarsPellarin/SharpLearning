﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpLearning.DecisionTrees.Learners;
using SharpLearning.Metrics.Entropy;
using SharpLearning.DecisionTrees.LeafFactories;

namespace SharpLearning.DecisionTrees.Test.Learners
{
    [TestClass]
    public class CartLearnerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CartLearner_InvalidMinimumSplitSize()
        {
            new CartLearner(0, 1, 0.1, 
                new GiniImpurityMetric(), 
                new AllFeatureCandidateSelector(), 
                new ClassificationLeafFactory());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CartLearner_InvalidMaximumTreeSize()
        {
            new CartLearner(1, 0, 0.1,
                new GiniImpurityMetric(),
                new AllFeatureCandidateSelector(),
                new ClassificationLeafFactory());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CartLearner_InvalidMinimumInformationGain()
        {
            new CartLearner(1, 1, 0,
                new GiniImpurityMetric(),
                new AllFeatureCandidateSelector(),
                new ClassificationLeafFactory());
        }
    }
}