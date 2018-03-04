using System;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class ScenarioRunnerTest
    {
        private readonly SimulationsRunner _sut;

        public ScenarioRunnerTest()
        {
            _sut = new SimulationsRunner();
        }

        [TestMethod]
        public void When_Running_Scenario_Without_Loading_Simulation_Should_Throw_Exception()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Run(100));
        }
    }
}
